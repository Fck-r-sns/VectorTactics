using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EventBus
{

    public class Dispatcher : MonoBehaviour
    {
        private static Dictionary<EBEventType, Dictionary<int, GameObject>> subscribers = new Dictionary<EBEventType, Dictionary<int, GameObject>>();
        private static int activeQueueIndex = 0;
        private static Queue<EBEvent>[] queues = { new Queue<EBEvent>(), new Queue<EBEvent>() };
        private static Queue<EBEvent> activeQueue = queues[activeQueueIndex]; // double buffer, protection from infinite event loops

        public static void Subscribe(EBEventType eventType, int address, GameObject subscriber)
        {
            if (!subscribers.ContainsKey(eventType))
            {
                subscribers.Add(eventType, new Dictionary<int, GameObject>());
            }
            subscribers[eventType].Add(address, subscriber);
        }

        public static void Unsubscribe(EBEventType eventType, int address)
        {
            if (subscribers.ContainsKey(eventType))
            {
                subscribers[eventType].Remove(address);
            }
        }

        public static void SendEvent(EBEvent e)
        {
            activeQueue.Enqueue(e);
        }

        public static void DispatchEvent(EBEvent e)
        {
            if (!subscribers.ContainsKey(e.type))
            {
                return;
            }

            Dictionary<int, GameObject> typeSubscribers = subscribers[e.type];
            if (e.address == Defines.BROADCAST_ADDRESS)
            {
                foreach (var entry in typeSubscribers)
                {
                    ExecuteEvents.Execute<IEventSubscriber>(entry.Value, null, (handler, data) => handler.OnReceived(e));
                }
            }
            else
            {
                if (!typeSubscribers.ContainsKey(e.address))
                {
                    return;
                }

                ExecuteEvents.Execute<IEventSubscriber>(typeSubscribers[e.address], null, (handler, data) => handler.OnReceived(e));
            }
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Queue<EBEvent> queue = activeQueue;
            swapQueues();
            foreach(EBEvent e in queue)
            {
                DispatchEvent(e);
            }
            queue.Clear();
        }

        private void swapQueues()
        {
            activeQueueIndex = 1 - activeQueueIndex;
            activeQueue = queues[activeQueueIndex];
        }
    }
}