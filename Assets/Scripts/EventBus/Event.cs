using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventBus
{
    public enum EBEventType
    {
        BlueSoldierHealthChanged,
        RedSoldierHealthChanged
    }

    public enum TriggerAction
    {
        Enter,
        Exit
    }

    public class EBEvent
    {
        public EBEventType type;
        public int address = Defines.BROADCAST_ADDRESS;

        public override string ToString()
        {
            return "Event(type=" + type + ";address=" + address + ")";
        }
    }
}
