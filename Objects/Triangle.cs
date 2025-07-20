using System.Text.Json;
using PhysicsEngineCore.Options;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects {
    /// <summary>
    /// 三角を表すクラス
    /// </summary>
    public class Triangle : BaseObject, IObject {
        private string _id;
        private readonly string _trackingId = IdGenerator.CreateId(15);
        private double _size;
        private string _color;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="option">三角の初期化クラス</param>
        public Triangle(TriangleOption option) : base(option.entities) {
            this._id = option.id ?? throw new ArgumentException(nameof(option.id));
            this.size = option.size;
            this._color = option.color;

            if(option.entities.Count == 0) {
                EntityOption entityOption1 = new EntityOption {
                    posX = option.posX,
                    posY = option.posY - (2 / Math.Sqrt(3)) * (this.size / 3),
                    diameter = option.size / 2,
                    mass = option.mass / 3,
                    stiffness = option.stiffness,
                    velocityX = option.velocityX,
                    velocityY = option.velocityY,
                    parentId = this.id
                };

                this.AddEntity(entityOption1);

                for(int i = -1;i <= 1;i += 2) {
                    EntityOption entityOption2 = new EntityOption {
                        posX = option.posX + i * (this.size / 3),
                        posY = option.posY + (1 / Math.Sqrt(3)) * (this.size / 3),
                        diameter = option.size / 2,
                        mass = option.mass / 3,
                        stiffness = option.stiffness,
                        velocityX = option.velocityX,
                        velocityY = option.velocityY,
                        parentId = this.id
                    };

                    this.AddEntity(entityOption2);
                }

                this.entities.ForEach(source => {
                    this.entities.ForEach(target => {
                        if(source.id == target.id) return;

                        Vector2 difference = target.position - source.position;

                        double distance = difference.Length();

                        source.connection.Add(target, distance, source.stiffness);
                    });
                });
            }
        }

        /// <summary>
        /// オブジェクトの固有ID
        /// </summary>
        public string id {
            get {
                return _id;
            }
            set {
                foreach(Entity entity in this.entities) {
                    entity.parentId = value;
                }

                this._id = value;
            }
        }

        public string trackingId {
            get{
                return this._trackingId;
            }
        }

        /// <summary>
        /// オブジェクトの大きさ
        /// </summary>
        public double size {
            get {
                return this._size;
            }
            set {
                if(value <= 0) throw new ArgumentException("幅を0以下に設定することはできません");

                this._size = value;
            }
        }

        /// <summary>
        /// オブジェクトの色
        /// Hexの値です
        /// </summary>
        public string color {
            get {
                return this._color;
            }
            set {
                this._color = value;
            }
        }

        /// <summary>
        /// 同じ状態のクラスを複製します
        /// </summary>
        /// <returns>複製されたクラス</returns>
        public string ToJson() {
            return JsonSerializer.Serialize(this.ToOption());
        }

        /// <summary>
        /// 同じ状態のクラスを複製します
        /// </summary>
        /// <returns>複製されたクラス</returns>
        public IObject Clone() {
            return new Triangle(this.ToOption());
        }

        /// <summary>
        /// 他のオブジェクトと同じ状態かどうかを比較します
        /// </summary>
        /// <param name="target">対象のオブジェクト</param>
        /// <returns>同じかどうか</returns>
        public bool Equals(IObject target) {
            if(target is not Triangle triangle) return false;

            return this.id == triangle.id &&
                   this.size == triangle.size &&
                   this.color == triangle.color &&
                   this.position.Equals(triangle.position) &&
                   this.velocity.Equals(triangle.velocity) &&
                   this.mass == triangle.mass &&
                   this.stiffness == triangle.stiffness;
        }

        /// <summary>
        /// クラスの引数に変換します
        /// </summary>
        /// <returns>クラスの引数</returns>
        public TriangleOption ToOption() {
            return new TriangleOption {
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
