using System.Text.Json;
using PhysicsEngineCore.Options;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects {
    /// <summary>
    /// ブースターを表すクラス
    /// </summary>
    public class Booster : IEffect {
        private string _id;
        private readonly string _trackingId = IdGenerator.CreateId(15);
        private string _color;
        public Vector2 start;
        public Vector2 end;
        public Vector2 velocity;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="option">ブースターの初期化クラス</param>
        public Booster(BoosterOption option) {
            this._id = option.id ?? throw new ArgumentException(nameof(option.id));
            this._color = option.color;
            this.start = new Vector2(option.startX, option.startY);
            this.end = new Vector2(option.endX, option.endY);
            this.velocity = new Vector2(option.velocityX, option.velocityY);
        }

        /// <summary>
        /// オブジェクトの固有ID
        /// </summary>
        public string id {
            get {
                return this._id;
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
        /// ブースターの効果をエンティティーに適用します
        /// </summary>
        /// <param name="entity">適用するエンティティー</param>
        /// <param name="deltaTime">時間の経過量</param>
        public void SetEffect(Entity entity,double deltaTime) {
            if(entity.mass == 0) return;

            Vector2 difference = entity.position - (this.start + this.end) / 2;

            if(
                Math.Abs(difference.X) >= Math.Abs(this.start.X - this.end.X)/2||
                Math.Abs(difference.Y) >= Math.Abs(this.start.Y - this.end.Y)/2
            ) return;

            entity.velocity += this.velocity*deltaTime;
        }

        /// <summary>
        /// クラスのデータをJSON形式の文字列に変換します
        /// </summary>
        /// <returns>JSON形式の文字列</returns>
        public IEffect Clone() {
            return new Booster(this.ToOption());
        }

        /// <summary>
        /// 他のオブジェクトと同じ状態かどうかを比較します
        /// </summary>
        /// <param name="target">対象のオブジェクト</param>
        /// <returns>同じかどうか</returns>
        public bool Equals(IEffect target) {
            if(target is not Booster booster) return false;

            return this.id == booster.id &&
                   this.color == booster.color &&
                   this.start.Equals(booster.start) &&
                   this.end.Equals(booster.end) &&
                   this.velocity.Equals(booster.velocity);
        }

        /// <summary>
        /// 同じ状態のクラスを複製します
        /// </summary>
        /// <returns>複製されたクラス</returns>
        public string ToJson() {
            return JsonSerializer.Serialize(this.ToOption());
        }

        /// <summary>
        /// クラスの引数に変換します
        /// </summary>
        /// <returns>クラスの引数</returns>
        public BoosterOption ToOption() {
            return new BoosterOption {
                id = this.id,
                color = this.color,
                startX = this.start.X,
                startY = this.start.Y,
                endX = this.end.X,
                endY = this.end.Y,
                velocityX = this.velocity.X,
                velocityY = this.velocity.Y
            };
        }
    }
}
