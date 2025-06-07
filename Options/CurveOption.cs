namespace PhysicsEngineCore.Options{
    public class CurveOption : IOption {
        public string? id { get; set; }
        public string color { get; set; } = "#F00000";
        public double startX { get; set; }
        public double startY { get; set; }
        public double middleX { get; set; }
        public double middleY { get; set; }
        public double endX { get; set; }
        public double endY { get; set; }
        public double width { get; set; }
    }
}
