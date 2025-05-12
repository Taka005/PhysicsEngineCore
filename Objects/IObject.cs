using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects{
    public interface IObject{
        double width { get; set; }

        Vector2 SolvePosition(Vector2 position);

        IGround Clone();

        public List<Entity> entities { get; set; }
    }
}
