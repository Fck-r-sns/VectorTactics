using System;

namespace Ai
{
    namespace Nn
    {
        public class OutputNeuron : LearnableNeuron
        {
            private float[] expectedOutputs;

            public OutputNeuron(Func<float, float> activationFunc) : base(activationFunc)
            {
            }

            public override float GetError()
            {
                float actual = GetOutput();
                return actual * (1 - actual) * (expectedOutputs[0] - actual);
            }

            public override void SetExpectedOutputs(params float[] expectedOutputs)
            {
                this.expectedOutputs = expectedOutputs;
            }
        }
    }
}