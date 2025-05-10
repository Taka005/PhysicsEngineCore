using PhysicsEngineCore.Options;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects{
    public class Circle: BaseObject{
        public readonly string type = "circle";
        public readonly string id;
        public double radius;
        public double mass;
        public double stiffness;
        public string color;

        public Circle(CircleOption option): base(option.entities){
            this.type = option.type;
            this.id = option.id ?? throw new ArgumentException(nameof(option.id));
            this.radius = option.radius;
            this.mass = option.mass;
            this.stiffness = option.stiffness;
            this.color = option.color;

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

                Entity entity = new Entity(entityOption);

                this.AddEntity(entity);
            }
        }
    }
}
