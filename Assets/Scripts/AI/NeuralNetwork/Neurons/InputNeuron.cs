using System;

namespace Ai
{
    namespace Nn
    {
        public class InputNeuron : Neuron
        {
            private Func<float> outputProvider;

            public InputNeuron(Func<float> outputProvider)
            {
                this.outputProvider = outputProvider;
            }

            public override float GetOutput()
            {
                return outputProvider();
            }
        }
    }
}