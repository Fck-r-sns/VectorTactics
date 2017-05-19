﻿using System;
using UnityEngine;

namespace Ai
{
    namespace Nn
    {
        public static class NeuralDefines
        {
            public static bool USE_HIDDEN_LAYER = false;
            public static int HIDDEN_LAYER_SIZE = 5;
            public static float SIGMOID_STEEP_COEF = 1.2f;
            public static float LEARNING_SPEED_COEF = 0.2f;
            public static int REPEAT_LEARNING_TIMES = 201;

            public readonly static Func<float, float> SIGMOID_FUNCTION = x => 1 / (1 + Mathf.Exp(-SIGMOID_STEEP_COEF * x));
        }
    }
}