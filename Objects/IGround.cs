using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects {
    public interface IGround {
        string id { get; }
        string color { get; set; }
        double width { get; set; }

        Vector2 SolvePosition(Vector2 position);

        IGround Clone();

        string ToJson();
    }
}
