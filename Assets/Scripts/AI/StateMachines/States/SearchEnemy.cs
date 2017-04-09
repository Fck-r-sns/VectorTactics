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
            private GameObject target;

            public SearchEnemy(WorldState world, TerrainReasoning terrain, Navigation navigation, Shooting shooting, SoldierController controller) :
                base(world, terrain, navigation, shooting, controller)
            {
                target = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                target.GetComponent<Collider>().enabled = false;
            }

            public override void OnEnter()
            {
                Debug.Log(Time.time + ": Enter SearchEnemy state");
                shooting.SetAimingEnabled(false);
                shooting.SetShootingEnabled(false);
            }

            public override void OnUpdate()
            {
                if (!destination.HasValue)
                {
                    destination = GetNextDestination();
                    navigation.SetDestination(destination);
                }

                if (
                    !navigation.IsDestinationReachable() 
                    || (destination.HasValue && Vector3.Distance(controller.transform.position, destination.Value) < 2.0f)
                    )
                {
                    destination = null;
                }

                if (destination.HasValue)
                {
                    target.SetActive(true);
                    target.transform.position = destination.Value;
                }
                else
                {
                    target.SetActive(false);
                }
            }

            public override void OnExit()
            {
                destination = null;
                navigation.SetDestination(null);
                target.SetActive(false);
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