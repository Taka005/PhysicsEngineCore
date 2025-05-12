using System.Text.Json;
using PhysicsEngineCore.Options;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects{
    public class Entity(EntityOption option){
        public readonly string id = option.id ?? throw new ArgumentException(nameof(option.id));
        public Vector2 position = new Vector2(option.posX, option.posY);
        public Vector2 previousPosition = new Vector2(option.prePosX, option.prePosY);
        public Vector2 velocity = new Vector2(option.velocityX, option.velocityY);
        public double rotateAngle = option.rotateAngle;
        public double rotateSpeed = option.rotateSpeed;
        private double _radius = CheckRadiusValue(option.radius);
        private double _mass = CheckMassValue(option.mass);
        private double _stiffness = CheckStiffnessValue(option.stiffness);
        public ConnectionManager connection = new ConnectionManager(option.targets);
        public string parentId = option.parentId ?? throw new ArgumentException(nameof(option.parentId));

        public double radius{
            get{
                return _radius;
            }
            set{
                _radius = CheckRadiusValue(value);
            }
        }

        public double mass{
            get{
                return _mass;
            }
            set{
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

        public string ToJson(){
            return JsonSerializer.Serialize(this.ToOption());
        }

        private EntityOption ToOption(){
            return new EntityOption{
                id = this.id,
                posX = this.position.X,
                posY = this.position.Y,
                prePosX = this.previousPosition.X,
                prePosY = this.previousPosition.Y,
                mass = this.mass,
                stiffness = this.stiffness,
                radius = this.radius,
                velocityX = this.velocity.X,
                velocityY = this.velocity.Y,
                rotateAngle = this.rotateAngle,
                rotateSpeed = this.rotateSpeed,
                parentId = this.parentId
            };
        }

        private static double CheckStiffnessValue(double stiffness){
            if(stiffness < 0|| stiffness >= 1) throw new Exception("剛性(stiffness)は0超過かつ1以下に設定する必要があります");

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
