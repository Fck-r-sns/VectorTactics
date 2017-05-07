using System.Collections.Generic;

namespace Ai
{
    namespace Bt
    {
        public class Sequence : Node
        {
            private List<Node> children = new List<Node>();

            public Sequence(Environment environment) : base(environment)
            {
            }

            public override Result Run()
            {
                foreach (Node child in children)
                {
                    Result childRes = child.Run();
                    if (childRes != Result.Success)
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