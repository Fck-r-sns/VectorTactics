using System;

namespace Ai
{
    namespace Nn
    {
        public class OutputNeuron : LearnableNeuron
        {
            private float expectedOutput;

            public OutputNeuron(Func<float, float> activationFunc) : base(activationFunc)
            {
            }

            public override float GetError()
            {
                float actual = GetOutput();
                return actual * (1 - actual) * (expectedOutput - actual);
            }

            public override void SetExpectedOutput(float expectedOutput)
            {
                this.expectedOutput = expectedOutput;
            }
        }
    }
}