namespace Ai
{
    namespace Bt
    {
        public abstract class Node
        {
            protected Environment environment;

            public Node(Environment environment)
            {
                this.environment = environment;
            }

            public abstract Result Run();

            public virtual void Terminate()
            {
                
            }

            // returns this for chain operations
            public Node AddChild(Node child)
            {
                AddChild_impl(child);
                return this;
            }

            protected virtual void AddChild_impl(Node child)
            {
                throw new System.Exception("Can't add child - node is terminal");
            }
        }
    }
}