using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Options;

namespace PhysicsEngineCore.Utils{
    public class BaseObject{
        public List<Entity> entities = [];

        public BaseObject(List<EntityOption>? entities){
            this.AddAllEntities(entities ?? []);
        }

        protected bool isStop{
            get{
                return entities.All(entity => entity.isStop);
            }
        }

        protected Vector2 position{
            get{
                double averagePosX = entities.Average(entity => entity.position.X);
                double averagePosY = entities.Average(entity => entity.position.Y);

                return new Vector2(averagePosX, averagePosY);
            }
        }

        protected void AddEntity(EntityOption entity){
            entities.Add(new Entity(entity));
        }

        protected void AddAllEntities(List<EntityOption> entities){
            foreach(EntityOption entity in entities){
                this.AddEntity(entity);
            }
        }
    }
}
