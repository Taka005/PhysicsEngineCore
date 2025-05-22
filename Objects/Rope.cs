using System.Text.Json;
using PhysicsEngineCore.Options;

namespace PhysicsEngineCore.Objects{
    public class Rope: BaseObject, IObject{
        private readonly string _id;
        public double width;
        public double mass;
        public double stiffness;
        private string _color;

        public Rope(RopeOption option): base(option.entities){
            this._id = option.id;
            this.width = option.width;
            this.mass = option.mass;
            this.stiffness = option.stiffness;
            this._color = option.color;

            if(option.entities.Count != 0) {
                this.AddAllEntities(option.entities);
            } else {
                double width = option.endX - option.startX;
                double height = option.endY - option.startY;

                double count = Math.Floor(Math.Sqrt(width * width + height * height) / (this.width * 2));

                Entity? entity = null;
                for(int i = 0;i <= count;i++) {
                    double posX = option.startX + i * (width / count);
                    double posY = option.startY + i * (height / count);

                    EntityOption entityOption = new EntityOption {
                        posX = posX,
                        posY = posY,
                        radius = option.width / 2,
                        mass = option.mass,
                        stiffness = option.stiffness,
                        velocityX = option.velocityX,
                        velocityY = option.velocityY,
                        parentId = this.id
                    };

                    Entity target = this.AddEntity(entityOption);

                    if(entity != null){
                        entity.connection.Add(target, this.width * 2, this.stiffness);

                        target.connection.Add(entity, this.width * 2, this.stiffness);
                    }

                    entity = target;
                }
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

        public string ToJson(){
            return JsonSerializer.Serialize(this.ToOption());
        }

        public IObject Clone(){
            return new Rope(this.ToOption());
        }

        public RopeOption ToOption(){
            return new RopeOption {
                id = this.id,
                startX = this.entities.First().position.X,
                startY = this.entities.First().position.Y,
                endX = this.entities.Last().position.X,
                endY = this.entities.Last().position.Y,
                velocityX = this.velocity.X,
                velocityY = this.velocity.Y,
                width = this.width,
                mass = this.mass,
                stiffness = this.stiffness,
                color = this.color,
                entities = [..this.entities.Select(entity =>entity.ToOption())]
            };
        }
    }
}
