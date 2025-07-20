namespace PhysicsEngineCore.Objects {
    public interface IObject : IBaseObject {
        string id { get; }
        string color { get; set; }

        IObject Clone();

        bool Equals(IObject target);

        string ToJson();
    }
}
