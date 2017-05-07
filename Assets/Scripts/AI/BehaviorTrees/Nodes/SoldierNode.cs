namespace Ai
{
    namespace Bt
    {
        public abstract class SoldierNode : Node
        {
            protected AiTools aiTools;

            public SoldierNode(Environment environment, AiTools aiTools) : base(environment)
            {
                this.aiTools = aiTools;
            }
        }
    }
}