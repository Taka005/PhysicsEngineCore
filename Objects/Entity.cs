using System.Numerics;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects
{
    class Entity(string name, float posX, float posY, float velocityX, float velocityY, float rotate, float rotateSpeed, float radius, float mass, float stiffness, string parentName, Target[] targets)
    {
        public readonly string name = name;
        public Vector2 position = new(posX, posY);
        public Vector2 previousPosition = new(posX, posY);
        public Vector2 velocity = new(velocityX, velocityY);
        public float rotate = rotate;
        public float rotateSpeed = rotateSpeed;
        public float radius = radius;
        public float mass = mass;
        private float _stiffness = clamp(stiffness, 0, 1);
        public string parentName = parentName;
        public Target[] targets = targets;

        public float stiffness
        {
            get => _stiffness;
            set => _stiffness = clamp(value, 0, 1);
        }

        public float invMass
        {
            get
            {
                if (mass == 0) return 0;

                return 1 / mass;
            }
        }

        public bool isStop
        {
            get
            {
                return position.Equals(previousPosition);
            }
        }

        public void savePosition()
        {
            previousPosition = position;
        }

        private static float clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;

            return value;
        }
    }
}
