namespace PhysicsEngineCore.Objects{
    public interface IObject{
        string id { get; }
        string type { get; }
        string color { get; set; }

        IObject Clone();

        string ToJson();

        List<Entity> entities { get; }
    }
}
