namespace Ai
{
    namespace Fl
    {
        public class FuzzySet
        {
            public delegate FuzzySet Union(params FuzzySet[] sets);
            public delegate FuzzySet Intersection(params FuzzySet[] sets);

            private MembershipFunction membershipFunction;

            public FuzzySet(MembershipFunction membershipFunction)
            {
                this.membershipFunction = membershipFunction;
            }

            public FuzzyValue CalculateMembership(float crispValue)
            {
                return membershipFunction.Calculate(crispValue);
            }
        }
    }
}