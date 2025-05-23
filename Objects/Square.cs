using System.Text.Json;
using PhysicsEngineCore.Options;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects{
    public class Square : BaseObject, IObject{
        private readonly string _id;
        public double size;
        public double mass;
        public double stiffness;
        private string _color;

        public Square(SquareOption option) : base(option.entities) {
            this._id = option.id;
            this.size = option.size;
            this.mass = option.mass;
            this.stiffness = option.stiffness;
            this._color = option.color;

            if(option.entities.Count != 0){
                this.AddAllEntities(option.entities);
            }else{
                for(int i = -1;i <= 1;i += 2){
                    for(int j = -1;j <= 1;j += 2){
                        EntityOption entityOption = new EntityOption {
                            posX = option.posX + i * (this.size / 2),
                            posY = option.posY + j * (this.size / 2),
                            diameter = this.size/2,
                            mass = option.mass/4,
                            stiffness = option.stiffness,
                            velocityX = option.velocityX,
                            velocityY = option.velocityY,
                            parentId = this.id
                        };

                        this.AddEntity(entityOption);
                    }
                }

                this.entities.ForEach(source => {
                    this.entities.ForEach(target => {
                        if(source.id == target.id) return;

                        Vector2 difference = target.position - source.position;

                        double distance = difference.Length();

                        source.connection.Add(target, distance, source.stiffness);

                        target.connection.Add(source, distance, target.stiffness);
                    });
                });
            }
        }

        public string id {
            get {
                return _id;
            }
        }

        public string color {
            get {
                return this._color;
            }
            set {
                this._color = value;
            }
        }

        public string ToJson() {
            return JsonSerializer.Serialize(this.ToOption());
        }

        public IObject Clone() {
            return new Square(this.ToOption());
        }

        public SquareOption ToOption() {
            return new SquareOption {
                id = this.id,
                posX = this.position.X,
                posY = this.position.Y,
                velocityX = this.velocity.X,
                velocityY = this.velocity.Y,
                size = this.size,
                mass = this.mass,
                stiffness = this.stiffness,
                color = this.color,
                entities = [.. this.entities.Select(entity => entity.ToOption())]
            };
        }
    }
}
