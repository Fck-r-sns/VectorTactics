﻿namespace Ai
{
    namespace Bt
    {
        public class Inverter : Node
        {
            private Node child;

            public Inverter(Environment environment) : base(environment)
            {
            }

            public override Result Run()
            {
                Result res = child.Run();
                switch (res)
                {
                    case Result.Success:
                        return Result.Failure;
                    case Result.Failure:
                        return Result.Success;
                    default:
                        return res;
                }
            }

            public override void AddChild(Node child)
            {
                this.child = child;
            }
        }
    }
}