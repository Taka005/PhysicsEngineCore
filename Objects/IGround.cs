using System.Numerics;

namespace PhysicsEngineCore.Objects{
    interface IGround{
        public string name { get; }

        public string type { get; }

        public Vector2 solvePosition(Vector2 position);

        public IGround clone();
    }
}
