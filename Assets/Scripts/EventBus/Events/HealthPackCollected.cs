using EventBus;

public class HealthPackCollected : EBEvent
{
    public readonly GameDefines.Side side;

    public HealthPackCollected(GameDefines.Side side)
    {
        this.type = EBEventType.HealthPackCollected;
        this.side = side;
    }
}
