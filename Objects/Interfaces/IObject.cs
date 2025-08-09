using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects.Interfaces {
    public interface IObject : IBaseObject {
        string id { get; set; }
        string trackingId { get; }
        string color { get; set; }
        string? imageName { get; }
        Image? image { get; set; }

        IObject Clone();

        bool Equals(IObject target);

        string ToJson();
    }
}
