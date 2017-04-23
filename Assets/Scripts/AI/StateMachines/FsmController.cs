using UnityEngine;

namespace Ai
{
    namespace Fsm
    {

        [RequireComponent(typeof(TerrainReasoning))]
        [RequireComponent(typeof(Navigation))]
        [RequireComponent(typeof(Shooting))]
        [RequireComponent(typeof(SoldierController))]
        public class FsmController : MonoBehaviour
        {

            private WorldState world;
            private Navigation navigation;
            private Shooting shooting;
            private TerrainReasoning terrain;
            private SoldierController controller;
            private CharacterState agentState;
            private CharacterState enemyState;

            private FiniteStateMachine fsm;

            // Use this for initialization
            void Start()
            {
                world = WorldState.GetInstance();
                navigation = GetComponent<Navigation>();
                shooting = GetComponent<Shooting>();
                terrain = GetComponent<TerrainReasoning>();

                controller = GetComponent<SoldierController>();
                agentState = controller.GetState();
                enemyState = agentState.enemyState;
                    
                fsm = new FiniteStateMachine();
                State initialState = InitStates();
                fsm.SetInitialState(initialState);
            }

            void Update()
            {
                fsm.Update();
            }

            // returns initial state
            private State InitStates()
            {
                AiTools aiTools = new AiTools(world, navigation, shooting, terrain, controller, agentState, enemyState);
                SearchEnemy searchEnemyState = new SearchEnemy(aiTools);
                SearchHealthPack searchHealthPackState = new SearchHealthPack(aiTools);
                Attack attackState = new Attack(aiTools);
                Defence defenceState = new Defence(aiTools);

                searchEnemyState.AddTransition(
                    new Transition(
                        () => agentState.isEnemyVisible,
                        attackState
                        )
                    );

                searchEnemyState.AddTransition(
                    new Transition(
                        () => (world.healthPacksAvailable > 0) && (agentState.health <= GameDefines.LOW_HP),
                        searchHealthPackState
                        )
                    );

                attackState.AddTransition(
                    new Transition(
                        () => (agentState.health <= GameDefines.MEDIUM_HP),
                        defenceState
                        )
                    );

                attackState.AddTransition(
                    new Transition(
                        () => (agentState.health > GameDefines.MEDIUM_HP) && !agentState.isEnemyVisible,
                        searchEnemyState
                        )
                    );

                defenceState.AddTransition(
                    new Transition(
                        () => (world.healthPacksAvailable > 0) && ((agentState.health <= GameDefines.LOW_HP) || !agentState.isEnemyVisible),
                        searchHealthPackState
                        )
                    );

                searchHealthPackState.AddTransition(
                    new Transition(
                        () => (agentState.health > GameDefines.LOW_HP) || (world.healthPacksAvailable == 0),
                        searchEnemyState
                        )
                    );

                return searchEnemyState;
            }
        }

    }
}