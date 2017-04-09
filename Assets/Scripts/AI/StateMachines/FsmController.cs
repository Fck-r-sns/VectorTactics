using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    namespace Fsm
    {

        [RequireComponent(typeof(TerrainReasoning))]
        [RequireComponent(typeof(Navigation))]
        [RequireComponent(typeof(SoldierController))]
        public class FsmController : MonoBehaviour
        {

            [SerializeField]
            private WorldState world;

            private TerrainReasoning terrain;
            private Navigation navigation;
            private SoldierController controller;
            private SoldierController enemy;

            private FiniteStateMachine fsm;

            // Use this for initialization
            void Awake()
            {
                terrain = GetComponent<TerrainReasoning>();
                navigation = GetComponent<Navigation>();
                controller = GetComponent<SoldierController>();
                enemy = (world.GetBlueSoldier() == controller) ? world.GetRedSoldier() : world.GetBlueSoldier();
                    
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
                SearchEnemy searchEnemyState = new SearchEnemy(world, terrain, navigation, controller);
                SearchHealthPack searchHealthPackState = new SearchHealthPack(world, terrain, navigation, controller);
                Attack attackState = new Attack(world, terrain, navigation, controller);
                Defence defenceState = new Defence(world, terrain, navigation, controller);

                searchEnemyState.AddTransition(
                    new Transition(
                        () => world.IsEnemySpotted(),
                        attackState
                        )
                    );

                searchEnemyState.AddTransition(
                    new Transition(
                        () => controller.GetHealth() <= Defines.LOW_HP,
                        searchHealthPackState
                        )
                    );

                attackState.AddTransition(
                    new Transition(
                        () => (controller.GetHealth() <= Defines.MEDIUM_HP),
                        defenceState
                        )
                    );

                attackState.AddTransition(
                    new Transition(
                        () => (controller.GetHealth() > Defines.MEDIUM_HP) && !world.IsEnemySpotted(),
                        searchEnemyState
                        )
                    );

                defenceState.AddTransition(
                    new Transition(
                        () => (controller.GetHealth() <= Defines.LOW_HP) || !world.IsEnemySpotted(),
                        searchHealthPackState
                        )
                    );

                searchHealthPackState.AddTransition(
                    new Transition(
                        () => controller.GetHealth() > Defines.LOW_HP,
                        searchEnemyState
                        )
                    );

                return searchEnemyState;
            }
        }

    }
}