using System.Text.Json;
using PhysicsEngineCore.Options;

namespace PhysicsEngineCore.Objects{
    public class Circle: BaseObject, IObject{
        private readonly string _id;
        public double diameter;
        public double mass;
        public double stiffness;
        private string _color;

        public Circle(CircleOption option): base(option.entities){
            this._id = option.id;
            this.diameter = option.diameter;
            this.mass = option.mass;
            this.stiffness = option.stiffness;
            this._color = option.color;

            if(option.entities.Count != 0){
                this.AddAllEntities(option.entities);
            }else{
                EntityOption entityOption = new EntityOption{
                    posX = option.posX,
                    posY = option.posY,
                    diameter = this.diameter,
                    mass = option.mass,
                    stiffness = option.stiffness,
                    velocityX = option.velocityX,
                    velocityY = option.velocityY,
                    parentId = this.id
                };

                this.AddEntity(entityOption);
            }
        }

        public string id{
            get{
                return _id;
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

        public double radius{
            get{
                return this.diameter/2;
            }
        }

        public string ToJson(){
            return JsonSerializer.Serialize(this.ToOption());
        }

        public IObject Clone(){
            return new Circle(this.ToOption());
        }

        public CircleOption ToOption(){
            return new CircleOption {
                id = this.id,
                posX = this.position.X,
                posY = this.position.Y,
                velocityX = this.velocity.X,
                velocityY = this.velocity.Y,
                diameter = this.diameter,
                mass = this.mass,
                stiffness = this.stiffness,
                color = this.color,
                entities = [..this.entities.Select(entity =>entity.ToOption())]
            };
        }
    }
}
