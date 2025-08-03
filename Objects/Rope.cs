using System.Text.Json;
using PhysicsEngineCore.Objects.Interfaces;
using PhysicsEngineCore.Options;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects {
    /// <summary>
    /// ロープを表すクラス
    /// </summary>
    public class Rope : BaseObject, IObject {
        private readonly string _id;
        private readonly string _trackingId = IdGenerator.CreateId(15);
        private double _width;
        private string _color;
        private string? _imageName;
        private Image? _image;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="option">ロープの初期化クラス</param>
        public Rope(RopeOption option) : base(option.entities) {
            this._id = option.id ?? throw new ArgumentException(nameof(option.id));
            this.width = option.width;
            this._color = option.color;
            this._imageName = option.imageName;

            if(option.entities.Count == 0) {
                double width = option.endX - option.startX;
                double height = option.endY - option.startY;

                double count = Math.Floor(Math.Sqrt(width * width + height * height) / (this.width));

                Entity? entity = null;
                for(int i = 0;i <= count;i++) {
                    double posX = option.startX + i * (width / count);
                    double posY = option.startY + i * (height / count);

                    EntityOption entityOption = new EntityOption {
                        posX = posX,
                        posY = posY,
                        diameter = option.width,
                        mass = option.mass,
                        stiffness = option.stiffness,
                        velocityX = option.velocityX,
                        velocityY = option.velocityY,
                        parentId = this.id
                    };

                    Entity target = this.AddEntity(entityOption);

                    if(entity != null) {
                        entity.connection.Add(target, this.width, option.stiffness);
                        target.connection.Add(entity, this.width, option.stiffness);
                    }

                    entity = target;
                }
            }
        }

        /// <summary>
        /// オブジェクトの固有ID
        /// </summary>
        public string id {
            get {
                return _id;
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
        /// ロープの幅
        /// </summary>
        public double width {
            get {
                return this._width;
            }
            set {
                if(value <= 0) throw new ArgumentException("幅を0以下に設定することはできません");

                this._width = value;
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
                this._imageName = value?.filename;
            }
        }

        /// <summary>
        /// クラスのデータをJSON形式の文字列に変換します
        /// </summary>
        /// <returns>JSON形式の文字列</returns>
        public string ToJson() {
            return JsonSerializer.Serialize(this.ToOption());
        }

        /// <summary>
        /// 同じ状態のクラスを複製します
        /// </summary>
        /// <returns>複製されたクラス</returns>
        public IObject Clone() {
            return new Rope(this.ToOption());
        }

        /// <summary>
        /// 他のオブジェクトと同じ状態かどうかを比較します
        /// </summary>
        /// <param name="target">対象のオブジェクト</param>
        /// <returns>同じかどうか</returns>
        public bool Equals(IObject target) {
            if(target is not Rope rope) return false;

            return this.id == rope.id &&
                   this.width == rope.width &&
                   this.color == rope.color &&
                   this.position.Equals(rope.position) &&
                   this.velocity.Equals(rope.velocity) &&
                   this.mass == rope.mass &&
                   this.stiffness == rope.stiffness;
        }


        /// <summary>
        /// クラスの引数に変換します
        /// </summary>
        /// <returns>クラスの引数</returns>
        public RopeOption ToOption() {
            return new RopeOption {
                id = this.id,
                startX = this.entities.First().position.X,
                startY = this.entities.First().position.Y,
                endX = this.entities.Last().position.X,
                endY = this.entities.Last().position.Y,
                velocityX = this.velocity.X,
                velocityY = this.velocity.Y,
                width = this.width,
                mass = this.mass,
                stiffness = this.stiffness,
                color = this.color,
                entities = [.. this.entities.Select(entity => entity.ToOption())]
            };
        }
    }
}
