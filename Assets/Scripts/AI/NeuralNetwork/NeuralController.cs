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

                SearchOptimal3LayerNetwork();
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
            }

            private float Test(List<LearningRecord> testingSet)
            {
                int matchesCount = 0;
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
                }
                return matchesCount / (float)testingSet.Count;
            }

            private float Function(float sigmoidCoef, float learningSpeedCoef, int repeatLearningTimes)
            {
                return Function(false, 0, sigmoidCoef, learningSpeedCoef, repeatLearningTimes);
            }

            private float Function(int hiddenLayerSize, float sigmoidCoef, float learningSpeedCoef, int repeatLearningTimes)
            {
                return Function(true, hiddenLayerSize, sigmoidCoef, learningSpeedCoef, repeatLearningTimes);
            }

            private float Function(bool useHiddenLayer, int hiddenLayerSize, float sigmoidCoef, float learningSpeedCoef, int repeatLearningTimes)
            {
                NeuralDefines.USE_HIDDEN_LAYER = useHiddenLayer;
                NeuralDefines.HIDDEN_LAYER_SIZE = hiddenLayerSize;
                NeuralDefines.SIGMOID_STEEP_COEF = sigmoidCoef;
                NeuralDefines.LEARNING_SPEED_COEF = learningSpeedCoef;
                NeuralDefines.REPEAT_LEARNING_TIMES = repeatLearningTimes;
                InitNetwork();
                List<LearningRecord> learningSet, testingSet;
                ReadLearningData(out learningSet, out testingSet);
                Learn(learningSet);
                return Test(testingSet);
            }

            private void SearchOptimal2LayerNetwork()
            {
                float r = (Mathf.Sqrt(5) - 1.0f) / 2.0f;

                int[] repeats = new int[] { 1, 2, 3, 4, 5, 7, 11, 15, 21, 51, 101, 201, 501, 1001, 2001 };
                for (int repeatsIdx = 0; repeatsIdx < repeats.Length; ++repeatsIdx)
                {
                    float sigm0 = 1.2f;
                    float learn0 = 0.2f;
                    float prevResult = -1, result = -1;
                    int iterations = 0;
                    do
                    {
                        ++iterations;
                        prevResult = result;
                        float sigmL = 0.01f;
                        float sigmR = 2.0f;
                        float learnL = 0.01f;
                        float learnR = 2.0f;
                        float x1, x2;

                        do
                        {
                            x1 = learnL + (1 - r) * (learnR - learnL);
                            x2 = learnL + r * (learnR - learnL);
                            float y1 = Function(sigm0, x1, repeats[repeatsIdx]);
                            float y2 = Function(sigm0, x2, repeats[repeatsIdx]);
                            if (y1 > y2)
                            {
                                learnR = x2;
                            }
                            else
                            {
                                learnL = x1;
                            }
                        } while (!Mathf.Approximately(x1, x2));
                        learn0 = x1;

                        do
                        {
                            x1 = sigmL + (1 - r) * (sigmR - sigmL);
                            x2 = sigmL + r * (sigmR - sigmL);
                            float y1 = Function(x1, learn0, repeats[repeatsIdx]);
                            float y2 = Function(x2, learn0, repeats[repeatsIdx]);
                            if (y1 > y2)
                            {
                                sigmR = x2;
                            }
                            else
                            {
                                sigmL = x1;
                            }
                        } while (!Mathf.Approximately(x1, x2));
                        sigm0 = x1;

                        result = Function(sigm0, learn0, repeats[repeatsIdx]);

                    } while (prevResult != result);

                    Debug.Log("repeats = " + repeats[repeatsIdx] + ", sigm = " + sigm0 + ", learn = " + learn0 + " -> " + Function(sigm0, learn0, repeats[repeatsIdx]) + ", iterations = " + iterations);
                }
            }

            private void SearchOptimal3LayerNetwork()
            {
                float r = (Mathf.Sqrt(5) - 1.0f) / 2.0f;

                int[] repeats = new int[] { 1, 2, 3, 4, 5, 7, 11, 15, 21, 51, 101, 201, 501, 1001, 2001 };
                for (int repeatsIdx = 0; repeatsIdx < repeats.Length; ++repeatsIdx)
                {
                    float layerSize0 = 5f;
                    float sigm0 = 1.2f;
                    float learn0 = 0.2f;
                    float prevResult = -1, result = -1;
                    int iterations = 0;
                    do
                    {
                        ++iterations;
                        prevResult = result;
                        float layerSizeL = 3f;
                        float layerSizeR = 20f;
                        float sigmL = 0.01f;
                        float sigmR = 2.0f;
                        float learnL = 0.01f;
                        float learnR = 2.0f;
                        float x1, x2;

                        do
                        {
                            x1 = learnL + (1 - r) * (learnR - learnL);
                            x2 = learnL + r * (learnR - learnL);
                            float y1 = Function(Mathf.RoundToInt(layerSize0), sigm0, x1, repeats[repeatsIdx]);
                            float y2 = Function(Mathf.RoundToInt(layerSize0), sigm0, x2, repeats[repeatsIdx]);
                            if (y1 > y2)
                            {
                                learnR = x2;
                            }
                            else
                            {
                                learnL = x1;
                            }
                        } while (!Mathf.Approximately(x1, x2));
                        learn0 = x1;

                        do
                        {
                            x1 = sigmL + (1 - r) * (sigmR - sigmL);
                            x2 = sigmL + r * (sigmR - sigmL);
                            float y1 = Function(Mathf.RoundToInt(layerSize0), x1, learn0, repeats[repeatsIdx]);
                            float y2 = Function(Mathf.RoundToInt(layerSize0), x2, learn0, repeats[repeatsIdx]);
                            if (y1 > y2)
                            {
                                sigmR = x2;
                            }
                            else
                            {
                                sigmL = x1;
                            }
                        } while (!Mathf.Approximately(x1, x2));
                        sigm0 = x1;

                        do
                        {
                            x1 = layerSizeL + (1 - r) * (layerSizeR - layerSizeL);
                            x2 = layerSizeL + r * (layerSizeR - layerSizeL);
                            float y1 = Function(Mathf.RoundToInt(x1), sigm0, learn0, repeats[repeatsIdx]);
                            float y2 = Function(Mathf.RoundToInt(x2), sigm0, learn0, repeats[repeatsIdx]);
                            if (y1 > y2)
                            {
                                layerSizeR = x2;
                            }
                            else
                            {
                                layerSizeL = x1;
                            }
                        } while (!Mathf.Approximately(x1, x2));
                        layerSize0 = x1;

                        result = Function(sigm0, learn0, repeats[repeatsIdx]);

                    } while (prevResult != result);

                    Debug.Log("repeats = " + repeats[repeatsIdx] + ", sigm = " + sigm0 + ", learn = " + learn0 + ", layerSize = " + layerSize0 
                        + " -> " + Function(Mathf.RoundToInt(layerSize0), sigm0, learn0, repeats[repeatsIdx]) + ", iterations = " + iterations);
                }
            }
        }
    }
}