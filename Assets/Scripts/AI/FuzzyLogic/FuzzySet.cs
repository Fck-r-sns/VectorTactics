namespace Ai
{
    namespace Fl
    {
        public class FuzzySet
        {
            public delegate FuzzySet Union(params FuzzySet[] sets);
            public delegate FuzzySet Intersection(params FuzzySet[] sets);

            public readonly MembershipFunction membershipFunction;

            public FuzzySet(MembershipFunction membershipFunction)
            {
                this.membershipFunction = membershipFunction;
            }
        }
    }
}