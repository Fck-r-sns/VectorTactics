using System;
using System.Collections.Generic;

namespace Ai
{
    namespace Nn
    {
        public abstract class LearnableNeuron : Neuron
        {
            protected List<Neuron> inputs = new List<Neuron>();
            protected List<Neuron> outputs = new List<Neuron>();
            protected Dictionary<Neuron, float> weights = new Dictionary<Neuron, float>();
            protected Func<float, float> activationFunc;
            private float output;

            public LearnableNeuron(Func<float, float> activationFunc)
            {
                this.activationFunc = activationFunc;
                // add bias input
                inputs.Add(new ConstantValueNeuron(1.0f));
                weights.Add(inputs[0], 0.0f);
            }

            public override void AddInput(Neuron input)
            {
                inputs.Add(input);
                weights.Add(input, 1.0f);
                input.AddOutput(this);
            }

            public override void AddOutput(Neuron output)
            {
                outputs.Add(output);
            }

            public override float GetWeight(Neuron input)
            {
                return weights[input];
            }

            public override void FeedForward()
            {
                float sum = 0.0f;
                foreach (Neuron input in inputs)
                {
                    sum += input.GetOutput() * weights[input];
                }
                output = activationFunc(sum);
            }

            public override void BackPropagate()
            {
                float error = GetError();
                foreach (Neuron input in inputs)
                {
                    weights[input] = weights[input] + NeuralDefines.LEARNING_SPEED_COEF * error * input.GetOutput();
                }
            }

            public override float GetOutput()
            {
                return output;
            }
        }
    }
}