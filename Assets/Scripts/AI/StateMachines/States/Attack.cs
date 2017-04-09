using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    namespace Fsm
    {
        public class Attack : SoldierState
        {
            public Attack(WorldState world, TerrainReasoning terrain, Navigation navigation, Shooting shooting, SoldierController controller) : 
                base(world, terrain, navigation, shooting, controller)
            {
            }

            public override void OnEnter()
            {
                Debug.Log(Time.time + ": Enter Attack state");
                shooting.SetAimingEnabled(true);
                shooting.SetShootingEnabled(true);
            }

            public override void OnUpdate()
            {
                
            }

            public override void OnExit()
            {
                
            }

        }
    }
}