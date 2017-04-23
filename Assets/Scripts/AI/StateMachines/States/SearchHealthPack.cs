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

            public SearchHealthPack(AiTools aiTools) : 
                base(aiTools)
            {
            }

            public override void OnEnter()
            {
                Debug.Log(Time.time + ": Enter SearchHealthPack state");
            }

            public override void OnUpdate()
            {
                aiTools.shooting.SetAimingEnabled(aiTools.agentState.isEnemyVisible);
                aiTools.shooting.SetShootingEnabled(aiTools.agentState.isEnemyVisible);
                base.OnUpdate();
            }

            protected override Vector3 GetNextDestination()
            {
                return GetRandomNextDestination();
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