using System;

namespace Ai
{
    namespace Nn
    {
        public abstract class Neuron
        {
            public virtual void AddInput(Neuron input)
            {
                throw new Exception("Not implemented");
            }

            public virtual void AddOutput(Neuron output)
            {
                throw new Exception("Not implemented");
            }

            public virtual float GetWeight(Neuron input)
            {
                throw new Exception("Not implemented");
            }

            public virtual float GetOutput()
            {
                throw new Exception("Not implemented");
            }

            public virtual float GetError()
            {
                throw new Exception("Not implemented");
            }

            public virtual void FeedForward()
            {
                throw new Exception("Not implemented");
            }

            public virtual void BackPropagate()
            {
                throw new Exception("Not implemented");
            }

            public virtual void SetExpectedOutput(float expectedOutput)
            {
                throw new Exception("Not implemented");
            }
        }
    }
}