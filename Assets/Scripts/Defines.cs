public class GameDefines
{
    public enum Side
    {
        Blue,
        Red
    }

    public enum ControllerType
    {
        FiniteStateMachine,
        BehaviorTree,
        FuzzyLogic,
        NeuralNetwork
    }

    public const float MAP_SIDE_SIZE = 30.0f;
    public const float MAP_WIDTH = MAP_SIDE_SIZE;
    public const float MAP_HEIGHT = MAP_SIDE_SIZE;

    public const float NEAR_POINT_CHECK_PRECISION = 2.0f;

    public const float FULL_HP = 100.0f;
    public const float MEDIUM_HP = 70.0f;
    public const float LOW_HP = 50.0f;

    public static Side OpposingTo(Side side)
    {
        if (side == Side.Blue)
        {
            return Side.Red;
        }
        return Side.Blue;
    }
}
