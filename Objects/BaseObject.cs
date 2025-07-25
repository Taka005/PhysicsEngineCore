﻿using PhysicsEngineCore.Objects.Interfaces;
using PhysicsEngineCore.Options;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects {
    /// <summary>
    /// オブジェクトの基礎クラス
    /// </summary>
    public class BaseObject : IBaseObject {
        private readonly List<Entity> _entities = [];

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="entities">全てのエンティティーのリスト</param>
        public BaseObject(List<EntityOption>? entities) {
            this.AddAllEntities(entities ?? []);
        }

        /// <summary>
        /// 全てのエンティティーのリスト
        /// </summary>
        public List<Entity> entities {
            get {
                return _entities;
            }
        }

        /// <summary>
        /// 存在するエンティティーの個数
        /// </summary>
        public int count {
            get {
                return this._entities.Count;
            }
        }

        /// <summary>
        /// 全ての物体が静止しているかを判定します
        /// </summary>
        public bool isStop {
            get {
                return _entities.All(entity => entity.isStop);
            }
        }

        /// <summary>
        /// 全てのエンティティーの平均直径
        /// </summary>
        public double entityDiameter {
            get {
                if(this.count == 0) return 0;

                return this._entities.Select(entity => entity.diameter).Average();
            }
        }

        /// <summary>
        /// 全てのエンティティーの平均位置
        /// </summary>
        public Vector2 position {
            get {
                if(this.count == 0) return Vector2.Zero;

                double averagePosX = this._entities.Select(entity => entity.position.X).Average();
                double averagePosY = this._entities.Select(entity => entity.position.Y).Average();

                return new Vector2(averagePosX, averagePosY);
            }
            set {
                Vector2 difference = value - this.position;

                this._entities.ForEach(entity => {
                    entity.position += difference;
                });
            }
        }

        /// <summary>
        /// 全てのエンティティーの平均速度
        /// </summary>
        public Vector2 velocity {
            get {
                if(this.count == 0) return Vector2.Zero;

                double averageVelocityX = this._entities.Select(entity => entity.velocity.X).Average();
                double averageVelocityY = this._entities.Select(entity => entity.velocity.Y).Average();

                return new Vector2(averageVelocityX, averageVelocityY);
            }
            set {
                this._entities.ForEach(entity => {
                    entity.velocity = value;
                });
            }
        }

        /// <summary>
        /// 全てのエンティティーの合計質量
        /// </summary>
        public double mass {
            get {
                if(this.count == 0) return 0;

                return this._entities.Select(entity => entity.mass).Sum();
            }
            set {
                if(value < 0) throw new Exception("質量(mass)は0以上に設定する必要があります");

                this._entities.ForEach(entity => {
                    entity.mass = value / this.count;
                });
            }
        }

        /// <summary>
        /// 全てのエンティティーの合計剛性
        /// </summary>
        public double stiffness {
            get {
                if(this.count == 0) return 0;

                return _entities.Select(entity => entity.stiffness).Average();
            }
            set {
                if(value <= 0 || value > 1) throw new Exception("剛性(stiffness)は0超過かつ1以下に設定する必要があります");

                this._entities.ForEach(entity => {
                    entity.stiffness = value;
                });
            }
        }

        /// <summary>
        /// エンティティーを追加します
        /// </summary>
        /// <param name="entityOption">エンティティーの初期化引数</param>
        /// <returns></returns>
        protected Entity AddEntity(EntityOption entityOption) {
            if(entityOption.id == null) entityOption.id = IdGenerator.CreateId(15);

            Entity entity = new Entity(entityOption);

            this._entities.Add(entity);

            return entity;
        }

        /// <summary>
        /// 複数のエンティティーを追加します
        /// </summary>
        /// <param name="entities">エンティティーの初期化引数のリスト</param>
        protected void AddAllEntities(List<EntityOption> entities) {
            entities.ForEach(entityOption => this.AddEntity(entityOption));
        }
    }
}
