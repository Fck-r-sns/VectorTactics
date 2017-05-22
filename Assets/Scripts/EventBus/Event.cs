namespace EventBus
{
    public enum EBEventType
    {
        GameStarted,
        ControllerInited,
        NewFrame,
        HealthChanged,
        HealthPackCollected,
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
