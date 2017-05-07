namespace Ai
{
    namespace Bt
    {
        public class IsTargetVisible : SoldierNode
        {
            public IsTargetVisible(Environment environment, AiTools aiTools) : base(environment, aiTools)
            {
            }

            public override Result Run()
            {
                return aiTools.agentState.isEnemyVisible ? Result.Success : Result.Failure;
            }
        }
    }
}