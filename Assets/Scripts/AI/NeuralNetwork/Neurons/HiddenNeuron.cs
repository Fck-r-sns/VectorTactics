using System;

namespace Ai
{
    namespace Nn
    {
        public class HiddenNeuron : LearnableNeuron
        {
            public HiddenNeuron(Func<float, float> activationFunc) : base(activationFunc)
            {
            }

            public override float GetError()
            {
                float actualOutput = GetOutput();
                float weightedOutputsErrorsSum = 0.0f;
                foreach (Neuron output in outputs)
                {
                    weightedOutputsErrorsSum += output.GetWeight(this) * output.GetError();
                }
                return actualOutput * (1 - actualOutput) * weightedOutputsErrorsSum;
            }
        }
    }
}