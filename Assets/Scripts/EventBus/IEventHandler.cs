using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EventBus
{
    public interface IEventSubscriber : IEventSystemHandler
    {
        void OnReceived(EBEvent e);
    }
}