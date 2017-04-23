
namespace Ai
{
    namespace Fsm
    {

        public class FiniteStateMachine
        {
            private State currentState;

            public void SetInitialState(State state)
            {
                currentState = state;
                currentState.OnEnter();
            }

            public void Update()
            {
                State nextState = currentState.CheckTransitions();
                if (nextState != null)
                {
                    currentState.OnExit();
                    currentState = nextState;
                    currentState.OnEnter();
                }
                currentState.OnUpdate();
            }
        }

    }
}