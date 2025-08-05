using System.Text.Json;
using PhysicsEngineCore.Objects.Interfaces;
using PhysicsEngineCore.Options;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects {
    /// <summary>
    /// 円を表すクラス
    /// </summary>
    public class Circle : BaseObject, IObject {
        private readonly string _id;
        private readonly string _trackingId = IdGenerator.CreateId(15);
        private double _diameter;
        private string _color;
        private string? _imageName = null;
        private Image? _image = null;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="option">円の初期化クラス</param>
        public Circle(CircleOption option) : base(option.entities) {
            this._id = option.id ?? throw new ArgumentException(nameof(option.id));
            this._diameter = option.diameter;
            this._color = option.color;
            this._imageName = option.imageName;

            if(option.entities.Count == 0) {
                EntityOption entityOption = new EntityOption {
                    posX = option.posX,
                    posY = option.posY,
                    diameter = option.diameter,
                    mass = option.mass,
                    stiffness = option.stiffness,
                    velocityX = option.velocityX,
                    velocityY = option.velocityY,
                    parentId = this.id
                };

                this.AddEntity(entityOption);
            } else{
                if(!this.position.Equals(new Vector2(option.posX, option.posY))){
                    this.position = new Vector2(option.posX, option.posY);
                }

                if (!this.velocity.Equals(new Vector2(option.velocityX, option.velocityY))){
                    this.velocity = new Vector2(option.velocityX, option.velocityY);
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
        /// オブジェクトの半径
        /// </summary>
        public double radius {
            get {
                return this.diameter / 2;
            }
            set {
                this.diameter = value * 2;
            }
        }

        /// <summary>
        /// オブジェクトの直径
        /// </summary>
        public double diameter {
            get {
                return this._diameter;
            }
            set {
                if(value <= 0) throw new ArgumentException("直径を0以下に設定することはできません");

                this._diameter = value;
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
            return new Circle(this.ToOption());
        }

        /// <summary>
        /// 他のオブジェクトと同じ状態かどうかを比較します
        /// </summary>
        /// <param name="target">対象のオブジェクト</param>
        /// <returns>同じかどうか</returns>
        public bool Equals(IObject target) {
            if(target is not Circle circle) return false;

            return this.id == circle.id &&
                   this.diameter == circle.diameter &&
                   this.color == circle.color &&
                   this.imageName == circle.imageName &&
                   this.position.Equals(circle.position) &&
                   this.velocity.Equals(circle.velocity) &&
                   this.mass == circle.mass &&
                   this.stiffness == circle.stiffness;
        }

        /// <summary>
        /// クラスの引数に変換します
        /// </summary>
        /// <returns>クラスの引数</returns>
        public CircleOption ToOption() {
            return new CircleOption {
                id = this.id,
                posX = this.position.X,
                posY = this.position.Y,
                velocityX = this.velocity.X,
                velocityY = this.velocity.Y,
                diameter = this.diameter,
                mass = this.mass,
                stiffness = this.stiffness,
                color = this.color,
                imageName = this.imageName,
                entities = [.. this.entities.Select(entity => entity.ToOption())]
            };
        }
    }
}
