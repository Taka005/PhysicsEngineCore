using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects.Interfaces {
    public interface IGround {
        string id { get; }
        string trackingId { get; }
        string color { get; set; }
        double width { get; set; }
        string? imageName { get; }
        Image? image { get; set; }

        Vector2 SolvePosition(Vector2 position);

        IGround Clone();

        bool Equals(IGround target);

        string ToJson();
    }
}
