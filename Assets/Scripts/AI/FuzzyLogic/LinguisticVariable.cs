using System.Collections.Generic;

namespace Ai
{
    namespace Fl
    {
        public class LinguisticVariable
        {
            private List<FuzzySet> sets = new List<FuzzySet>();

            public void AddSet(FuzzySet set)
            {
                sets.Add(set);
            }

            public LinguisticValue Calculate(float crispValue)
            {
                LinguisticValue result = new LinguisticValue();
                foreach (FuzzySet set in sets)
                {
                    FuzzyValue membership = set.membershipFunction(crispValue);
                    if (membership.value > 0.0f)
                    {
                        result.AddSet(new KeyValuePair<FuzzySet, FuzzyValue>(set, membership));
                    }
                }
                return result;
            }
        }
    }
}