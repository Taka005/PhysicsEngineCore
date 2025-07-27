namespace PhysicsEngineCore.Objects.Interfaces {
    public interface IEffect {
        string id { get; }
        string trackingId { get; }
        string color { get; set; }

        void SetEffect(Entity entity,double deltaTime);

        IEffect Clone();

        bool Equals(IEffect target);

        string ToJson();
    }
}
