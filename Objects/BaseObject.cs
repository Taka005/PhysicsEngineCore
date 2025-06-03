using PhysicsEngineCore.Options;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects{
    public class BaseObject: IBaseObject{
        private readonly List<Entity> _entities = [];

        public BaseObject(List<EntityOption>? entities){
            this.AddAllEntities(entities ?? []);
        }

        public List<Entity> entities {
            get {
                return _entities;
            }
        }

        public int count{
            get{
                return this._entities.Count;
            }
        }

        public bool isStop{
            get{
                return _entities.All(entity => entity.isStop);
            }
        }

        public Vector2 position{
            get{
                if(this.count == 0) return Vector2.Zero;

                double averagePosX = this._entities.Select(entity => entity.position.X).Average();
                double averagePosY = this._entities.Select(entity => entity.position.Y).Average();

                return new Vector2(averagePosX, averagePosY);
            }
        }

        public Vector2 velocity{
            get{
                if(this.count == 0) return Vector2.Zero;

                double averageVelocityX = this._entities.Select(entity => entity.velocity.X).Average();
                double averageVelocityY = this._entities.Select(entity => entity.velocity.Y).Average();

                return new Vector2(averageVelocityX, averageVelocityY);
            }
            set{
                this._entities.ForEach(entity=>{
                    entity.velocity = value;
                });
            }
        }

        public double mass{
            get{
                if(this.count == 0) return 0;

                return this._entities.Select(entity => entity.mass).Average();
            }
            set{
                if(value < 0) throw new Exception("質量(mass)は0以上に設定する必要があります");

                this._entities.ForEach(entity=>{
                    entity.mass = value / this.count;
                });
            }
        }

        public double stiffness{
            get{
                if(this.count == 0) return 0;

                return _entities.Select(entity => entity.stiffness).Average();
            }
            set{
                if(value < 0 || value > 1) throw new Exception("剛性(stiffness)は0超過かつ1以下に設定する必要があります");

                this._entities.ForEach(entity=>{
                    entity.stiffness = value / this.count;
                });
            }
        }

        protected Entity AddEntity(EntityOption entityOption){
            Entity entity = new Entity(entityOption);

            this._entities.Add(entity);

            return entity;
        }

        protected void AddAllEntities(List<EntityOption> entities){
            entities.ForEach(entityOption => this.AddEntity(entityOption));
        }
    }
}
