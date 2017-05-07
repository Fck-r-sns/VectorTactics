using UnityEngine;

namespace Ai
{
    namespace Fsm
    {

        [RequireComponent(typeof(AiTools))]
        public class FsmController : MonoBehaviour
        {

            private FiniteStateMachine fsm;

            // Use this for initialization
            void Start()
            {
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
                AiTools aiTools = GetComponent<AiTools>();
                aiTools.Init();
                SearchEnemy searchEnemyState = new SearchEnemy(aiTools);
                SearchHealthPack searchHealthPackState = new SearchHealthPack(aiTools);
                Attack attackState = new Attack(aiTools);
                Defence defenceState = new Defence(aiTools);

                searchEnemyState.AddTransition(
                    new Transition(
                        () => aiTools.agentState.isEnemyVisible,
                        attackState
                        )
                    );

                searchEnemyState.AddTransition(
                    new Transition(
                        () => (aiTools.world.healthPacksAvailable > 0) && (aiTools.agentState.health <= GameDefines.LOW_HP),
                        searchHealthPackState
                        )
                    );

                attackState.AddTransition(
                    new Transition(
                        () => (aiTools.agentState.health <= GameDefines.MEDIUM_HP),
                        defenceState
                        )
                    );

                attackState.AddTransition(
                    new Transition(
                        () => (aiTools.agentState.health > GameDefines.MEDIUM_HP) && !aiTools.agentState.isEnemyVisible,
                        searchEnemyState
                        )
                    );

                defenceState.AddTransition(
                    new Transition(
                        () => (aiTools.world.healthPacksAvailable > 0) && ((aiTools.agentState.health <= GameDefines.LOW_HP) || !aiTools.agentState.isEnemyVisible),
                        searchHealthPackState
                        )
                    );

                searchHealthPackState.AddTransition(
                    new Transition(
                        () => (aiTools.agentState.health > GameDefines.LOW_HP) || (aiTools.world.healthPacksAvailable == 0),
                        searchEnemyState
                        )
                    );

                return searchEnemyState;
            }
        }

    }
}