using System;
using System.Collections.Generic;
using System.IO;
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

            private List<Neuron> inputLayer = new List<Neuron>();
            private List<Neuron> hiddenLayer = new List<Neuron>();
            private List<Neuron> outputLayer = new List<Neuron>();

            private Dictionary<OutputVariable, Strategy> strategies = new Dictionary<OutputVariable, Strategy>();
            private Strategy currentStrategy;

            private void Start()
            {
                aiTools = GetComponent<AiTools>();
                controller = GetComponent<SoldierController>();
                aiTools.Init();
                Init();
                Debug.Log(Time.realtimeSinceStartup + ": start learning");
                Learn();
                Debug.Log(Time.realtimeSinceStartup + ": learning finished");
            }

            private void Update()
            {
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
                    currentStrategy.OnExit();
                    currentStrategy = newStrategy;
                    currentStrategy.OnEnter();
                }
                currentStrategy.OnUpdate();
            }

            private void Init()
            {
                inputLayer.Add(new InputNeuron(() => this.agentHealthInput / 100.0f));
                inputLayer.Add(new InputNeuron(() => this.enemyHealthInput / 100.0f));
                inputLayer.Add(new InputNeuron(() => this.enemyVisibilityInput));

                for (int i = 0; i < NeuralDefines.HIDDEN_LAYER_SIZE; ++i)
                {
                    Neuron neuron = new HiddenNeuron(NeuralDefines.SIGMOID_FUNCTION);
                    foreach (Neuron input in inputLayer)
                    {
                        neuron.AddInput(input);
                    }
                    hiddenLayer.Add(neuron);
                }

                foreach (OutputVariable var in Enum.GetValues(typeof(OutputVariable)))
                {
                    Neuron neuron = new OutputNeuron(NeuralDefines.SIGMOID_FUNCTION);
                    foreach (Neuron input in hiddenLayer)
                    {
                        neuron.AddInput(input);
                    }
                    outputLayer.Add(neuron);
                }

                strategies.Add(OutputVariable.Attack, new AttackStrategy(aiTools));
                strategies.Add(OutputVariable.Defence, new DefenceStrategy(aiTools));
                strategies.Add(OutputVariable.SearchEnemy, new SearchEnemyStrategy(aiTools));
                strategies.Add(OutputVariable.SearchHealth, new SearchHealthStrategy(aiTools));
                currentStrategy = strategies[OutputVariable.SearchEnemy];
            }

            private void FeedForwardNetwork()
            {
                foreach (Neuron n in hiddenLayer)
                {
                    n.FeedForward();
                }
                foreach (Neuron n in outputLayer)
                {
                    n.FeedForward();
                }
            }

            private void Learn()
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

                List<LearningRecord> learningSet = new List<LearningRecord>(records.Count / 2 + 1);
                List<LearningRecord> testingSet = new List<LearningRecord>(records.Count - learningSet.Count + 1);
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
                    foreach (Neuron neuron in hiddenLayer)
                    {
                        neuron.BackPropagate();
                    }
                }
            }
        }
    }
}