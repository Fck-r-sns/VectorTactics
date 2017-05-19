using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Ai
{
    namespace Nn
    {
        [RequireComponent(typeof(AiTools))]
        [RequireComponent(typeof(SoldierController))]
        public class NeuralController : MonoBehaviour
        {
            private enum OutputVariable
            {
                Attack,
                Defence,
                SearchEnemy,
                SearchHealth
            }

            private class LearningRecord
            {
                public float agentHealth;
                public float enemyHealth;
                public float enemyVisibility;
                public float attack;
                public float defence;
                public float searchEnemy;
                public float searchHealth;
            }

            private AiTools aiTools;
            private SoldierController controller;

            private float agentHealthInput;
            private float enemyHealthInput;
            private float enemyVisibilityInput;

            private List<Neuron> inputLayer;
            private List<Neuron> hiddenLayer;
            private List<Neuron> outputLayer;

            private Dictionary<OutputVariable, Strategy> strategies = new Dictionary<OutputVariable, Strategy>();
            private Strategy currentStrategy;

            private void Start()
            {
                aiTools = GetComponent<AiTools>();
                controller = GetComponent<SoldierController>();

                aiTools.Init();
                InitStrategies();
                InitNetwork();

                List<LearningRecord> learningSet, testingSet;
                ReadLearningData(out learningSet, out testingSet);
                Learn(learningSet);
                Test(testingSet);
            }

            private void Update()
            {
                agentHealthInput = aiTools.agentState.health;
                enemyHealthInput = aiTools.enemyState.health;
                enemyVisibilityInput = aiTools.agentState.isEnemyVisible ? 1.0f : 0.0f;
                FeedForwardNetwork();
                OutputVariable? optimalStrategy = null;
                float maxWeight = -float.MaxValue;
                for (int i = 0; i < outputLayer.Count; ++i)
                {
                    Neuron output = outputLayer[i];
                    if (!optimalStrategy.HasValue || maxWeight < output.GetOutput())
                    {
                        optimalStrategy = (OutputVariable)i;
                        maxWeight = output.GetOutput();
                    }
                }
                Strategy newStrategy = strategies[optimalStrategy.Value];
                if (currentStrategy != newStrategy)
                {
                    //Debug.Log(Time.time + ": new strategy - " + newStrategy.GetType().Name);
                    currentStrategy.OnExit();
                    currentStrategy = newStrategy;
                    currentStrategy.OnEnter();
                }
                currentStrategy.OnUpdate();
            }

            private void InitStrategies()
            {
                strategies.Add(OutputVariable.Attack, new AttackStrategy(aiTools));
                strategies.Add(OutputVariable.Defence, new DefenceStrategy(aiTools));
                strategies.Add(OutputVariable.SearchEnemy, new SearchEnemyStrategy(aiTools));
                strategies.Add(OutputVariable.SearchHealth, new SearchHealthStrategy(aiTools));
                currentStrategy = strategies[OutputVariable.SearchEnemy];
                currentStrategy.OnEnter();
            }

            private void InitNetwork()
            {
                inputLayer = new List<Neuron>();
                if (NeuralDefines.USE_HIDDEN_LAYER)
                {
                    hiddenLayer = new List<Neuron>();
                }
                outputLayer = new List<Neuron>();

                inputLayer.Add(new InputNeuron(() => this.agentHealthInput / 100.0f));
                inputLayer.Add(new InputNeuron(() => this.enemyHealthInput / 100.0f));
                inputLayer.Add(new InputNeuron(() => this.enemyVisibilityInput));

                if (NeuralDefines.USE_HIDDEN_LAYER)
                {
                    for (int i = 0; i < NeuralDefines.HIDDEN_LAYER_SIZE; ++i)
                    {
                        Neuron neuron = new HiddenNeuron(NeuralDefines.SIGMOID_FUNCTION);
                        foreach (Neuron input in inputLayer)
                        {
                            neuron.AddInput(input);
                        }
                        hiddenLayer.Add(neuron);
                    }
                }

                foreach (OutputVariable var in Enum.GetValues(typeof(OutputVariable)))
                {
                    Neuron neuron = new OutputNeuron(NeuralDefines.SIGMOID_FUNCTION);
                    List<Neuron> previousLayer = NeuralDefines.USE_HIDDEN_LAYER ? hiddenLayer : inputLayer;
                    foreach (Neuron input in previousLayer)
                    {
                        neuron.AddInput(input);
                    }
                    outputLayer.Add(neuron);
                }
            }

            private void FeedForwardNetwork()
            {
                if (NeuralDefines.USE_HIDDEN_LAYER)
                {
                    foreach (Neuron n in hiddenLayer)
                    {
                        n.FeedForward();
                    }
                }
                foreach (Neuron n in outputLayer)
                {
                    n.FeedForward();
                }
            }

            void ReadLearningData(out List<LearningRecord> learningSet, out List<LearningRecord> testingSet)
            {
                List<LearningRecord> records = new List<LearningRecord>();
                using (TextReader file = new StreamReader(File.OpenRead("./Files/NeuralNetworkLearningSet.csv")))
                {
                    string line = file.ReadLine(); // skip header
                    while ((line = file.ReadLine()) != null)
                    {
                        string[] split = line.Split(';');
                        LearningRecord record = new LearningRecord()
                        {
                            agentHealth = float.Parse(split[0]),
                            enemyHealth = float.Parse(split[1]),
                            enemyVisibility = float.Parse(split[2]),
                            attack = float.Parse(split[4]),
                            defence = float.Parse(split[5]),
                            searchEnemy = float.Parse(split[6]),
                            searchHealth = float.Parse(split[7])
                        };
                        records.Add(record);
                    }
                }

                learningSet = new List<LearningRecord>(records.Count / 2 + 1);
                testingSet = new List<LearningRecord>(records.Count - learningSet.Count + 1);
                for (int i = 0; i < records.Count; ++i)
                {
                    if (i % 2 == 1)
                    {
                        learningSet.Add(records[i]);
                    }
                    else
                    {
                        testingSet.Add(records[i]);
                    }
                }
            }

            private void Learn(List<LearningRecord> learningSet)
            {
                Debug.Log(Time.realtimeSinceStartup + ": start learning");
                for (int i = 0; i < NeuralDefines.REPEAT_LEARNING_TIMES; ++i)
                {
                    learningSet.Reverse();
                    foreach (LearningRecord record in learningSet)
                    {
                        agentHealthInput = record.agentHealth;
                        enemyHealthInput = record.enemyHealth;
                        enemyVisibilityInput = record.enemyVisibility;
                        FeedForwardNetwork();
                        outputLayer[(int)OutputVariable.Attack].SetExpectedOutput(record.attack);
                        outputLayer[(int)OutputVariable.Defence].SetExpectedOutput(record.defence);
                        outputLayer[(int)OutputVariable.SearchEnemy].SetExpectedOutput(record.searchEnemy);
                        outputLayer[(int)OutputVariable.SearchHealth].SetExpectedOutput(record.searchHealth);
                        foreach (Neuron neuron in outputLayer)
                        {
                            neuron.BackPropagate();
                        }
                        if (NeuralDefines.USE_HIDDEN_LAYER)
                        {
                            foreach (Neuron neuron in hiddenLayer)
                            {
                                neuron.BackPropagate();
                            }
                        }
                    }
                }
                Debug.Log(Time.realtimeSinceStartup + ": learning finished");
            }

            private float Test(List<LearningRecord> testingSet)
            {
                Debug.Log(Time.realtimeSinceStartup + ": start testing");
                int matchesCount = 0;
                using (TextWriter file = new StreamWriter(File.OpenWrite("./Files/NeuralNetworkTesting.csv")))
                {
                    file.WriteLine(
                        "HPa;HPe;Visible;ExpStrategy;ExpAttack;ExpDefence;ExpSearchEnemy;ExpSearchHealth;Strategy;Attack;Defence;SearchEnemy;SearchHealth;ErrAttack;ErrDefence;ErrSearhEnemy;ErrSearchHealth;ResMatch;"
                        );
                    foreach (LearningRecord record in testingSet)
                    {
                        agentHealthInput = record.agentHealth;
                        enemyHealthInput = record.enemyHealth;
                        enemyVisibilityInput = record.enemyVisibility;
                        FeedForwardNetwork();
                        outputLayer[(int)OutputVariable.Attack].SetExpectedOutput(record.attack);
                        outputLayer[(int)OutputVariable.Defence].SetExpectedOutput(record.defence);
                        outputLayer[(int)OutputVariable.SearchEnemy].SetExpectedOutput(record.searchEnemy);
                        outputLayer[(int)OutputVariable.SearchHealth].SetExpectedOutput(record.searchHealth);
                        List<float> expected = new List<float>()
                        {
                            record.attack,
                            record.defence,
                            record.searchEnemy,
                            record.searchHealth
                        };
                        List<float> actual = new List<float>()
                        {
                            outputLayer[(int)OutputVariable.Attack].GetOutput(),
                            outputLayer[(int)OutputVariable.Defence].GetOutput(),
                            outputLayer[(int)OutputVariable.SearchEnemy].GetOutput(),
                            outputLayer[(int)OutputVariable.SearchHealth].GetOutput()
                        };
                        OutputVariable expectedStrategy = (OutputVariable)expected.IndexOf(expected.Max());
                        OutputVariable actualStrategy = (OutputVariable)actual.IndexOf(actual.Max());
                        if (expectedStrategy == actualStrategy)
                        {
                            ++matchesCount;
                        }
                        file.WriteLine(
                            record.agentHealth + ";" +
                            record.enemyHealth + ";" +
                            record.enemyVisibility + ";" +
                            expectedStrategy + ";" +
                            expected[(int)OutputVariable.Attack] + ";" +
                            expected[(int)OutputVariable.Defence] + ";" +
                            expected[(int)OutputVariable.SearchEnemy] + ";" +
                            expected[(int)OutputVariable.SearchHealth] + ";" +
                            actualStrategy + ";" +
                            actual[(int)OutputVariable.Attack] + ";" +
                            actual[(int)OutputVariable.Defence] + ";" +
                            actual[(int)OutputVariable.SearchEnemy] + ";" +
                            actual[(int)OutputVariable.SearchHealth] + ";" +
                            outputLayer[(int)OutputVariable.Attack].GetError() + ";" +
                            outputLayer[(int)OutputVariable.Defence].GetError() + ";" +
                            outputLayer[(int)OutputVariable.SearchEnemy].GetError() + ";" +
                            outputLayer[(int)OutputVariable.SearchHealth].GetError() + ";" +
                            (expectedStrategy == actualStrategy) + ";"
                            );
                    }
                }
                float learningQuality = matchesCount / testingSet.Count * 100.0f;
                Debug.Log(Time.realtimeSinceStartup + ": testing finished, matches count = "
                    + matchesCount + "/" + testingSet.Count
                    + "(" + learningQuality + "%)"
                    );
                return learningQuality;
            }
        }
    }
}