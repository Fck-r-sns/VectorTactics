namespace Ai
{
    namespace Fl
    {
        public class FuzzySet
        {
            public delegate FuzzySet Union(params FuzzySet[] sets);
            public delegate FuzzySet Intersection(params FuzzySet[] sets);

            private FuzzyContext.Variable variable;
            private string name;
            private MembershipFunction membershipFunction;

            public FuzzySet(FuzzyContext.Variable variable, string name, MembershipFunction membershipFunction)
            {
                this.variable = variable;
                this.name = name;
                this.membershipFunction = membershipFunction;
            }

            public FuzzyContext.Variable GetVariable()
            {
                return variable;
            }

            public string GetName()
            {
                return name;
            }

            public MembershipFunction GetMembershipFunction()
            {
                return membershipFunction;
            }

            public FuzzyValue CalculateMembership(float crispValue)
            {
                return membershipFunction.Calculate(crispValue);
            }

            public override string ToString()
            {
                return name;
            }
        }
    }
}