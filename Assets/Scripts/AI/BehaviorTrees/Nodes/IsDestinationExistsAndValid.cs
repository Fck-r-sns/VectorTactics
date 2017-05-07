namespace Ai
{
    namespace Bt
    {
        public class IsDestinationExistsAndValid : SoldierNode
        {
            public IsDestinationExistsAndValid(Environment environment, AiTools aiTools) : base(environment, aiTools)
            {
            }

            public override Result Run()
            {
                return aiTools.navigation.IsDestinationReachable() ? Result.Success : Result.Failure;
            }
        }
    }
}