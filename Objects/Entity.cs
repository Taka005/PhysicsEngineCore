using System.Numerics;

namespace PhysicsEngineCore.Objects
{
    class Entity(string name, float posX, float posY, float speedX, float speedY, float routate, float rotateSpeed, float size, float mass, string parent)
    {
        public readonly string name = name;
        public Vector2 position = new Vector2(posX, posY);
        public Vector2 velocity = new Vector2(speedX, speedY);
        public float routate = routate;
        public float rotateSpeed = rotateSpeed;
        public float size = size;
        public float mass = mass;
        public string parent = parent;

        public float InvMass
        {
            get
            {
                if(mass == 0) return 0;

                return 1 / mass;
            }
        }
    }
}
