
namespace Ai
{

    namespace Fsm
    {

        public abstract class SoldierState : State
        {
            protected readonly WorldState world;
            protected readonly TerrainReasoning terrain;
            protected readonly Navigation navigation;
            protected readonly SoldierController controller;

            public SoldierState(WorldState world, TerrainReasoning terrain, Navigation navigation, SoldierController controller)
            {
                this.world = world;
                this.terrain = terrain;
                this.navigation = navigation;
                this.controller = controller;
            }
        }

    }
}