using PhysicsEngineCore.Options;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects{
    public class BaseObject: IBaseObject{
        private List<Entity> _entities = [];

        public List<Entity> entities{
            get{
                return _entities;
            }
        }

        public BaseObject(List<EntityOption>? entities){
            this.AddAllEntities(entities ?? []);
        }

        public bool isStop{
            get{
                return _entities.All(entity => entity.isStop);
            }
        }

        public Vector2 position{
            get{
                double averagePosX = _entities.Average(entity => entity.position.X);
                double averagePosY = _entities.Average(entity => entity.position.Y);

                return new Vector2(averagePosX, averagePosY);
            }
        }

        protected void AddEntity(EntityOption entity){
            this._entities.Add(new Entity(entity));
        }

        protected void AddAllEntities(List<EntityOption> entities){
            foreach(EntityOption entity in entities){
                this.AddEntity(entity);
            }
        }
    }
}
