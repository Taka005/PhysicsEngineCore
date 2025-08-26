using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects.Interfaces {
    public interface IEffect {
        string id { get; set; }
        string trackingId { get; }
        string color { get; set; }
        Image? image { get; set; }

        void SetEffect(Entity entity,double deltaTime);

        IEffect Clone();

        bool Equals(IEffect target);

        string ToJson();
    }
}
