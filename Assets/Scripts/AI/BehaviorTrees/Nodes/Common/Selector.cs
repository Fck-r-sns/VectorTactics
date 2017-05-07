using System.Collections.Generic;

namespace Ai
{
    namespace Bt
    {
        public class Selector : Node
        {
            private List<Node> children = new List<Node>();

            public Selector(Environment environment) : base(environment)
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

            protected override void AddChild_impl(Node child)
            {
                child.AddChild(child);
            }
        }
    }
}