using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects{
    public interface IBaseObject{
        List<Entity> entities { get; }
        bool isStop { get; }
        Vector2 position { get; }
    }
}
