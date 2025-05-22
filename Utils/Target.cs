namespace PhysicsEngineCore.Utils{
    public class Target(string entityId, double distance, double stiffness){
        public readonly string entityId = entityId;
        public double distance = distance;
        public double stiffness = stiffness;
    }
}