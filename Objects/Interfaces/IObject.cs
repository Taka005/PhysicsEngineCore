namespace PhysicsEngineCore.Objects.Interfaces {
    public interface IObject : IBaseObject {
        string id { get; }
        string trackingId { get; }
        string color { get; set; }

        IObject Clone();

        bool Equals(IObject target);

        string ToJson();
    }
}
