using UnityEngine;

using EventBus;

namespace Ai
{
    namespace Bt
    {
        [RequireComponent(typeof(AiTools))]
        [RequireComponent(typeof(SoldierController))]
        public class BtController : MonoBehaviour
        {
            private const string WEIGHT_FUNCTION_CHANGED_VARIABLE = "weightFunctionChanged";
            private const string DESTINATION_SEARCH_RADIUS_VARIABLE = "destinationSearchRadius";
            private const string DESTINATION_SEARCH_MIN_WEIGHT_VARIABLE = "destinationSearchMinWeight";
            private const string DESTINATION_POINT_VARIABLE = "destination";

            private AiTools aiTools;
            private SoldierController controller;

            private Node root;

            void Start()
            {
                controller = GetComponent<SoldierController>();
                InitTree();

                Dispatcher.GetInstance().SendEvent(new ControllerInited(aiTools.agentState.side, GameDefines.ControllerType.BehaviorTree));
            }

            void Update()
            {
                if (!controller.GetState().isDead)
                {
                    float time = Time.realtimeSinceStartup;
                    root.Run();
                    Dispatcher.GetInstance().SendEvent(new NewFrame(aiTools.agentState.side, Time.realtimeSinceStartup - time));
                }
            }

            private void InitTree()
            {
                Environment environment = new Environment();
                aiTools = GetComponent<AiTools>();
                aiTools.Init();

                root = new Sequence(environment);

                // shooting branch
                root.AddChild(
                    new Selector(environment)
                        .AddChild(
                            new Sequence(environment)
                                .AddChild(new IsTargetVisible(environment, aiTools))
                                .AddChild(new StartShooting(environment, aiTools))
                        )
                        .AddChild(new StopShooting(environment, aiTools))
                );

                // weight function branch
                root.AddChild(
                    new Selector(environment)
                        .AddChild(
                            new Sequence(environment)
                                .AddChild(new IsTargetVisible(environment, aiTools))
                                .AddChild(
                                    new Selector(environment)
                                        .AddChild(
                                            // attack state
                                            new Sequence(environment)
                                                .AddChild(new IsHealthMoreThan(environment, aiTools, GameDefines.MEDIUM_HP))
                                                .AddChild(new SetWeightFunction(environment, aiTools, TerrainReasoning.AGGRESSIVE_WEIGHT_FUNCTION))
                                                .AddChild(new SetEnvironmentVariable<float>(environment, DESTINATION_SEARCH_RADIUS_VARIABLE, 5.0f))
                                                .AddChild(new SetEnvironmentVariable<float>(environment, DESTINATION_SEARCH_MIN_WEIGHT_VARIABLE, 0.75f))
                                        )
                                        .AddChild(
                                            // defence state
                                            new Sequence(environment)
                                                .AddChild(new IsHealthMoreThan(environment, aiTools, GameDefines.LOW_HP))
                                                .AddChild(new SetWeightFunction(environment, aiTools, TerrainReasoning.DEFENSIVE_WEIGHT_FUNCTION))
                                                .AddChild(new SetEnvironmentVariable<float>(environment, DESTINATION_SEARCH_RADIUS_VARIABLE, 5.0f))
                                                .AddChild(new SetEnvironmentVariable<float>(environment, DESTINATION_SEARCH_MIN_WEIGHT_VARIABLE, 0.75f))
                                        )
                                )
                        )
                        .AddChild(
                            new Selector(environment)
                                .AddChild(
                                        // patrol state
                                        new Sequence(environment)
                                            .AddChild(new IsHealthMoreThan(environment, aiTools, GameDefines.LOW_HP))
                                            .AddChild(new SetWeightFunction(environment, aiTools, TerrainReasoning.PATROL_WEIGHT_FUNCTION))
                                            .AddChild(new SetEnvironmentVariable<float>(environment, DESTINATION_SEARCH_RADIUS_VARIABLE, 500.0f))
                                            .AddChild(new SetEnvironmentVariable<float>(environment, DESTINATION_SEARCH_MIN_WEIGHT_VARIABLE, 0.0f))
                                )
                                .AddChild(
                                        // retreat state
                                        new Sequence(environment)
                                            .AddChild(new SetWeightFunction(environment, aiTools, TerrainReasoning.RETREAT_WEIGHT_FUNCTION))
                                            .AddChild(new SetEnvironmentVariable<float>(environment, DESTINATION_SEARCH_RADIUS_VARIABLE, 500.0f))
                                            .AddChild(new SetEnvironmentVariable<float>(environment, DESTINATION_SEARCH_MIN_WEIGHT_VARIABLE, -1.0f))
                                )
                        )
                );

                // navigation branch
                root.AddChild(
                    new Sequence(environment)
                        .AddChild(
                            new Selector(environment)
                                .AddChild(
                                    new Sequence(environment)
                                        .AddChild(new CheckEnvironmentVariable<bool>(environment, WEIGHT_FUNCTION_CHANGED_VARIABLE, true))
                                        .AddChild(new ClearEnvironmentVariable(environment, WEIGHT_FUNCTION_CHANGED_VARIABLE))
                                )
                                .AddChild(
                                    new Inverter(environment)
                                        .AddChild(new IsDestinationExistsAndValid(environment, aiTools))
                                )
                                .AddChild(new IsNearDestination(environment, aiTools))
                        )
                        .AddChild(new SelectDestinationPoint(environment, aiTools, DESTINATION_SEARCH_RADIUS_VARIABLE, DESTINATION_SEARCH_MIN_WEIGHT_VARIABLE, DESTINATION_POINT_VARIABLE))
                        .AddChild(new MoveToPoint(environment, aiTools, DESTINATION_POINT_VARIABLE))
                );
            }
        }
    }
}