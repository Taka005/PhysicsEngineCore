using System.Numerics;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects{
    class Entity(string name, float posX, float posY, float velocityX, float velocityY, float rotate, float rotateSpeed, float radius, float mass, float stiffness, string parentName, Target[] targets){
        public readonly string name = name;
        public Vector2 position = new Vector2(posX, posY);
        public Vector2 previousPosition = new Vector2(posX, posY);
        public Vector2 velocity = new(velocityX, velocityY);
        public float rotate = rotate;
        public float rotateSpeed = rotateSpeed;
        public float radius = radius;
        public float mass = mass;
        private float _stiffness = checkStiffnessValue(stiffness);
        public string parentName = parentName;
        public Target[] targets = targets;

        public float stiffness{
            get{
                return _stiffness;
            }
            set{
                _stiffness = checkStiffnessValue(value);
            }
        }

        public float invMass{
            get{
                if(this.mass == 0) return 0;

                return 1 / this.mass;
            }
        }

        public bool isStop{
            get{
                return position.Equals(previousPosition);
            }
        }

        public void savePosition(){
            this.previousPosition = this.position;
        }

        private static float checkStiffnessValue(float stiffness){
            if(stiffness < 0|| stiffness >= 1) throw new Exception("剛性(stiffness)は0以上1以下に設定する必要があります");

            return stiffness;
        }
    }
}
