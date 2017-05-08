using System.Collections.Generic;

namespace Ai
{
    namespace Fl
    {
        public class FuzzyInference
        {
            private List<FuzzyRule> rules = new List<FuzzyRule>();
            private FuzzyCalculator calc;

            public FuzzyInference(FuzzyCalculator calc)
            {
                this.calc = calc;
            }

            public void AddRule(FuzzyRule rule)
            {
                rules.Add(rule);
            }

            public LinguisticValue Infer(params LinguisticValue[] vars)
            {
                List<LinguisticValue> rulesResults = new List<LinguisticValue>();
                foreach (FuzzyRule rule in rules)
                {
                    rulesResults.Add(rule(calc, vars));
                }

                Dictionary<FuzzySet, List<FuzzyValue>> valuesBySet = new Dictionary<FuzzySet, List<FuzzyValue>>();
                foreach (LinguisticValue ruleResult in rulesResults)
                {
                    foreach (KeyValuePair<FuzzySet, FuzzyValue> kv in ruleResult.GetSets())
                    {
                        if (!valuesBySet.ContainsKey(kv.Key))
                        {
                            valuesBySet.Add(kv.Key, new List<FuzzyValue>());
                        }
                        valuesBySet[kv.Key].Add(kv.Value);
                    }
                }

                LinguisticValue result = new LinguisticValue();
                foreach (KeyValuePair<FuzzySet, List<FuzzyValue>> kv in valuesBySet)
                {
                    FuzzyValue totalMembership = calc.or(kv.Value.ToArray());
                    result.AddSet(new KeyValuePair<FuzzySet, FuzzyValue>(kv.Key, totalMembership));
                }
                return result;
            }
        }
    }
}