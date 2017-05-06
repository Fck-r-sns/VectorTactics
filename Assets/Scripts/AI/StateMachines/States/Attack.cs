using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    namespace Fsm
    {
        public class Attack : SoldierState
        {

            private const float MOVEMENT_RADIUS = 5.0f;
            private const float MIN_ATTACK_RADIUS = 10.0f;
            private const float MAX_ATTACK_RADIUS = 15.0f;

            public Attack(AiTools aiTools) :
                base(aiTools)
            {
            }

            public override void OnEnter()
            {
                base.OnEnter();
                Debug.Log(Time.time + ": Enter Attack state");
                aiTools.shooting.SetAimingEnabled(true);
                aiTools.shooting.SetShootingEnabled(true);

                aiTools.terrain.SetWeightFunction(wp =>
                {
                    float weight = 0.0f;
                    if (!wp.isBehindWall)
                    {
                        weight += 0.5f;
                    }
                    if (wp.isInCover)
                    {
                        weight += 0.2f;
                    }
                    if (MIN_ATTACK_RADIUS <= wp.directDistanceToEnemy && wp.directDistanceToEnemy <= MAX_ATTACK_RADIUS)
                    {
                        float center = (MIN_ATTACK_RADIUS + MAX_ATTACK_RADIUS) / 2.0f;
                        if (wp.directDistanceToEnemy < center)
                        {
                            weight += 0.3f * (wp.directDistanceToEnemy - MIN_ATTACK_RADIUS) / (center - MIN_ATTACK_RADIUS);
                        }
                        else
                        {
                            weight += 0.3f * (MAX_ATTACK_RADIUS - wp.directDistanceToEnemy) / (MAX_ATTACK_RADIUS - center);
                        }
                    }
                    return weight;
                });
            }

            public override void OnExit()
            {

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