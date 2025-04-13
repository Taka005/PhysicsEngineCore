using System.Numerics;

namespace PhysicsEngineCore.Objects{
    interface IGround{
        float thickness { get; set; }

        Vector2 SolvePosition(Vector2 position);

        IGround Clone();
    }
}
