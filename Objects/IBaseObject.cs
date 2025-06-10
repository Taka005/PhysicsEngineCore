using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects{
    public interface IBaseObject{
        List<Entity> entities { get; }
        bool isStop { get; }
        int count { get; }
        Vector2 position { get; }
        Vector2 velocity { get; set; }
        double mass {  get; set; }
        double stiffness { get; set; }
        string? script { get; set; }
    }
}
