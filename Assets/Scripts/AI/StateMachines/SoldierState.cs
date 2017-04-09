
namespace Ai
{

    namespace Fsm
    {

        public abstract class SoldierState : State
        {
            private WorldState world;
            private TerrainReasoning terrain;
            private SoldierController controller;

            public SoldierState(WorldState world, TerrainReasoning terrain, SoldierController controller)
            {
                this.world = world;
                this.terrain = terrain;
                this.controller = controller;
            }
        }

    }
}