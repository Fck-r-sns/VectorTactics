namespace Ai
{
    namespace Bt
    {
        public class ClearEnvironmentVariable : Node
        {
            private string variable;

            public ClearEnvironmentVariable(Environment environment, string variable) : base(environment)
            {
                this.variable = variable;
            }

            public override Result Run()
            {
                environment.RemoveVariable(variable);
                return Result.Success;
            }
        }
    }
}