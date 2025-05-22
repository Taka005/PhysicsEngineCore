using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Options{
    public class RopeOption(){
        public string id { get; set; } = IdGenerator.CreateId(10);
        public double startX { get; set; }
        public double startY { get; set; }
        public double endX { get; set; }
        public double endY { get; set; }
        public double width { get; set; }
        public double mass { get; set; }
        public double stiffness { get; set; }
        public double velocityX { get; set; } = 0;
        public double velocityY { get; set; } = 0;
        public string color { get; set; } = "red";
        public List<EntityOption> entities { get; set; } = [];
    }
}