namespace PhysicsEngineCore.Options{
    public class CircleOption(): IOption{
        public string? id { get; set; }
        public double posX { get; set; }
        public double posY { get; set; }
        public double diameter { get; set; }
        public double mass { get; set; }
        public double stiffness { get; set; }
        public double velocityX { get; set; } = 0;
        public double velocityY { get; set; } = 0;
        public string color { get; set; } = "#F00000";
        public string? script { get; set; } = null;
        public List<EntityOption> entities { get; set; } = [];
    }
}