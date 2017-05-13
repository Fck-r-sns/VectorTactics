using UnityEngine;

namespace Ai
{
    namespace Nn
    {
        public class SearchHealthStrategy : Strategy
        {
            private const float MOVEMENT_RADIUS = 50.0f;
            private float nextDestinationUpdateTime = 0.0f;

            public SearchHealthStrategy(AiTools aiTools) : base(aiTools)
            {
            }

            public override void OnEnter()
            {
                base.OnEnter();

                aiTools.terrain.SetWeightFunction(TerrainReasoning.RETREAT_WEIGHT_FUNCTION);
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