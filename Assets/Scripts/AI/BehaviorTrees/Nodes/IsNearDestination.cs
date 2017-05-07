using UnityEngine;

namespace Ai
{
    namespace Bt
    {
        public class IsNearDestination : SoldierNode
        {
            public IsNearDestination(Environment environment, AiTools aiTools) : base(environment, aiTools)
            {
            }

            public override Result Run()
            {
                Vector3? destination = aiTools.navigation.GetDestination();
                if (!destination.HasValue)
                {
                    return Result.Failure;
                }
                Vector3 position = aiTools.agentState.position;
                return Vector3.Distance(destination.Value, position) < GameDefines.NEAR_POINT_CHECK_PRECISION ? Result.Success : Result.Failure;
            }
        }
    }
}