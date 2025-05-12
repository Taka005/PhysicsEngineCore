namespace PhysicsEngineCore.Options{
    class EngineOption{
        public int pps { get; set; }
        public double gravity { get; set; }
        public double friction { get; set; }
        public float playBackSpeed { get; set; }
        public float trackingInterval { get; set; }
        public int trackingLimit { get; set; }
        public int movementLimit { get; set; }
    }
}