using System.Collections.Generic;

namespace Ai
{
    namespace Fl
    {
        public class FuzzyRule
        {
            List<FuzzySet> inputVariables = new List<FuzzySet>();
            List<FuzzySet> outputVariables = new List<FuzzySet>();

            public FuzzyRule In(FuzzySet set)
            {
                inputVariables.Add(set);
                return this;
            }

            public FuzzyRule Out(FuzzySet set)
            {
                outputVariables.Add(set);
                return this;
            }

            public LinguisticValue Infer(FuzzyCalculator calc, FuzzyContext context)
            {
                List<FuzzyValue> inputValues = new List<FuzzyValue>(inputVariables.Count);
                foreach (FuzzySet set in inputVariables)
                {
                    inputValues.Add(context.fuzzyInputValues[set]);
                }
                FuzzyValue membership = calc.and(inputValues.ToArray());

                LinguisticValue result = new LinguisticValue();
                foreach (FuzzySet set in outputVariables)
                {
                    result.AddSet(set, membership);
                }
                return result;
            }
        }
    }
}