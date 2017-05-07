namespace Ai
{
    namespace Bt
    {
        public class CheckEnvironmentVariable<T> : Node
        {
            private string variable;
            private T value;

            public CheckEnvironmentVariable(Environment environment, string variable, T value) : base(environment)
            {
                this.variable = variable;
                this.value = value;
            }

            public override Result Run()
            {
                if (!environment.ContainsValue(variable))
                {
                    return Result.Failure;
                }
                return environment.GetValue<T>(variable).Equals(value) ? Result.Success : Result.Failure;
            }
        }
    }
}