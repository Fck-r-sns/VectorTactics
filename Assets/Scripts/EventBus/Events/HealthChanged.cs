using EventBus;

public class HealthChanged : EBEvent
{
    public readonly GameDefines.Side side;
    public readonly float value;
    public readonly float diff;
    public readonly bool isHeal;

    public HealthChanged(GameDefines.Side side, float value, float diff, bool isHeal)
    {
        this.type = EBEventType.HealthChanged;
        this.side = side;
        this.value = value;
        this.diff = diff;
        this.isHeal = isHeal;
    }
}
