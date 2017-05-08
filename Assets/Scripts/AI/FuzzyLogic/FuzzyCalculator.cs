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
            public readonly DefuzzificationFunction defuzzificationFunc;

            public FuzzyCalculator(FuzzyValue.And and, FuzzyValue.Or or, FuzzyValue.Not not, FuzzySet.Union union, FuzzySet.Intersection intersection, DefuzzificationFunction defuzzificationFunc)
            {
                this.and = and;
                this.or = or;
                this.not = not;
                this.union = union;
                this.intersection = intersection;
                this.defuzzificationFunc = defuzzificationFunc;
            }
        }
    }
}