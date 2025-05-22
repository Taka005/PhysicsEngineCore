using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Options{
    public class CircleOption(){
        public string id { get; set; } = IdGenerator.CreateId(10);
        public double posX { get; set; }
        public double posY { get; set; }
        public double diameter { get; set; }
        public double mass { get; set; }
        public double stiffness { get; set; }
        public double velocityX { get; set; } = 0;
        public double velocityY { get; set; } = 0;
        public string color { get; set; } = "red";
        public List<EntityOption> entities { get; set; } = [];
    }
}