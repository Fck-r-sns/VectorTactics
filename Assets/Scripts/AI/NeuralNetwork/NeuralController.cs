using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    namespace Nn
    {
        [RequireComponent(typeof(AiTools))]
        [RequireComponent(typeof(SoldierController))]
        public class NeuralContoller : MonoBehaviour
        {
            private enum OutputVariable
            {
                Attack,
                Defence,
                SearchEnemy,
                SearchHealth
            }

            private AiTools aiTools;
            private SoldierController controller;

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
            }

            private void Update()
            {
                foreach (Neuron n in hiddenLayer)
                {
                    n.FeedForward();
                }
                foreach (Neuron n in outputLayer)
                {
                    n.FeedForward();
                }
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
                inputLayer.Add(new InputNeuron(() => aiTools.agentState.health));
                inputLayer.Add(new InputNeuron(() => aiTools.enemyState.health));
                inputLayer.Add(new InputNeuron(() => aiTools.agentState.isEnemyVisible ? 1.0f : 0.0f));

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
        }
    }
}