namespace Ai
{
    namespace Bt
    {
        public abstract class Node
        {
            private Environment environment;

            public Node(Environment environment)
            {
                this.environment = environment;
            }

            public abstract Result Run();
            public abstract void Terminate();
            public abstract void AddChild(Node child); 
        }
    }
}