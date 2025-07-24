namespace PhysicsEngineCore.Objects {
    public interface IEffect {
        string id { get; }
        string trackingId { get; }
        string color { get; set; }

        void SetEffect(Entity entity);

        IEffect Clone();

        bool Equals(IEffect target);

        string ToJson();
    }
}
