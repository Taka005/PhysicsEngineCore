﻿using PhysicsEngineCore.Options;
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

        public bool isStop{
            get{
                return _entities.All(entity => entity.isStop);
            }
        }

        public Vector2 position{
            get{
                if(_entities.Count == 0) return Vector2.Zero;

                double averagePosX = _entities.Select(entity => entity.position.X).Average();
                double averagePosY = _entities.Select(entity => entity.position.Y).Average();

                return new Vector2(averagePosX, averagePosY);
            }
        }

        public Vector2 velocity{
            get{
                if(_entities.Count == 0) return Vector2.Zero;

                double averageVelocityX = _entities.Select(entity => entity.velocity.X).Average();
                double averageVelocityY = _entities.Select(entity => entity.velocity.Y).Average();

                return new Vector2(averageVelocityX, averageVelocityY);
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
