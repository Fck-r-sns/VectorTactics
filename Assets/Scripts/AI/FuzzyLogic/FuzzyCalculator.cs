using UnityEngine;

namespace Ai
{
    namespace Fl
    {
        public class FuzzyCalculator
        {
            public readonly FuzzyValue.And and;
            public readonly FuzzyValue.Or or;
            public readonly FuzzyValue.Not not;
            public readonly FuzzySet.Union union;
            public readonly FuzzySet.Intersection intersection;

            public FuzzyCalculator()
            {
                and = vars =>
                {
                    float res = 1.0f;
                    foreach (FuzzyValue var in vars)
                    {
                        res = Mathf.Min(res, var.value);
                    }
                    return res;
                };

                or = vars =>
                {
                    float res = 0.0f;
                    foreach (FuzzyValue var in vars)
                    {
                        res = Mathf.Max(res, var.value);
                    }
                    return res;
                };

                not = var => 1 - var.value;
            }

            public FuzzyCalculator(FuzzyValue.And and, FuzzyValue.Or or, FuzzyValue.Not not, FuzzySet.Union union, FuzzySet.Intersection intersection)
            {
                this.and = and;
                this.or = or;
                this.not = not;
                this.union = union;
                this.intersection = intersection;
            }
        }
    }
}