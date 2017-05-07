namespace Ai
{
    namespace Bt
    {
        public class SetWeightFunction : SoldierNode
        {
            private TerrainReasoning.WeightFunction function;

            public SetWeightFunction(Environment environment, AiTools aiTools, TerrainReasoning.WeightFunction function) : base(environment, aiTools)
            {
                this.function = function;
            }

            public override Result Run()
            {
                if (aiTools.terrain.GetWeightFunction() != function)
                {
                    aiTools.terrain.SetWeightFunction(function);
                    environment.SetValue("weightFunctionChanged", true);
                }
                return Result.Success;
            }
        }
    }
}