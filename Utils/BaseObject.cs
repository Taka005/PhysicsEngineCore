using PhysicsEngineCore.Objects;

namespace PhysicsEngineCore.Utils{
    public class BaseObject(List<Entity>? entities){
        public List<Entity> entities = entities ?? [];

        protected bool isStop{
            get{
                return entities.All(entity => entity.isStop);
            }
        }

        protected Vector2 position{
            get{
                double averagePosX = entities.Average(entity => entity.position.X);
                double averatePosY = entities.Average(entity => entity.position.Y);

                return new Vector2(averagePosX, averatePosY);
            }
        }
    }
}
