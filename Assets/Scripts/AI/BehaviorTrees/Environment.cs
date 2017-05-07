using System.Collections.Generic;

namespace Ai
{
    namespace Bt
    {
        public class Environment
        {
            private Dictionary<string, object> variables = new Dictionary<string, object>();

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

            public void RemoveVariable(string variable)
            {
                variables.Remove(variable);
            }

            public bool ContainsValue(string variable)
            {
                return variables.ContainsKey(variable);
            }
        }
    }
}