using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    namespace Fsm
    {
        public class Defence : SoldierState
        {

            private const float MOVEMENT_RADIUS = 5.0f;

            public Defence(AiTools aiTools) : 
                base(aiTools)
            {
            }

            public override void OnEnter()
            {
                base.OnEnter();
                Debug.Log(Time.time + ": Enter Defence state");
                aiTools.shooting.SetAimingEnabled(true);
                aiTools.shooting.SetShootingEnabled(true);

                aiTools.terrain.SetWeightFunction(TerrainReasoning.DEFENSIVE_WEIGHT_FUNCTION);
            }

            protected override Vector3 GetNextDestination()
            {
                List<Waypoint> wps = aiTools.terrain.GetGoodWaypoints(MOVEMENT_RADIUS, 0.75f);
                if (wps.Count == 0)
                {
                    return GetRandomNextDestination();
                }
                else
                {
                    int index = Random.Range(0, wps.Count - 1);
                    return wps[index].position;
                }
            }

            private Vector3 GetRandomNextDestination()
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