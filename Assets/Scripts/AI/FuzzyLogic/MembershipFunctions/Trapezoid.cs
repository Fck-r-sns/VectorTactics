namespace Ai
{
    namespace Fl
    {
        public class Trapezoid : MembershipFunction
        {
            private float a;
            private float b;
            private float c;
            private float d;

            public Trapezoid(float a, float b, float c, float d)
            {
                this.a = a;
                this.b = b;
                this.c = c;
                this.d = d;
            }

            public FuzzyValue Calculate(float crispValue)
            {
                if (a < crispValue && crispValue < b)
                {
                    return (crispValue - a) / (b - a);
                }
                if (b <= crispValue && crispValue <= c)
                {
                    return 1.0f;
                }
                if (c < crispValue && crispValue < d)
                {
                    return (crispValue - c) / (d - c);
                }
                return 0.0f;
            }

            public float GetMaxCrispValue()
            {
                return d;
            }

            public float GetMinCrispValue()
            {
                return a;
            }
        }
    }
}