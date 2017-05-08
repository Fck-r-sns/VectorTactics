using System.Collections.Generic;

namespace Ai
{
    namespace Fl
    {
        public class LinguisticValue
        {
            private List<KeyValuePair<FuzzySet, FuzzyValue>> sets = new List<KeyValuePair<FuzzySet, FuzzyValue>>();

            public void AddSet(KeyValuePair<FuzzySet, FuzzyValue> set)
            {
                sets.Add(set);
            }

            public List<KeyValuePair<FuzzySet, FuzzyValue>> GetSets()
            {
                return sets;
            }
        }
    }
}