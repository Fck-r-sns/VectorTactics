using EventBus;

public class GameStarted : EBEvent
{
    public readonly long timestamp;

    public GameStarted(long timestamp)
    {
        this.type = EBEventType.GameStarted;
        this.timestamp = timestamp;
    }
}
