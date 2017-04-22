using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{

    public class AiTools
    {
        public readonly Navigation navigation;
        public readonly Shooting shooting;
        public readonly TerrainReasoning terrain;
        public readonly SoldierController controller;
        public readonly CharacterState agentState;
        public readonly CharacterState enemyState;

        public AiTools(Navigation navigation, Shooting shooting, TerrainReasoning terrain, SoldierController controller, CharacterState agentState, CharacterState enemyState)
        {
            this.navigation = navigation;
            this.shooting = shooting;
            this.terrain = terrain;
            this.controller = controller;
            this.agentState = agentState;
            this.enemyState = enemyState;
        }
    }

}