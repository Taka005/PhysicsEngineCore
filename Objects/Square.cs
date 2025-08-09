using System.Text.Json;
using PhysicsEngineCore.Objects.Interfaces;
using PhysicsEngineCore.Options;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects {
    /// <summary>
    /// 四角を表すクラス
    /// </summary>
    public class Square : BaseObject, IObject {
        private string _id;
        private readonly string _trackingId = IdGenerator.CreateId(Engine.DEFAULT_ID_LENGTH);
        private double _size;
        private string _color;
        private string? _imageName = null;
        private Image? _image = null;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="option">四角の初期化クラス</param>
        public Square(SquareOption option) : base(option.entities) {
            this._id = option.id ?? throw new ArgumentException(nameof(option.id));
            this.size = option.size;
            this._color = option.color;
            this._imageName = option.imageName;

            if(option.entities.Count == 0) {
                for(int i = -1;i <= 1;i += 2) {
                    for(int j = -1;j <= 1;j += 2) {
                        EntityOption entityOption = new EntityOption {
                            posX = option.posX + i * (this.size / 4),
                            posY = option.posY + j * (this.size / 4),
                            diameter = this.size / 2,
                            mass = option.mass / 4,
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
            set{
                if (string.IsNullOrEmpty(value)) throw new ArgumentException("IDは空にできません");

                this._id = value;

                foreach (Entity entity in this.entities){
                    entity.parentId = this._id;
                }
            }
        }

        /// <summary>
        /// クラスの固有ID
        /// トラッキング用
        /// </summary>
        public string trackingId {
            get{
                return this._trackingId;
            }
        }

        /// <summary>
        /// オブジェクトの幅
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

        public string? imageName {
            get {
                return this._imageName;
            }
        }

        public Image? image {
            get {
                return this._image;
            }
            set {
                this._image = value;
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
            return new Square(this.ToOption());
        }

        /// <summary>
        /// 他のオブジェクトと同じ状態かどうかを比較します
        /// </summary>
        /// <param name="target">対象のオブジェクト</param>
        /// <returns>同じかどうか</returns>
        public bool Equals(IObject target) {
            if(target is not Square square) return false;

            return this.id == square.id &&
                   this.size == square.size &&
                   this.color == square.color &&
                   this.imageName == square.imageName &&
                   this.position.Equals(square.position) &&
                   this.velocity.Equals(square.velocity) &&
                   this.mass == square.mass &&
                   this.stiffness == square.stiffness;
        }

        /// <summary>
        /// クラスの引数に変換します
        /// </summary>
        /// <returns>クラスの引数</returns>
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
                imageName = this.imageName,
                entities = [.. this.entities.Select(entity => entity.ToOption())]
            };
        }
    }
}
