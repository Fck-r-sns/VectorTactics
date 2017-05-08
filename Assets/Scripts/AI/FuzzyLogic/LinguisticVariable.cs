using System.Collections.Generic;

namespace Ai
{
    namespace Fl
    {
        public class LinguisticVariable
        {
            private List<FuzzySet> sets = new List<FuzzySet>();

            public void AddSets(params FuzzySet[] sets)
            {
                this.sets.AddRange(sets);
            }

            public LinguisticValue Calculate(float crispValue)
            {
                LinguisticValue result = new LinguisticValue();
                foreach (FuzzySet set in sets)
                {
                    FuzzyValue membership = set.CalculateMembership(crispValue);
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