using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects{
    class Entity(string name, double posX, double posY, double velocityX, double velocityY, double rotate, double rotateSpeed, double radius, double mass, double stiffness, string parentName, Target[] targets){
        public readonly string name = name;
        public Vector2 position = new Vector2(posX, posY);
        public Vector2 previousPosition = new Vector2(posX, posY);
        public Vector2 velocity = new(velocityX, velocityY);
        public double rotate = rotate;
        public double rotateSpeed = rotateSpeed;
        public double radius = radius;
        public double mass = mass;
        private double _stiffness = CheckStiffnessValue(stiffness);
        public string parentName = parentName;
        public Target[] targets = targets;

        public double stiffness {
            get{
                return _stiffness;
            }
            set{
                _stiffness = CheckStiffnessValue(value);
            }
        }

        public double invMass {
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

        public void SavePosition(){
            this.previousPosition = this.position;
        }

        private static double CheckStiffnessValue(double stiffness){
            if(stiffness < 0|| stiffness >= 1) throw new Exception("剛性(stiffness)は0以上1以下に設定する必要があります");

            return stiffness;
        }
    }
}
