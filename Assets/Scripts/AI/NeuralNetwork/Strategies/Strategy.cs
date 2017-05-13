using UnityEngine;

namespace Ai
{
    namespace Nn
    {
        public abstract class Strategy
        {
            protected readonly AiTools aiTools;
            protected Vector3? destination;

            public Strategy(AiTools aiTools)
            {
                this.aiTools = aiTools;
            }

            public virtual void OnEnter()
            {
                destination = null;
            }

            public virtual void OnUpdate()
            {
                if (aiTools.agentState.isDead)
                {
                    aiTools.navigation.SetDestination(null);
                    return;
                }

                if (!destination.HasValue)
                {
                    destination = GetNextDestination();
                    aiTools.navigation.SetDestination(destination);
                }

                if (
                    !aiTools.navigation.IsDestinationReachable()
                    || (destination.HasValue && Vector3.Distance(aiTools.agentState.position, destination.Value) < GameDefines.NEAR_POINT_CHECK_PRECISION)
                    )
                {
                    destination = null;
                }
            }

            public virtual void OnExit()
            {

            }

            protected abstract Vector3 GetNextDestination();
        }
    }
}