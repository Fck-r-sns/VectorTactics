using EventBus;

public class HealthChanged : EBEvent
{

    public readonly float value;

    public HealthChanged(float value, global::GameDefines.Side soldier)
    {
        this.type = (soldier == GameDefines.Side.Blue) ? EBEventType.BlueSoldierHealthChanged : EBEventType.RedSoldierHealthChanged;
        this.value = value;
    }
}
