using System.Collections.Generic;

namespace Ai
{
    namespace Bt
    {
        public class PrioritySelector : Node
        {
            private List<Node> children = new List<Node>();

            public PrioritySelector(Environment environment) : base(environment)
            {
            }

            public override Result Run()
            {
                foreach (Node child in children)
                {
                    Result childRes = child.Run();
                    if (childRes != Result.Failure)
                    {
                        return childRes;
                    }
                }
                return Result.Success;
            }

            public override void Terminate()
            {

            }

            public override void AddChild(Node child)
            {
                child.AddChild(child);
            }
        }
    }
}