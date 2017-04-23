using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    namespace Fsm
    {
        public class Attack : SoldierState
        {

            private const float MOVEMENT_RADIUS = 3.0f;

            private Vector3? destination;

            public Attack(AiTools aiTools) :
                base(aiTools)
            {
            }

            public override void OnEnter()
            {
                Debug.Log(Time.time + ": Enter Attack state");
                aiTools.shooting.SetAimingEnabled(true);
                aiTools.shooting.SetShootingEnabled(true);
            }

            public override void OnUpdate()
            {
                if (!destination.HasValue)
                {
                    destination = GetNextDestination();
                    aiTools.navigation.SetDestination(destination);
                }

                if (
                    !aiTools.navigation.IsDestinationReachable()
                    || (destination.HasValue && Vector3.Distance(aiTools.agentState.position, destination.Value) < 2.0f)
                    || (destination.HasValue && aiTools.terrain.GetNearestWaypoint(destination.Value).isBehindWall)
                    )
                {
                    destination = null;
                }
            }

            public override void OnExit()
            {

            }

            private Vector3 GetNextDestination()
            {
                return new Vector3(
                    aiTools.agentState.position.x + Random.Range(-MOVEMENT_RADIUS, MOVEMENT_RADIUS),
                    0.0f,
                    aiTools.agentState.position.z + Random.Range(-MOVEMENT_RADIUS, MOVEMENT_RADIUS)
                    );
            }

        }
    }
}