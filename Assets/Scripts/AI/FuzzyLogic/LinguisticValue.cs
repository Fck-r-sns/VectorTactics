using System.Collections.Generic;

namespace Ai
{
    namespace Fl
    {
        public class LinguisticValue
        {
            private Dictionary<FuzzySet, FuzzyValue> sets = new Dictionary<FuzzySet, FuzzyValue>();

            public void AddSet(FuzzySet set, FuzzyValue membership)
            {
                sets.Add(set, membership);
            }

            public Dictionary<FuzzySet, FuzzyValue> GetSets()
            {
                return sets;
            }

            public FuzzyValue GetMembership(FuzzySet set)
            {
                if (sets.ContainsKey(set))
                {
                    return sets[set];
                }
                return 0.0f;
            }
        }
    }
}