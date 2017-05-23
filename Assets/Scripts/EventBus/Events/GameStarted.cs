using EventBus;

public class GameStarted : EBEvent
{
    public readonly int gameIndex;
    public readonly long timestamp;

    public GameStarted(int gameIndex, long timestamp)
    {
        this.type = EBEventType.GameStarted;
        this.gameIndex = gameIndex;
        this.timestamp = timestamp;
    }
}
