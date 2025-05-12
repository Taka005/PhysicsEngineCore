namespace PhysicsEngineCore.Options{
    class EngineOption{
        public int pps { get; set; } = 180;
        public double gravity { get; set; } = 500;
        public double friction { get; set; } = 0.0001;
        public float playBackSpeed { get; set; } = 1;
        public float trackingInterval { get; set; } = 100;
        public int trackingLimit { get; set; } = 10000;
        public int movementLimit { get; set; } = 10000;
    }
}