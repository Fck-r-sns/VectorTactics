
namespace Ai
{

    namespace Fsm
    {

        public abstract class SoldierState : State
        {
            protected readonly AiTools aiTools;

            public SoldierState(AiTools aiTools)
            {
                this.aiTools = aiTools;
            }
        }

    }
}