using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects{
    public interface IGround{
        double thickness { get; set; }

        Vector2 SolvePosition(Vector2 position);

        IGround Clone();
    }
}
