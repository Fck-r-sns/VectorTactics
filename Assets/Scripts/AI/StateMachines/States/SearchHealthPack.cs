using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    namespace Fsm
    {

        public class SearchHealthPack : SoldierState
        {

            private const float MOVEMENT_RADIUS = 50.0f;
            private float nextDestinationUpdateTime = 0.0f;

            public SearchHealthPack(AiTools aiTools) : 
                base(aiTools)
            {
            }

            public override void OnEnter()
            {
                base.OnEnter();
                Debug.Log(Time.time + ": Enter SearchHealthPack state");

                aiTools.terrain.SetWeightFunction(wp =>
                {
                    float weight = 0.0f;
                    if (wp.isBehindWall)
                    {
                        weight += 0.4f;
                    }
                    if (wp.isHealthPack)
                    {
                        weight += 0.3f;
                        weight += 0.2f * Mathf.Clamp01(wp.movementDistanceToAgent / 10.0f);
                        weight -= 0.2f * Mathf.Clamp01(wp.movementDistanceToEnemy / 10.0f);
                    }
                    return weight;
                });
            }

            public override void OnUpdate()
            {
                if (Time.time > nextDestinationUpdateTime)
                {
                    nextDestinationUpdateTime = Time.time + 1.0f;
                    destination = null;
                }
                aiTools.shooting.SetAimingEnabled(aiTools.agentState.isEnemyVisible);
                aiTools.shooting.SetShootingEnabled(aiTools.agentState.isEnemyVisible);
                base.OnUpdate();
            }

            protected override Vector3 GetNextDestination()
            {
                return aiTools.terrain.GetBestWaypoint(500.0f).position;
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