using EventBus;

public class HealthChanged : EBEvent
{

    public readonly float value;

    public HealthChanged(float value, global::Defines.Side soldier)
    {
        this.type = (soldier == Defines.Side.Blue) ? EBEventType.BlueSoldierHealthChanged : EBEventType.RedSoldierHealthChanged;
        this.value = value;
    }
}
