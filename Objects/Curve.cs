using System.Text.Json;
using System.Windows.Media;
using PhysicsEngineCore.Options;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects{

    /// <summary>
    /// 曲線を表すクラス
    /// </summary>
    public class Curve: IGround{
        private readonly string _id;
        private string _color;
        public Vector2 start;
        public Vector2 middle;
        public Vector2 center;
        public Vector2 end;
        public double radius;
        private double _width;
        private readonly DrawingVisual _visual = new DrawingVisual();

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="option">曲線の初期化クラス</param>
        public Curve(CurveOption option){
            this._id = option.id;
            this._color = option.color;
            this.start = new Vector2(option.startX, option.startY);
            this.middle = new Vector2(option.middleX, option.middleY);
            this.end = new Vector2(option.endX, option.endY);
            this._width = CheckWidthValue(option.width);

            double slope1 = (option.middleX - option.startX) / (option.startY - option.middleY);
            double slope2 = (option.endX - option.middleX) / (option.middleY - option.endY);
            double equat1 = (option.startY + option.middleY) / 2 - slope1 * ((option.startX + option.middleX) / 2);
            double equat2 = (option.middleY + option.endY) / 2 - slope2 * ((option.middleX + option.endX) / 2);

            double centerX = (equat2 - equat1) / (slope1 - slope2);
            double centerY = slope1 * centerX + equat1;

            this.center = new Vector2(centerX, centerY);
            this.radius = Vector2.Distance(this.start,this.center);
        }

        /// <summary>
        /// オブジェクトの固有ID
        /// </summary>
        public string id{
            get{
                return this._id;
            }
        }

        /// <summary>
        /// 描画インスタンス
        /// </summary>
        public DrawingVisual visual{
            get{
                return _visual;
            }
        }

        /// <summary>
        /// オブジェクトの色
        /// Hexの値です
        /// </summary>
        public string color{
            get{
                return this._color;
            }
            set{
                this._color = value;
            }
        }

        /// <summary>
        /// 線の幅
        /// </summary>
        public double width{
            get{
                return this._width;
            }
            set{
                this._width = CheckWidthValue(value);
            }
        }

        /// <summary>
        /// オブジェクトを描画します
        /// </summary>
        public void Draw() {
            DrawingContext context = _visual.RenderOpen();

            Brush brush = Utility.ParseColor(this._color);
        }

        /// <summary>
        /// クラスのデータをJSON形式の文字列に変換します
        /// </summary>
        /// <returns>JSON形式の文字列</returns>
        public string ToJson(){
            return JsonSerializer.Serialize(this.ToOption());
        }

        /// <summary>
        /// 同じ状態のクラスを複製します
        /// </summary>
        /// <returns>複製されたクラス</returns>
        public IGround Clone(){
            return new Curve(this.ToOption());
        }

        /// <summary>
        /// クラスの引数に変換します
        /// </summary>
        /// <returns>クラスの引数</returns>
        public CurveOption ToOption(){
            return new CurveOption {
                id = this.id,
                color = this.color,
                startX = this.start.X,
                startY = this.start.Y,
                middleX = this.middle.X,
                middleY = this.middle.Y,
                endX = this.end.X,
                endY = this.end.Y,
                width = this.width
            };
        }

        public Vector2 SolvePosition(Vector2 position){
            Vector2 difference = position - this.center;

            double distance = difference.Length();
            if(distance == 0) return this.start;

            double scale = this.radius / distance;

            Vector2 cross = this.center + difference * scale;

            double startAngle = NormalizeAngle(Math.Atan2(this.start.Y - this.center.Y, this.start.X - this.center.X));
            double midAngle = NormalizeAngle(Math.Atan2(this.middle.Y - this.center.Y, this.middle.X - this.center.X));
            double endAngle = NormalizeAngle(Math.Atan2(this.end.Y - this.center.Y, this.end.X - this.center.X));
            double crossAngle = NormalizeAngle(Math.Atan2(cross.Y - this.center.Y, cross.X - this.center.X));

            bool clockwise = (startAngle > endAngle) ? (midAngle > startAngle || midAngle < endAngle) : (midAngle > startAngle && midAngle < endAngle);

            if(!IsAngleBetween(crossAngle, startAngle, endAngle, clockwise)){
                double startDistance = Vector2.DistanceSquared(this.start, position);
                double endDistance = Vector2.DistanceSquared(this.end, position);

                if(startDistance < endDistance){
                    return this.start;
                }else{
                    return this.end;
                }
            }

            return cross;
        }

        private static bool IsAngleBetween(double angle, double start, double end, bool clockwise){
            if(clockwise){
                return (angle >= start&&angle <= end)||(start > end&&(angle >= start||angle <= end));
            }else{
                return (angle <= start&&angle >= end)||(start < end&&(angle <= start||angle >= end));
            }
        }

        private static double NormalizeAngle(double angle){
            return (angle + 2 * Math.PI) % (2 * Math.PI);
        }

        private static double CheckWidthValue(double thickness){
            if(thickness < 0) throw new Exception("厚さ(thickness)は0以上に設定する必要があります");

            return thickness;
        }
    }
}
