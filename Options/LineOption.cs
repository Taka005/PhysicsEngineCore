using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Options{
    public class LineOption{
        public string id { get; set; } = IdGenerator.CreateId(10);
        public string color { get; set; } = "red";
        public double startX { get; set; }
        public double startY { get; set; }
        public double endX { get; set; }
        public double endY { get; set; }
        public double width { get; set; }
    }
}
