using PhysicsEngineCore.Options;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects{
    public class Curve: IGround{
        public readonly string id;
        public readonly string type = "curve";
        public string color;
        public Vector2 start;
        public Vector2 middle;
        public Vector2 center;
        public Vector2 end;
        public double radius;
        public double _width;

        public Curve(CurveOption option){
            this.id = option.id ?? throw new ArgumentException(nameof(option.id));
            this.color = option.color;
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

        public double width{
            get{
                return this._width;
            }
            set{
                this._width = CheckWidthValue(value);
            }
        }

        public IGround Clone(){
            CurveOption option = new CurveOption{
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

            return new Curve(option);
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
