using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    namespace Fsm
    {

        public class SearchEnemy : SoldierState
        {

            private Vector3? destination;

            public SearchEnemy(AiTools aiTools) :
                base(aiTools)
            {

            }

            public override void OnEnter()
            {
                Debug.Log(Time.time + ": Enter SearchEnemy state");
                aiTools.shooting.SetAimingEnabled(false);
                aiTools.shooting.SetShootingEnabled(false);
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
                    || (destination.HasValue && Vector3.Distance(aiTools.controller.transform.position, destination.Value) < 2.0f)
                    )
                {
                    destination = null;
                }
            }

            public override void OnExit()
            {
                destination = null;
                aiTools.navigation.SetDestination(null);
            }

            private Vector3 GetNextDestination()
            {
                return new Vector3(
                    Random.Range(-Defines.MAP_WIDTH / 2.0f, Defines.MAP_WIDTH / 2.0f),
                    0.0f,
                    Random.Range(-Defines.MAP_HEIGHT / 2.0f, Defines.MAP_HEIGHT / 2.0f)
                    );
            }
        }

    }
}