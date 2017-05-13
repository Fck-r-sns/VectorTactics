namespace Ai
{
    namespace Nn
    {
        public class ConstantValueNeuron : Neuron
        {
            private readonly float value;

            public ConstantValueNeuron(float value)
            {
                this.value = value;
            }

            public override float GetOutput()
            {
                return value;
            }
        }
    }
}