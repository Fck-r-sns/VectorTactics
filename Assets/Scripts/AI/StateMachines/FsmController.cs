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
                AiTools aiTools = new AiTools(navigation, shooting, terrain, controller, agentState, enemyState);
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
                        () => agentState.health <= Defines.LOW_HP,
                        searchHealthPackState
                        )
                    );

                attackState.AddTransition(
                    new Transition(
                        () => (agentState.health <= Defines.MEDIUM_HP),
                        defenceState
                        )
                    );

                attackState.AddTransition(
                    new Transition(
                        () => (agentState.health > Defines.MEDIUM_HP) && !agentState.isEnemyVisible,
                        searchEnemyState
                        )
                    );

                defenceState.AddTransition(
                    new Transition(
                        () => (agentState.health <= Defines.LOW_HP) || !agentState.isEnemyVisible,
                        searchHealthPackState
                        )
                    );

                searchHealthPackState.AddTransition(
                    new Transition(
                        () => agentState.health > Defines.LOW_HP,
                        searchEnemyState
                        )
                    );

                return searchEnemyState;
            }
        }

    }
}