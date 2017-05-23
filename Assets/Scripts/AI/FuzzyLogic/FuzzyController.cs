using System;
using System.Collections.Generic;
using UnityEngine;

using EventBus;

namespace Ai
{
    namespace Fl
    {
        [RequireComponent(typeof(AiTools))]
        [RequireComponent(typeof(SoldierController))]
        public class FuzzyController : MonoBehaviour
        {
            private AiTools aiTools;
            private SoldierController controller;

            private FuzzyCalculator calculator;
            private FuzzyContext context;
            private FuzzyInference inference;

            private bool enemyVisibilityOnLastTurn = false;
            private Vector3? destination;

            void Start()
            {
                aiTools = GetComponent<AiTools>();
                controller = GetComponent<SoldierController>();
                aiTools.Init();
                aiTools.terrain.SetWeightsNormalizationEnabled(true);
                Init();

                Dispatcher.SendEvent(new ControllerInited(aiTools.agentState.side, GameDefines.ControllerType.FuzzyLogic));
            }

            void Update()
            {
                if (controller.GetState().isDead)
                {
                    aiTools.shooting.SetAimingEnabled(false);
                    aiTools.shooting.SetShootingEnabled(false);
                    aiTools.navigation.SetDestination(null);
                    destination = null;
                    return;
                }
                float time = Time.realtimeSinceStartup;
                UpdateCrispValues();
                Fuzzify();
                Infer();
                Defuzzify();
                ApplyResultValues();
                DoStuff();
                Dispatcher.SendEvent(new NewFrame(aiTools.agentState.side, Time.realtimeSinceStartup - time));
            }

            private void Init()
            {
                calculator = new FuzzyCalculator();
                context = new FuzzyContext();
                inference = new FuzzyInference(calculator, context);

                LinguisticVariable agentHealth = new LinguisticVariable();
                FuzzySet lowAgentHealth = new FuzzySet(FuzzyContext.Variable.AgentHealth, "lowAgentHealth", new Trapezoid(0.0f, 0.0f, 30.0f, 60.0f));
                FuzzySet mediumAgentHealth = new FuzzySet(FuzzyContext.Variable.AgentHealth, "mediumAgentHealth", new Trapezoid(30.0f, 60.0f, 60.0f, 80.0f));
                FuzzySet highAgentHealth = new FuzzySet(FuzzyContext.Variable.AgentHealth, "highAgentHealth", new Trapezoid(60.0f, 80.0f, 100.0f, 100.0f));
                agentHealth.AddSets(lowAgentHealth, mediumAgentHealth, highAgentHealth);

                LinguisticVariable enemyHealth = new LinguisticVariable();
                FuzzySet lowEnemyHealth = new FuzzySet(FuzzyContext.Variable.EnemyHealth, "lowEnemyHealth", new Trapezoid(0.0f, 0.0f, 30.0f, 60.0f));
                FuzzySet mediumEnemyHealth = new FuzzySet(FuzzyContext.Variable.EnemyHealth, "mediumEnemyHealth", new Trapezoid(30.0f, 60.0f, 60.0f, 80.0f));
                FuzzySet highEnemyHealth = new FuzzySet(FuzzyContext.Variable.EnemyHealth, "highEnemyHealth", new Trapezoid(60.0f, 80.0f, 100.0f, 100.0f));
                enemyHealth.AddSets(lowEnemyHealth, mediumEnemyHealth, highEnemyHealth);

                LinguisticVariable enemyVisibility = new LinguisticVariable();
                FuzzySet enemyIsVisible = new FuzzySet(FuzzyContext.Variable.EnemyVisibility, "enemyIsVisible", new Singleton(1.0f));
                FuzzySet enemyIsNotVisible = new FuzzySet(FuzzyContext.Variable.EnemyVisibility, "enemyIsNotVisible", new Singleton(0.0f));
                enemyVisibility.AddSets(enemyIsVisible, enemyIsNotVisible);

                LinguisticVariable distanceToEnemy = new LinguisticVariable();
                FuzzySet closeDistance = new FuzzySet(FuzzyContext.Variable.DistanceToEnemy, "closeDistance", new Trapezoid(0.0f, 0.0f, 7.0f, 10.0f));
                FuzzySet mediumDistance = new FuzzySet(FuzzyContext.Variable.DistanceToEnemy, "mediumDistance", new Trapezoid(7.0f, 10.0f, 14.0f, 18.0f));
                FuzzySet farDistance = new FuzzySet(FuzzyContext.Variable.DistanceToEnemy, "farDistance", new Trapezoid(14.0f, 18.0f, 100.0f, 100.0f));
                distanceToEnemy.AddSets(closeDistance, mediumDistance, farDistance);

                LinguisticVariable distanceWeight = new LinguisticVariable();
                FuzzySet lowDistanceWeight = new FuzzySet(FuzzyContext.Variable.DistanceWeight, "lowDistanceWeight", new Trapezoid(0.0f, 0.0f, 0.2f, 0.4f));
                FuzzySet mediumDistanceWeight = new FuzzySet(FuzzyContext.Variable.DistanceWeight, "mediumDistanceWeight", new Trapezoid(0.2f, 0.4f, 0.6f, 0.8f));
                FuzzySet highDistanceWeight = new FuzzySet(FuzzyContext.Variable.DistanceWeight, "highDistanceWeight", new Trapezoid(0.6f, 0.8f, 1.0f, 1.0f));
                distanceWeight.AddSets(lowDistanceWeight, mediumDistanceWeight, highDistanceWeight);

                LinguisticVariable inCoverWeight = new LinguisticVariable();
                FuzzySet lowInCoverWeight = new FuzzySet(FuzzyContext.Variable.InCoverWeight, "lowInCoverWeight", new Trapezoid(0.0f, 0.0f, 0.2f, 0.4f));
                FuzzySet mediumInCoverWeight = new FuzzySet(FuzzyContext.Variable.InCoverWeight, "mediumInCoverWeight", new Trapezoid(0.2f, 0.4f, 0.6f, 0.8f));
                FuzzySet highInCoverWeight = new FuzzySet(FuzzyContext.Variable.InCoverWeight, "highInCoverWeight", new Trapezoid(0.6f, 0.8f, 1.0f, 1.0f));
                inCoverWeight.AddSets(lowInCoverWeight, mediumInCoverWeight, highInCoverWeight);

                LinguisticVariable behindCoverWeight = new LinguisticVariable();
                FuzzySet lowBehindCoverWeight = new FuzzySet(FuzzyContext.Variable.BehindCoverWeight, "lowBehindCoverWeight", new Trapezoid(0.0f, 0.0f, 0.2f, 0.4f));
                FuzzySet mediumBehindCoverWeight = new FuzzySet(FuzzyContext.Variable.BehindCoverWeight, "mediumBehindCoverWeight", new Trapezoid(0.2f, 0.4f, 0.6f, 0.8f));
                FuzzySet highBehindCoverWeight = new FuzzySet(FuzzyContext.Variable.BehindCoverWeight, "highBehindCoverWeight", new Trapezoid(0.6f, 0.8f, 1.0f, 1.0f));
                behindCoverWeight.AddSets(lowBehindCoverWeight, mediumBehindCoverWeight, highBehindCoverWeight);

                LinguisticVariable behindWallWeight = new LinguisticVariable();
                FuzzySet lowBehindWallWeight = new FuzzySet(FuzzyContext.Variable.BehindWallWeight, "lowBehindWallWeight", new Trapezoid(0.0f, 0.0f, 0.2f, 0.4f));
                FuzzySet mediumBehindWallWeight = new FuzzySet(FuzzyContext.Variable.BehindWallWeight, "mediumBehindWallWeight", new Trapezoid(0.2f, 0.4f, 0.6f, 0.8f));
                FuzzySet highBehindWallWeight = new FuzzySet(FuzzyContext.Variable.BehindWallWeight, "highBehindWallWeight", new Trapezoid(0.6f, 0.8f, 1.0f, 1.0f));
                behindWallWeight.AddSets(lowBehindWallWeight, mediumBehindWallWeight, highBehindWallWeight);

                LinguisticVariable healhPackWeight = new LinguisticVariable();
                FuzzySet lowHealhPackWeight = new FuzzySet(FuzzyContext.Variable.HealthPackWeight, "lowHealhPackWeight", new Trapezoid(0.0f, 0.0f, 0.2f, 0.4f));
                FuzzySet mediumHealhPackWeight = new FuzzySet(FuzzyContext.Variable.HealthPackWeight, "mediumHealhPackWeight", new Trapezoid(0.2f, 0.4f, 0.6f, 0.8f));
                FuzzySet highHealhPackWeight = new FuzzySet(FuzzyContext.Variable.HealthPackWeight, "highHealhPackWeight", new Trapezoid(0.6f, 0.8f, 1.0f, 1.0f));
                healhPackWeight.AddSets(lowHealhPackWeight, mediumHealhPackWeight, highHealhPackWeight);

                LinguisticVariable plainSightWeight = new LinguisticVariable();
                FuzzySet lowPlainSightWeight = new FuzzySet(FuzzyContext.Variable.PlainSightWeight, "lowPlainSightWeight", new Trapezoid(0.0f, 0.0f, 0.2f, 0.4f));
                FuzzySet mediumPlainSightWeight = new FuzzySet(FuzzyContext.Variable.PlainSightWeight, "mediumPlainSightWeight", new Trapezoid(0.2f, 0.4f, 0.6f, 0.8f));
                FuzzySet highPlainSightWeight = new FuzzySet(FuzzyContext.Variable.PlainSightWeight, "highPlainSightWeight", new Trapezoid(0.6f, 0.8f, 1.0f, 1.0f));
                plainSightWeight.AddSets(lowPlainSightWeight, mediumPlainSightWeight, highPlainSightWeight);

                LinguisticVariable searchRadius = new LinguisticVariable();
                FuzzySet lowSearchRadius = new FuzzySet(FuzzyContext.Variable.SearchRadius, "lowSearchRadius", new Trapezoid(0.0f, 0.0f, 6.0f, 8.0f));
                FuzzySet mediumSearchRadius = new FuzzySet(FuzzyContext.Variable.SearchRadius, "mediumSearchRadius", new Trapezoid(6.0f, 8.0f, 12.0f, 14.0f));
                FuzzySet highSearchRadius = new FuzzySet(FuzzyContext.Variable.SearchRadius, "highSearchRadius", new Trapezoid(12.0f, 14.0f, 100.0f, 100.0f));
                searchRadius.AddSets(lowSearchRadius, mediumSearchRadius, highSearchRadius);

                context.inputVariables.Add(FuzzyContext.Variable.AgentHealth, agentHealth);
                context.inputVariables.Add(FuzzyContext.Variable.EnemyHealth, enemyHealth);
                context.inputVariables.Add(FuzzyContext.Variable.EnemyVisibility, enemyVisibility);

                context.outputVariables.Add(FuzzyContext.Variable.DistanceToEnemy, distanceToEnemy);
                context.outputVariables.Add(FuzzyContext.Variable.DistanceWeight, distanceWeight);
                context.outputVariables.Add(FuzzyContext.Variable.InCoverWeight, inCoverWeight);
                context.outputVariables.Add(FuzzyContext.Variable.BehindCoverWeight, behindCoverWeight);
                context.outputVariables.Add(FuzzyContext.Variable.BehindWallWeight, behindWallWeight);
                context.outputVariables.Add(FuzzyContext.Variable.HealthPackWeight, healhPackWeight);
                context.outputVariables.Add(FuzzyContext.Variable.PlainSightWeight, plainSightWeight);
                context.outputVariables.Add(FuzzyContext.Variable.SearchRadius, searchRadius);

                inference.AddRule(
                    new FuzzyRule()
                        .In(highAgentHealth).In(highEnemyHealth).In(enemyIsVisible)
                        .Out(mediumDistance).Out(mediumDistanceWeight).Out(lowInCoverWeight).Out(lowBehindCoverWeight)
                        .Out(lowBehindWallWeight).Out(lowHealhPackWeight).Out(highPlainSightWeight).Out(lowSearchRadius)
                );
                inference.AddRule(
                    new FuzzyRule()
                        .In(highAgentHealth).In(mediumEnemyHealth).In(enemyIsVisible)
                        .Out(closeDistance).Out(highDistanceWeight).Out(lowInCoverWeight).Out(lowBehindCoverWeight)
                        .Out(lowBehindWallWeight).Out(lowHealhPackWeight).Out(highPlainSightWeight).Out(lowSearchRadius)
                );
                inference.AddRule(
                    new FuzzyRule()
                        .In(highAgentHealth).In(lowEnemyHealth).In(enemyIsVisible)
                        .Out(closeDistance).Out(highDistanceWeight).Out(lowInCoverWeight).Out(lowBehindCoverWeight)
                        .Out(lowBehindWallWeight).Out(lowHealhPackWeight).Out(highPlainSightWeight).Out(lowSearchRadius)
                );
                inference.AddRule(
                    new FuzzyRule()
                        .In(mediumAgentHealth).In(highEnemyHealth).In(enemyIsVisible)
                        .Out(farDistance).Out(mediumDistanceWeight).Out(highInCoverWeight).Out(mediumBehindCoverWeight)
                        .Out(mediumBehindWallWeight).Out(mediumHealhPackWeight).Out(mediumPlainSightWeight).Out(lowSearchRadius)
                );
                inference.AddRule(
                    new FuzzyRule()
                        .In(mediumAgentHealth).In(mediumEnemyHealth).In(enemyIsVisible)
                        .Out(mediumDistance).Out(mediumDistanceWeight).Out(mediumInCoverWeight).Out(mediumBehindCoverWeight)
                        .Out(lowBehindWallWeight).Out(mediumHealhPackWeight).Out(mediumPlainSightWeight).Out(lowSearchRadius)
                );
                inference.AddRule(
                    new FuzzyRule()
                        .In(mediumAgentHealth).In(lowEnemyHealth).In(enemyIsVisible)
                        .Out(closeDistance).Out(highDistanceWeight).Out(mediumInCoverWeight).Out(mediumBehindCoverWeight)
                        .Out(lowBehindWallWeight).Out(mediumHealhPackWeight).Out(lowPlainSightWeight).Out(lowSearchRadius)
                );
                inference.AddRule(
                    new FuzzyRule()
                        .In(lowAgentHealth).In(highEnemyHealth).In(enemyIsVisible)
                        .Out(farDistance).Out(highDistanceWeight).Out(highInCoverWeight).Out(highBehindCoverWeight)
                        .Out(highBehindWallWeight).Out(highHealhPackWeight).Out(lowPlainSightWeight).Out(highSearchRadius)
                );
                inference.AddRule(
                    new FuzzyRule()
                        .In(lowAgentHealth).In(mediumEnemyHealth).In(enemyIsVisible)
                        .Out(farDistance).Out(mediumDistanceWeight).Out(mediumInCoverWeight).Out(mediumBehindCoverWeight)
                        .Out(highBehindWallWeight).Out(highHealhPackWeight).Out(lowPlainSightWeight).Out(highSearchRadius)
                );
                inference.AddRule(
                    new FuzzyRule()
                        .In(lowAgentHealth).In(lowEnemyHealth).In(enemyIsVisible)
                        .Out(mediumDistance).Out(mediumDistanceWeight).Out(highInCoverWeight).Out(highBehindCoverWeight)
                        .Out(highBehindWallWeight).Out(highHealhPackWeight).Out(lowPlainSightWeight).Out(highSearchRadius)
                );
                inference.AddRule(
                    new FuzzyRule()
                        .In(highAgentHealth).In(enemyIsNotVisible)
                        .Out(closeDistance).Out(lowDistanceWeight).Out(lowInCoverWeight).Out(lowBehindCoverWeight)
                        .Out(lowBehindWallWeight).Out(lowHealhPackWeight).Out(lowPlainSightWeight).Out(highSearchRadius)
                );
                inference.AddRule(
                    new FuzzyRule()
                        .In(mediumAgentHealth).In(enemyIsNotVisible)
                        .Out(mediumDistance).Out(lowDistanceWeight).Out(lowInCoverWeight).Out(lowBehindCoverWeight)
                        .Out(lowBehindWallWeight).Out(mediumHealhPackWeight).Out(lowPlainSightWeight).Out(highSearchRadius)
                );
                inference.AddRule(
                    new FuzzyRule()
                        .In(lowAgentHealth).In(enemyIsNotVisible)
                        .Out(farDistance).Out(lowDistanceWeight).Out(lowInCoverWeight).Out(lowBehindCoverWeight)
                        .Out(lowBehindWallWeight).Out(highHealhPackWeight).Out(lowPlainSightWeight).Out(highSearchRadius)
                );
            }

            private void UpdateCrispValues()
            {
                context.crispInputValues[FuzzyContext.Variable.AgentHealth] = controller.GetState().health;
                context.crispInputValues[FuzzyContext.Variable.EnemyHealth] = controller.GetState().enemyState.health;
                context.crispInputValues[FuzzyContext.Variable.EnemyVisibility] = controller.GetState().isEnemyVisible ? 1.0f : 0.0f;
            }

            private void Fuzzify()
            {
                foreach (var varsKv in context.inputVariables)
                {
                    LinguisticValue value = varsKv.Value.Calculate(context.crispInputValues[varsKv.Key]);
                    foreach (var setsKv in value.GetSets())
                    {
                        context.fuzzyInputValues[setsKv.Key] = setsKv.Value;
                    }
                }
            }

            private void Infer()
            {
                context.inferedResult = inference.Infer();
            }

            private void Defuzzify()
            {
                Dictionary<FuzzyContext.Variable, List<KeyValuePair<FuzzySet, FuzzyValue>>> setsByVariable = new Dictionary<FuzzyContext.Variable, List<KeyValuePair<FuzzySet, FuzzyValue>>>();
                foreach (var setKv in context.inferedResult.GetSets())
                {
                    if (!setsByVariable.ContainsKey(setKv.Key.GetVariable()))
                    {
                        setsByVariable.Add(setKv.Key.GetVariable(), new List<KeyValuePair<FuzzySet, FuzzyValue>>());
                    }
                    setsByVariable[setKv.Key.GetVariable()].Add(setKv);
                }

                foreach (var varKV in setsByVariable)
                {
                    FuzzyContext.Variable variable = varKV.Key;
                    float min = float.MaxValue;
                    float max = float.MinValue;
                    foreach (var setKv in varKV.Value)
                    {
                        min = Mathf.Min(min, setKv.Key.GetMembershipFunction().GetMinCrispValue());
                        max = Mathf.Max(max, setKv.Key.GetMembershipFunction().GetMaxCrispValue());
                    }
                    float step = (max - min) / 100.0f;
                    float numerator = 0.0f;
                    float denominator = 0.0f;
                    for (float x = min; x <= max; x += step)
                    {
                        float maxMembership = 0.0f;
                        foreach (var setKv in varKV.Value)
                        {
                            maxMembership = Mathf.Max(maxMembership, Mathf.Min(setKv.Key.GetMembershipFunction().Calculate(x).value, setKv.Value.value));
                        }
                        numerator += x * maxMembership;
                        denominator += maxMembership;
                    }
                    float centerOfMass = numerator / denominator;
                    context.crispOutputValues[variable] = centerOfMass;
                }
            }

            private void ApplyResultValues()
            {
                aiTools.terrain.SetWeightFunction(wp =>
                {
                    float MIN_ATTACK_RADIUS = context.crispOutputValues[FuzzyContext.Variable.DistanceToEnemy] / 2.0f;
                    float MAX_ATTACK_RADIUS = context.crispOutputValues[FuzzyContext.Variable.DistanceToEnemy] * 2.0f;
                    float weight = 0.0f;
                    if (wp.isBehindWall)
                    {
                        weight += context.crispOutputValues[FuzzyContext.Variable.BehindWallWeight];
                    }
                    if (wp.isInCover)
                    {
                        weight += context.crispOutputValues[FuzzyContext.Variable.InCoverWeight];
                    }
                    if (wp.isBehindCover)
                    {
                        weight += context.crispOutputValues[FuzzyContext.Variable.BehindCoverWeight];
                    }
                    if (wp.isHealthPack)
                    {
                        weight += context.crispOutputValues[FuzzyContext.Variable.HealthPackWeight];
                    }
                    if (!wp.isInCover && !wp.isBehindCover && !wp.isBehindWall)
                    {
                        weight += context.crispOutputValues[FuzzyContext.Variable.PlainSightWeight];
                    }
                    if (MIN_ATTACK_RADIUS <= wp.directDistanceToEnemy && wp.directDistanceToEnemy <= MAX_ATTACK_RADIUS)
                    {
                        float center = (MIN_ATTACK_RADIUS + MAX_ATTACK_RADIUS) / 2.0f;
                        if (wp.directDistanceToEnemy < center)
                        {
                            weight += context.crispOutputValues[FuzzyContext.Variable.DistanceWeight]
                                * (wp.directDistanceToEnemy - MIN_ATTACK_RADIUS) / (center - MIN_ATTACK_RADIUS);
                        }
                        else
                        {
                            weight += context.crispOutputValues[FuzzyContext.Variable.DistanceWeight]
                                * (MAX_ATTACK_RADIUS - wp.directDistanceToEnemy) / (MAX_ATTACK_RADIUS - center);
                        }
                    }
                    return weight;
                });
            }

            private void DoStuff()
            {
                aiTools.shooting.SetAimingEnabled(controller.GetState().isEnemyVisible);
                aiTools.shooting.SetShootingEnabled(controller.GetState().isEnemyVisible);

                if (!destination.HasValue)
                {
                    destination = GetNextDestination();
                    aiTools.navigation.SetDestination(destination);
                }

                if (
                    !aiTools.navigation.IsDestinationReachable()
                    || (destination.HasValue && Vector3.Distance(aiTools.agentState.position, destination.Value) < GameDefines.NEAR_POINT_CHECK_PRECISION)
                    )
                {
                    destination = null;
                }

                if (aiTools.agentState.isEnemyVisible != enemyVisibilityOnLastTurn)
                {
                    destination = null;
                }
                enemyVisibilityOnLastTurn = aiTools.agentState.isEnemyVisible;
            }

            private Vector3 GetNextDestination()
            {
                List<Waypoint> wps = aiTools.terrain.GetGoodWaypoints(context.crispOutputValues[FuzzyContext.Variable.SearchRadius], 0.75f);
                if (wps.Count == 0)
                {
                    return GetRandomNextDestination();
                }
                else
                {
                    int index = UnityEngine.Random.Range(0, wps.Count - 1);
                    return wps[index].position;
                }
            }

            private Vector3 GetRandomNextDestination()
            {
                float searchRadius = context.crispOutputValues[FuzzyContext.Variable.SearchRadius];
                return new Vector3(
                    aiTools.agentState.position.x + UnityEngine.Random.Range(-searchRadius, searchRadius),
                    0.0f,
                    aiTools.agentState.position.z + UnityEngine.Random.Range(-searchRadius, searchRadius)
                    );
            }
        }
    }
}