using PhysicsEngineCore.Objects;

namespace PhysicsEngineCore.Utils{
    class Target(Entity entity, double distance, double stiffness){
        public readonly Entity entity = entity;
        public double distance = distance;
        public double stiffness = stiffness;
    }
}
