using UnityEngine;

namespace Ai
{
    namespace Fl
    {
        public class Singleton : MembershipFunction
        {
            private float value;

            public Singleton(float value)
            {
                this.value = value;
            }

            public FuzzyValue Calculate(float crispValue)
            {
                return Mathf.Approximately(value, crispValue) ? 1.0f : 0.0f;
            }

            public float GetMaxCrispValue()
            {
                return value;
            }

            public float GetMinCrispValue()
            {
                return value;
            }
        }
    }
}