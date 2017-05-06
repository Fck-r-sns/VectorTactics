using UnityEngine;

namespace Ai
{

    namespace Fsm
    {

        public abstract class SoldierState : State
        {
            protected readonly AiTools aiTools;
            protected Vector3? destination;

            public SoldierState(AiTools aiTools)
            {
                this.aiTools = aiTools;
            }

            public override void OnEnter()
            {
                destination = null;
            }

            public override void OnUpdate()
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
                    || (destination.HasValue && Vector3.Distance(aiTools.agentState.position, destination.Value) < 2.0f)
                    )
                {
                    destination = null;
                }
            }

            protected abstract Vector3 GetNextDestination();
        }

    }
}