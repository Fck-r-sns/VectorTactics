namespace Ai
{
    namespace Fl
    {
        public interface MembershipFunction
        {
            FuzzyValue Calculate(float crispValue);
        }
    }
}