using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects{
    public class Circle: BaseObject{
        public readonly string type = "circle";
        public readonly string name;
        public double radius;
        public double mass;
        public double stiffness;
        public string color;

        public Circle(string type, string name, double posX, double posY, double radius, double mass, double stiffness, string color, double velocityX, double velocityY, List<Entity>? entities = null): base(entities){
            this.type = type;
            this.name = name;
            this.radius = radius;
            this.mass = mass;
            this.stiffness = stiffness;
            this.color = color;

            if(entities != null){
                this.AddAllEntities(entities);
            }else{

            }
        }
    }
}
