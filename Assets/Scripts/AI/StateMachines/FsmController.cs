using System.Collections;
using System.Collections.Generic;
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

            [SerializeField]
            private WorldState world;

            private TerrainReasoning terrain;
            private Navigation navigation;
            private Shooting shooting;
            private SoldierController controller;
            private SoldierController enemy;

            private FiniteStateMachine fsm;

            // Use this for initialization
            void Awake()
            {
                terrain = GetComponent<TerrainReasoning>();
                navigation = GetComponent<Navigation>();
                shooting = GetComponent<Shooting>();
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
                SearchEnemy searchEnemyState = new SearchEnemy(world, terrain, navigation, shooting, controller);
                SearchHealthPack searchHealthPackState = new SearchHealthPack(world, terrain, navigation, shooting, controller);
                Attack attackState = new Attack(world, terrain, navigation, shooting, controller);
                Defence defenceState = new Defence(world, terrain, navigation, shooting, controller);

                searchEnemyState.AddTransition(
                    new Transition(
                        () => world.IsEnemySpotted(enemy.GetSide()),
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
                        () => (controller.GetHealth() > Defines.MEDIUM_HP) && !world.IsEnemySpotted(enemy.GetSide()),
                        searchEnemyState
                        )
                    );

                defenceState.AddTransition(
                    new Transition(
                        () => (controller.GetHealth() <= Defines.LOW_HP) || !world.IsEnemySpotted(enemy.GetSide()),
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