namespace Ai
{
    namespace Bt
    {
        public class StartShooting : SoldierNode
        {
            public StartShooting(Environment environment, AiTools aiTools) : base(environment, aiTools)
            {
            }

            public override Result Run()
            {
                aiTools.shooting.SetAimingEnabled(true);
                aiTools.shooting.SetShootingEnabled(true);
                return Result.Success;
            }
        }
    }
}