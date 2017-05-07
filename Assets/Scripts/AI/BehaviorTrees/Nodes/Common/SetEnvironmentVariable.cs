namespace Ai
{
    namespace Bt
    {
        public class SetEnvironmentVariable<T> : Node
        {
            private string variable;
            private T value;

            public SetEnvironmentVariable(Environment environment, string variable, T value) : base(environment)
            {
                this.variable = variable;
                this.value = value;
            }

            public override Result Run()
            {
                environment.SetValue(variable, value);
                return Result.Success;
            }
        }
    }
}