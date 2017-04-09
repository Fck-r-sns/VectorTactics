
namespace Ai
{
    namespace Fsm
    {

        public class FiniteStateMachine
        {
            private State currentState;
            private WorldState world;

            public FiniteStateMachine(WorldState world)
            {
                this.world = world;
            }

            public void SetInitialState(State state)
            {
                currentState = state;
            }

            public void Update()
            {
                State nextState = currentState.CheckTransitions(world);
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