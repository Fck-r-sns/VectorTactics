using System.Collections.Generic;

namespace Ai
{
    namespace Bt
    {
        public class Environment
        {
            private Dictionary<string, object> variables;

            public void SetValue(string variable, object value)
            {
                variables[variable] = value;
            }

            public T GetValue<T>(string variable, T defaultValue = default(T))
            {
                if (variables.ContainsKey(variable))
                {
                    return (T)variables[variable];
                }
                return defaultValue;
            }

            public bool ContainsValue(string variable)
            {
                return variables.ContainsKey(variable);
            }
        }
    }
}