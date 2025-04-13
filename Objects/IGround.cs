namespace PhysicsEngineCore.Objects{
    interface IGround{
        public string name { get; }

        public string type { get; }

        public (float posX, float posY) solvePosition(float posX, float posY);
    }
}
