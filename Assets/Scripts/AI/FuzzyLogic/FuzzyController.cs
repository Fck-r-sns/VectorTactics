using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            void Start()
            {
                aiTools = GetComponent<AiTools>();
                controller = GetComponent<SoldierController>();

                Init();
            }

            void Update()
            {
                if (controller.GetState().isDead)
                {
                    return;
                }
                UpdateCrispValues();
                Fuzzify();
                Infer();
                Defuzzify();
                ApplyResultValues();
            }

            private void Init()
            {
                calculator = new FuzzyCalculator();
                context = new FuzzyContext();
                inference = new FuzzyInference(calculator, context);

                LinguisticVariable agentHealth = new LinguisticVariable();
                FuzzySet lowAgentHealth = new FuzzySet(new Trapezoid(0.0f, 0.0f, 30.0f, 60.0f));
                FuzzySet mediumAgentHealth = new FuzzySet(new Trapezoid(30.0f, 60.0f, 60.0f, 80.0f));
                FuzzySet highAgentHealth = new FuzzySet(new Trapezoid(60.0f, 80.0f, 100.0f, 100.0f));
                agentHealth.AddSets(lowAgentHealth, mediumAgentHealth, highAgentHealth);

                LinguisticVariable enemyHealth = new LinguisticVariable();
                FuzzySet lowEnemyHealth = new FuzzySet(new Trapezoid(0.0f, 0.0f, 30.0f, 60.0f));
                FuzzySet mediumEnemyHealth = new FuzzySet(new Trapezoid(30.0f, 60.0f, 60.0f, 80.0f));
                FuzzySet highEnemyHealth = new FuzzySet(new Trapezoid(60.0f, 80.0f, 100.0f, 100.0f));
                enemyHealth.AddSets(lowEnemyHealth, mediumEnemyHealth, highEnemyHealth);

                LinguisticVariable enemyVisibility = new LinguisticVariable();
                FuzzySet enemyIsVisible = new FuzzySet(new Singleton(1.0f));
                FuzzySet enemyIsNotVisible = new FuzzySet(new Singleton(0.0f));
                enemyVisibility.AddSets(enemyIsVisible, enemyIsNotVisible);

                LinguisticVariable distanceToEnemy = new LinguisticVariable();
                FuzzySet closeDistance = new FuzzySet(new Trapezoid(0.0f, 0.0f, 4.0f, 8.0f));
                FuzzySet mediumDistance = new FuzzySet(new Trapezoid(4.0f, 8.0f, 12.0f, 18.0f));
                FuzzySet farDistance = new FuzzySet(new Trapezoid(12.0f, 18.0f, 100.0f, 100.0f));
                distanceToEnemy.AddSets(closeDistance, mediumDistance, farDistance);

                LinguisticVariable distanceWeight = new LinguisticVariable();
                FuzzySet lowDistanceWeight = new FuzzySet(new Trapezoid(0.0f, 0.0f, 0.2f, 0.4f));
                FuzzySet mediumDistanceWeight = new FuzzySet(new Trapezoid(0.2f, 0.4f, 0.6f, 0.8f));
                FuzzySet highDistanceWeight = new FuzzySet(new Trapezoid(0.6f, 0.8f, 1.0f, 1.0f));
                distanceWeight.AddSets(lowDistanceWeight, mediumDistanceWeight, highDistanceWeight);

                LinguisticVariable inCoverWeight = new LinguisticVariable();
                FuzzySet lowInCoverWeight = new FuzzySet(new Trapezoid(0.0f, 0.0f, 0.2f, 0.4f));
                FuzzySet mediumInCoverWeight = new FuzzySet(new Trapezoid(0.2f, 0.4f, 0.6f, 0.8f));
                FuzzySet highInCoverWeight = new FuzzySet(new Trapezoid(0.6f, 0.8f, 1.0f, 1.0f));
                inCoverWeight.AddSets(lowInCoverWeight, mediumInCoverWeight, highInCoverWeight);

                LinguisticVariable behindCoverWeight = new LinguisticVariable();
                FuzzySet lowBehindCoverWeight = new FuzzySet(new Trapezoid(0.0f, 0.0f, 0.2f, 0.4f));
                FuzzySet mediumBehindCoverWeight = new FuzzySet(new Trapezoid(0.2f, 0.4f, 0.6f, 0.8f));
                FuzzySet highBehindCoverWeight = new FuzzySet(new Trapezoid(0.6f, 0.8f, 1.0f, 1.0f));
                behindCoverWeight.AddSets(lowBehindCoverWeight, mediumBehindCoverWeight, highBehindCoverWeight);

                LinguisticVariable behindWallWeight = new LinguisticVariable();
                FuzzySet lowBehindWallWeight = new FuzzySet(new Trapezoid(0.0f, 0.0f, 0.2f, 0.4f));
                FuzzySet mediumBehindWallWeight = new FuzzySet(new Trapezoid(0.2f, 0.4f, 0.6f, 0.8f));
                FuzzySet highBehindWallWeight = new FuzzySet(new Trapezoid(0.6f, 0.8f, 1.0f, 1.0f));
                behindWallWeight.AddSets(lowBehindWallWeight, mediumBehindWallWeight, highBehindWallWeight);

                LinguisticVariable healhPackWeight = new LinguisticVariable();
                FuzzySet lowHealhPackWeight = new FuzzySet(new Trapezoid(0.0f, 0.0f, 0.2f, 0.4f));
                FuzzySet mediumHealhPackWeight = new FuzzySet(new Trapezoid(0.2f, 0.4f, 0.6f, 0.8f));
                FuzzySet highHealhPackWeight = new FuzzySet(new Trapezoid(0.6f, 0.8f, 1.0f, 1.0f));
                healhPackWeight.AddSets(lowHealhPackWeight, mediumHealhPackWeight, highHealhPackWeight);

                LinguisticVariable plainSightWeight = new LinguisticVariable();
                FuzzySet lowPlainSightWeight = new FuzzySet(new Trapezoid(0.0f, 0.0f, 0.2f, 0.4f));
                FuzzySet mediumPlainSightWeight = new FuzzySet(new Trapezoid(0.2f, 0.4f, 0.6f, 0.8f));
                FuzzySet highPlainSightWeight = new FuzzySet(new Trapezoid(0.6f, 0.8f, 1.0f, 1.0f));
                plainSightWeight.AddSets(lowPlainSightWeight, mediumPlainSightWeight, highPlainSightWeight);

                LinguisticVariable searchRadius = new LinguisticVariable();
                FuzzySet lowSearchRadius = new FuzzySet(new Trapezoid(0.0f, 0.0f, 0.2f, 0.4f));
                FuzzySet mediumSearchRadius = new FuzzySet(new Trapezoid(0.2f, 0.4f, 0.6f, 0.8f));
                FuzzySet highSearchRadius = new FuzzySet(new Trapezoid(0.6f, 0.8f, 1.0f, 1.0f));
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
                        .Out(lowBehindWallWeight).Out(lowHealhPackWeight).Out(highPlainSightWeight).Out(mediumSearchRadius)
                );
                inference.AddRule(
                    new FuzzyRule()
                        .In(highAgentHealth).In(mediumEnemyHealth).In(enemyIsVisible)
                        .Out(closeDistance).Out(highDistanceWeight).Out(lowInCoverWeight).Out(lowBehindCoverWeight)
                        .Out(lowBehindWallWeight).Out(lowHealhPackWeight).Out(highPlainSightWeight).Out(mediumSearchRadius)
                );
                inference.AddRule(
                    new FuzzyRule()
                        .In(highAgentHealth).In(lowEnemyHealth).In(enemyIsVisible)
                        .Out(closeDistance).Out(highDistanceWeight).Out(lowInCoverWeight).Out(lowBehindCoverWeight)
                        .Out(lowBehindWallWeight).Out(lowHealhPackWeight).Out(highPlainSightWeight).Out(mediumSearchRadius)
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

            }

            private void Fuzzify() {

            }

            private void Infer()
            {

            }

            private void Defuzzify()
            {

            }

            private void ApplyResultValues()
            {

            }
        }
    }
}