using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EventBus
{

    public class Dispatcher : MonoBehaviour
    {

        [SerializeField]
        private AddressProvider addressProvider;

        private static Dispatcher instance;
        private Dictionary<EBEventType, Dictionary<int, GameObject>> subscribers;
        private int activeQueueIndex = 0;
        private Queue<EBEvent>[] queues;
        private Queue<EBEvent> activeQueue; // double buffer, protection from infinite event loops

        public static Dispatcher GetInstance()
        {
            return instance;
        }

        public int GetFreeAddress()
        {
            return addressProvider.GetFreeAddress();
        }

        public void Subscribe(EBEventType eventType, int address, GameObject subscriber)
        {
            if (!subscribers.ContainsKey(eventType))
            {
                subscribers.Add(eventType, new Dictionary<int, GameObject>());
            }
            subscribers[eventType][address] = subscriber;
        }

        public void Unsubscribe(EBEventType eventType, int address)
        {
            if (subscribers.ContainsKey(eventType))
            {
                subscribers[eventType].Remove(address);
            }
        }

        public void SendEvent(EBEvent e)
        {
            activeQueue.Enqueue(e);
        }

        private void DispatchEvent(EBEvent e)
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

        private void Awake()
        {
            instance = this;
            subscribers = new Dictionary<EBEventType, Dictionary<int, GameObject>>();
            queues = new Queue<EBEvent>[] { new Queue<EBEvent>(), new Queue<EBEvent>() };
            activeQueue = queues[activeQueueIndex];
            subscribers.Clear();
            foreach (var queue in queues)
            {
                queue.Clear();
            }
        }

        private void Update()
        {
            Queue<EBEvent> queue = activeQueue;
            SwapQueues();
            foreach (EBEvent e in queue)
            {
                DispatchEvent(e);
            }
            queue.Clear();
        }

        private void SwapQueues()
        {
            activeQueueIndex = 1 - activeQueueIndex;
            activeQueue = queues[activeQueueIndex];
        }
    }
}