using EventBus;

public class NewFrame : EBEvent
{
    public readonly GameDefines.Side side;
    public readonly float frameTime;

    public NewFrame(GameDefines.Side side, float frameTime)
    {
        this.type = EBEventType.NewFrame;
        this.side = side;
        this.frameTime = frameTime;
    }
}
