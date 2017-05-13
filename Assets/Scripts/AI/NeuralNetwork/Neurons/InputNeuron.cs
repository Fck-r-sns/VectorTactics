using System;
using System.Collections.Generic;

namespace Ai
{
    namespace Nn
    {
        public class InputNeuron : Neuron
        {
            private Func<float> outputProvider;
            private List<Neuron> outputs = new List<Neuron>();

            public InputNeuron(Func<float> outputProvider)
            {
                this.outputProvider = outputProvider;
            }

            public override void AddOutput(Neuron output)
            {
                outputs.Add(output);
            }

            public override float GetOutput()
            {
                return outputProvider();
            }
        }
    }
}