namespace PhysicsEngineCore.Options{
    public class BoosterOption : IOption {
        public string? id { get; set; }
        public string color { get; set; } = "#F00000";
        public double startX { get; set; }
        public double startY { get; set; }
        public double endX { get; set; }
        public double endY { get; set; }
        public double velocityX { get; set; }
        public double velocityY { get; set; }
    }
}
