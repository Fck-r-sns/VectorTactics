using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    namespace Fsm
    {
        public class Attack : SoldierState
        {
            public Attack(WorldState world, TerrainReasoning terrain, Navigation navigation, SoldierController controller) : 
                base(world, terrain, navigation, controller)
            {
            }

            public override void OnEnter()
            {
                Debug.Log("Enter Attack state");
            }

        }
    }
}