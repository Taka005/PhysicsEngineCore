using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Options{
    public class EntityOption(){
        public string? id { get; set; }
        public double posX { get; set; }
        public double posY { get; set; }
        public double radius { get; set; }
        public double mass { get; set; }
        public double stiffness { get; set; }
        public string? parentName { get; set; }
        public double velocityX { get; set; } = 0;
        public double velocityY { get; set; } = 0;
        public double rotateAngle { get; set; } = 0;
        public double rotateSpeed { get; set; } = 0;
        public List<Target> targets { get; set; } = [];
    }
}