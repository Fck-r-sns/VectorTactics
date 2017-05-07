namespace Ai
{
    namespace Bt
    {
        public class IsHealthMoreThan : SoldierNode
        {
            private float value;

            public IsHealthMoreThan(Environment environment, AiTools aiTools, float healthValue) : base(environment, aiTools)
            {
                this.value = healthValue;
            }

            public override Result Run()
            {
                return aiTools.agentState.health > value ? Result.Success : Result.Failure;
            }
        }
    }
}