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

            public virtual void AddChild(Node child)
            {
                throw new System.Exception("Can't add child - node is terminal");
            }
        }
    }
}