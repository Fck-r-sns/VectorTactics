﻿
namespace Ai
{
    namespace Fsm
    {

        public class Transition
        {

            public delegate bool ConditionChecker();
            public readonly ConditionChecker condition;
            public readonly State destination;

            public Transition(ConditionChecker condition, State destination)
            {
                this.condition = condition;
                this.destination = destination;
            }
            
        }

    }
}