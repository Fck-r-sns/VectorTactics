using System.Collections.Generic;

namespace Ai
{
    namespace Fl
    {
        public class FuzzyInference
        {
            private List<FuzzyRule> rules;
            private FuzzyCalculator calc;
            private FuzzyContext context;

            public FuzzyInference(FuzzyCalculator calc, FuzzyContext context)
            {
                this.rules = new List<FuzzyRule>();
                this.calc = calc;
                this.context = context;
            }

            public void AddRule(FuzzyRule rule)
            {
                rules.Add(rule);
            }

            public LinguisticValue Infer()
            {
                List<LinguisticValue> rulesResults = new List<LinguisticValue>();
                foreach (FuzzyRule rule in rules)
                {
                    rulesResults.Add(rule.Infer(calc, context));
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
                    result.AddSet(kv.Key, totalMembership);
                }
                return result;
            }
        }
    }
}