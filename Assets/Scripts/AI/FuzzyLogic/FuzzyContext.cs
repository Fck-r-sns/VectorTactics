using System.Collections.Generic;

namespace Ai
{
    namespace Fl
    {
        public class FuzzyContext
        {
            public enum Variable
            {
                AgentHealth,
                EnemyHealth,
                EnemyVisibility,
                DistanceToEnemy,
                DistanceWeight,
                InCoverWeight,
                BehindCoverWeight,
                BehindWallWeight,
                HealthPackWeight,
                PlainSightWeight,
                SearchRadius
            }

            public readonly Dictionary<Variable, float> crispInputValues = new Dictionary<Variable, float>();
            public readonly Dictionary<FuzzySet, FuzzyValue> fuzzyInputValues = new Dictionary<FuzzySet, FuzzyValue>();
            public readonly Dictionary<Variable, LinguisticVariable> inputVariables = new Dictionary<Variable, LinguisticVariable>();
            public readonly Dictionary<Variable, LinguisticVariable> outputVariables = new Dictionary<Variable, LinguisticVariable>();
            public readonly Dictionary<Variable, float> crispOutputValues = new Dictionary<Variable, float>();
            public LinguisticValue inferedResult;
        }
    }
}