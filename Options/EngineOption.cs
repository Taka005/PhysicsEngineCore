namespace PhysicsEngineCore.Options{
    public class EngineOption{
        public int pps { get; set; } = 180;
        public double gravity { get; set; } = 100;
        public double friction { get; set; } = 0;
        public float playBackSpeed { get; set; } = 1;
        public float trackingInterval { get; set; } = 100;
        public float scriptExecutionInterval { get; set; } = 1000;
        public int trackingLimit { get; set; } = 100;
        public int movementLimit { get; set; } = 10000;
    }
}