using System.Text.Json;
using PhysicsEngineCore.Objects.Interfaces;
using PhysicsEngineCore.Options;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects {
    /// <summary>
    /// 線を表すクラス
    /// </summary>
    public class Line : IGround {
        private string _id;
        private readonly string _trackingId = IdGenerator.CreateId(15);
        private string _color;
        public Vector2 start;
        public Vector2 end;
        private double _width;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="option">線の初期化クラス</param>
        public Line(LineOption option) {
            this._id = option.id ?? throw new ArgumentException(nameof(option.id));
            this._color = option.color;
            this.start = new Vector2(option.startX, option.startY);
            this.end = new Vector2(option.endX, option.endY);
            this._width = CheckWidthValue(option.width);
        }

        /// <summary>
        /// 線の幅
        /// </summary>
        public double width {
            get {
                return this._width;
            }
            set {
                this._width = CheckWidthValue(value);
            }
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
        /// 線の長さ
        /// </summary>
        public double lenght {
            get {
                return Vector2.Distance(this.start, this.end);
            }
        }

        /// <summary>
        /// クラスのデータをJSON形式の文字列に変換します
        /// </summary>
        /// <returns>JSON形式の文字列</returns>
        public IGround Clone() {
            return new Line(this.ToOption());
        }

        /// <summary>
        /// 他のオブジェクトと同じ状態かどうかを比較します
        /// </summary>
        /// <param name="target">対象のオブジェクト</param>
        /// <returns>同じかどうか</returns>
        public bool Equals(IGround target) {
            if(target is not Line line) return false;

            return this.id == line.id &&
                   this.width == line.width &&
                   this.color == line.color &&
                   this.start.Equals(line.start) &&
                   this.end.Equals(line.end);
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
        public LineOption ToOption() {
            return new LineOption {
                id = this.id,
                color = this.color,
                startX = this.start.X,
                startY = this.start.Y,
                endX = this.end.X,
                endY = this.end.Y,
                width = this.width,
            };
        }

        /// <summary>
        /// ある点から線に垂直で下した時の交点を返します
        /// 線の端のほうが近い場合は、端の座標を返します
        /// </summary>
        /// <param name="position">任意の座標</param>
        /// <returns>交差した座標</returns>
        public Vector2 SolvePosition(Vector2 position) {
            double flag = ((position.X - this.start.X) * (this.end.X - this.start.X) + (position.Y - this.start.Y) * (this.end.Y - this.start.Y)) / Vector2.DistanceSquared(this.start, this.end);

            if(flag <= 0) {
                return this.start;
            } else if(flag >= 1) {
                return this.end;
            } else {
                double crossX = this.start.X + flag * (this.end.X - this.start.X);
                double crossY = this.start.Y + flag * (this.end.Y - this.start.Y);

                return new Vector2(crossX, crossY);
            }
        }

        /// <summary>
        /// 幅が正しい値かチェックします
        /// </summary>
        /// <param name="width">幅</param>
        /// <returns>正しい幅</returns>
        /// <exception cref="Exception">0未満であった時に例外</exception>
        private static double CheckWidthValue(double width) {
            if(width < 0) throw new Exception("厚さ(width)は0以上に設定する必要があります");

            return width;
        }
    }
}
