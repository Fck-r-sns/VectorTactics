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

            public SearchEnemy(WorldState world, TerrainReasoning terrain, Navigation navigation, SoldierController controller) :
                base(world, terrain, navigation, controller)
            {
            }

            public override void OnEnter()
            {
                Debug.Log("Enter SearchEnemy state");
            }

            public override void OnUpdate()
            {
                destination = GetNextDestination();
                navigation.SetDestination(destination);
                if (!navigation.IsDestinationReachable())
                {
                    destination = null;
                }
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