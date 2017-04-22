using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    namespace Fsm
    {
        public class Attack : SoldierState
        {
            public Attack(AiTools aiTools) : 
                base(aiTools)
            {
            }

            public override void OnEnter()
            {
                Debug.Log(Time.time + ": Enter Attack state");
                aiTools.shooting.SetAimingEnabled(true);
                aiTools.shooting.SetShootingEnabled(true);
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