using System.Text.Json;
using PhysicsEngineCore.Options;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects{
    public class Circle: BaseObject, IObject{
        private readonly string _type = "circle";
        private readonly string _id;
        public double radius;
        public double mass;
        public double stiffness;
        private string _color;

        public Circle(CircleOption option): base(option.entities){
            this._type = option.type;
            this._id = option.id ?? throw new ArgumentException(nameof(option.id));
            this.radius = option.radius;
            this.mass = option.mass;
            this.stiffness = option.stiffness;
            this._color = option.color;

            if(option.entities != null){
                this.AddAllEntities(option.entities);
            }else{
                EntityOption entityOption = new EntityOption{
                    id = IdGenerator.CreateId(10),
                    posX = option.posX,
                    posY = option.posY,
                    radius = option.radius,
                    mass = option.mass,
                    stiffness = option.stiffness,
                    velocityX = option.velocityX,
                    velocityY = option.velocityY,
                };

                this.AddEntity(entityOption);
            }
        }

        public List<Entity> entities => base.entities;

        public string id{
            get{
                return _id;
            }
        }

        public string type{
            get {
                return _type;
            }
        }

        public string color{
            get{
                return this._color;
            }
            set{
                this._color = value;
            }
        }

        public string ToJson(){
            return JsonSerializer.Serialize(this.ToOption());
        }

        public IObject Clone(){
            return new Circle(this.ToOption());
        }

        private CircleOption ToOption(){
            return new CircleOption {
                id = this.id,
                radius = this.radius,
                mass = this.mass,
                stiffness = this.stiffness,
                color = this.color
            };
        }
    }
}
