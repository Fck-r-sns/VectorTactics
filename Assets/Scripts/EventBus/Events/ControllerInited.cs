using EventBus;

public class ControllerInited : EBEvent
{
    public readonly GameDefines.Side side;
    public readonly GameDefines.ControllerType controllerType;

    public ControllerInited(GameDefines.Side side, GameDefines.ControllerType controllerType)
    {
        this.type = EBEventType.ControllerInited;
        this.side = side;
        this.controllerType = controllerType;
    }
}
