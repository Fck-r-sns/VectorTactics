using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    namespace Bt
    {
        public class SelectDestinationPoint : SoldierNode
        {
            private string searchRadiusVariable;
            private string minWeightVariable;
            private string resultVariable;

            public SelectDestinationPoint(Environment environment, AiTools aiTools, string searchRadiusVariable, string minWeightVariable, string resultVariable) : base(environment, aiTools)
            {
                this.searchRadiusVariable = searchRadiusVariable;
                this.minWeightVariable = minWeightVariable;
                this.resultVariable = resultVariable;
            }

            public override Result Run()
            {
                if (!environment.ContainsValue(searchRadiusVariable) || !environment.ContainsValue(minWeightVariable))
                {
                    return Result.Failure;
                }
                float searchRadius = environment.GetValue<float>(searchRadiusVariable);
                float minWeight = environment.GetValue<float>(minWeightVariable);
                Vector3 result;
                List<Waypoint> wps;
                if (minWeight < 0)
                {
                    wps = new List<Waypoint>() { aiTools.terrain.GetBestWaypoint(searchRadius) };
                }
                else
                {
                    wps = aiTools.terrain.GetGoodWaypoints(searchRadius, minWeight);
                }
                if (wps.Count == 0)
                {
                    result = new Vector3(
                        aiTools.agentState.position.x + Random.Range(-searchRadius, searchRadius),
                        0.0f,
                        aiTools.agentState.position.z + Random.Range(-searchRadius, searchRadius)
                    );
                }
                else
                {
                    int index = Random.Range(0, wps.Count - 1);
                    result = wps[index].position;
                }
                environment.SetValue(resultVariable, result);
                return Result.Success;
            }
        }
    }
}