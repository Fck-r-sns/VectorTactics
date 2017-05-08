namespace Ai
{
    namespace Fl
    {
        public class FuzzyValue
        {
            public delegate FuzzyValue And(params FuzzyValue[] vars);
            public delegate FuzzyValue Or(params FuzzyValue[] vars);
            public delegate FuzzyValue Not(FuzzyValue v);

            public readonly float value;

            public FuzzyValue(float value)
            {
                this.value = value;
            }

            public static implicit operator FuzzyValue(float value)
            {
                return new FuzzyValue(value);
            }
        }
    }
}