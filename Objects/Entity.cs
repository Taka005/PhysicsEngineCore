using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects{
    class Entity(string name, double posX, double posY, double velocityX, double velocityY, double rotateAngle, double rotateSpeed, double radius, double mass, double stiffness, string parentName){
        public readonly string name = name;
        public Vector2 position = new Vector2(posX, posY);
        public Vector2 previousPosition = new Vector2(posX, posY);
        public Vector2 velocity = new Vector2(velocityX, velocityY);
        public double rotateAngle = rotateAngle;
        public double rotateSpeed = rotateSpeed;
        public double _radius = CheckRadiusValue(radius);
        public double _mass = CheckMassValue(mass);
        private double _stiffness = CheckStiffnessValue(stiffness);
        public string parentName = parentName;

        public double radius{
            get {
                return _radius;
            }
            set {
                _radius = CheckRadiusValue(value);
            }
        }

        public double mass{
            get {
                return _mass;
            }
            set {
                _mass = CheckMassValue(value);
            }
        }

        public double stiffness{
            get{
                return _stiffness;
            }
            set{
                _stiffness = CheckStiffnessValue(value);
            }
        }

        public double invMass{
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

        private static double CheckRadiusValue(double radius){
            if(radius < 0) throw new Exception("半径(radius)は0以上に設定する必要があります");

            return radius;
        }

        private static double CheckMassValue(double mass){
            if(mass < 0) throw new Exception("質量(mass)は0以上に設定する必要があります");

            return mass;
        }
    }
}
