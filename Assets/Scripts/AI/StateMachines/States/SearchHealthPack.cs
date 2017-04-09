using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    namespace Fsm
    {

        public class SearchHealthPack : SoldierState
        {
            public SearchHealthPack(WorldState world, TerrainReasoning terrain, Navigation navigation, SoldierController controller) : 
                base(world, terrain, navigation, controller)
            {
            }

            public override void OnEnter()
            {
                Debug.Log(Time.time + ": Enter SearchHealthPack state");
            }
        }

    }
}