using UnityEngine;

namespace Ai
{
    namespace Bt
    {
        public class MoveToPoint : SoldierNode
        {
            private string pointVariable;

            public MoveToPoint(Environment environment, AiTools aiTools, string pointVariable) : base(environment, aiTools)
            {
                this.pointVariable = pointVariable;
            }

            public override Result Run()
            {
                if (!environment.ContainsValue(pointVariable))
                {
                    return Result.Failure;
                }
                Vector3 point = environment.GetValue<Vector3>(pointVariable);
                aiTools.navigation.SetDestination(point);
                return Result.Success;
            }
        }
    }
}