using System.Text.Json;
using PhysicsEngineCore.Options;

namespace PhysicsEngineCore.Objects {
    /// <summary>
    /// 円を表すクラス
    /// </summary>
    public class Circle : BaseObject, IObject {
        private readonly string _id;
        private double _diameter;
        private string _color;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="option">円の初期化クラス</param>
        public Circle(CircleOption option) : base(option.entities) {
            this._id = option.id ?? throw new ArgumentException(nameof(option.id));
            this._diameter = option.diameter;
            this._color = option.color;

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
                entities = [.. this.entities.Select(entity => entity.ToOption())]
            };
        }
    }
}
