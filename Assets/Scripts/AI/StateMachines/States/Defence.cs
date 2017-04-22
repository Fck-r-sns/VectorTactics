using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    namespace Fsm
    {
        public class Defence : SoldierState
        {
            public Defence(AiTools aiTools) : 
                base(aiTools)
            {
            }

            public override void OnEnter()
            {
                Debug.Log(Time.time + ": Enter Defence state");
            }
        }
    }
}