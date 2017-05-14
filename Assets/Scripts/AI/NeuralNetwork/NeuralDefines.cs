using System;
using UnityEngine;

namespace Ai
{
    namespace Nn
    {
        public static class NeuralDefines
        {
            public const bool USE_HIDDEN_LAYER = false;
            public const float HIDDEN_LAYER_SIZE = 5.0f;
            public const float SIGMOID_STEEP_COEF = 1.2f;
            public const float LEARNING_SPEED_COEF = 0.2f;

            public readonly static Func<float, float> SIGMOID_FUNCTION = x => 1 / (1 + Mathf.Exp(-SIGMOID_STEEP_COEF * x));
        }
    }
}