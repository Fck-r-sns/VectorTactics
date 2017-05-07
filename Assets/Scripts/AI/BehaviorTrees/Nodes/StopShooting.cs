namespace Ai
{
    namespace Bt
    {
        public class StopShooting : SoldierNode
        {
            public StopShooting(Environment environment, AiTools aiTools) : base(environment, aiTools)
            {
            }

            public override Result Run()
            {
                aiTools.shooting.SetAimingEnabled(false);
                aiTools.shooting.SetShootingEnabled(false);
                return Result.Success;
            }
        }
    }
}