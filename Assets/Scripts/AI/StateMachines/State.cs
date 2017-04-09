
using System.Collections.Generic;

namespace Ai
{
    namespace Fsm
    {

        public abstract class State
        {
            private List<Transition> transitions = new List<Transition>();

            public void AddTransition(Transition transition)
            {
                transitions.Add(transition);
            }

            public virtual State CheckTransitions(WorldState world)
            {
                foreach (Transition t in transitions)
                {
                    if (t.condition(world))
                    {
                        return t.destination;
                    }
                }
                return null;
            }

            public virtual void OnEnter()
            {
                // do nothing by default
            }

            public virtual void OnUpdate()
            {
                // do nothing by default
            }

            public virtual void OnExit()
            {
                // do nothing by default
            }
        }

    }
}