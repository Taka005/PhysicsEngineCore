using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects{
    class Curve: IGround{
        public readonly string name;
        public readonly string type = "curve";
        public string color;
        public readonly Vector2 start;
        public readonly Vector2 middle;
        public readonly Vector2 center;
        public readonly Vector2 end;
        public readonly double radius;
        private double _thickness;

        public Curve(string name, string color, double startX, double startY, double middleX, double middleY, double endX, double endY, double thickness){
            this.name = name;
            this.color = color;
            this.start = new Vector2(startX, startY);
            this.middle = new Vector2(middleX, middleY);
            this.end = new Vector2(endX, endY);
            this._thickness = CheckThicknessValue(thickness);

            double slope1 = (middleX - startX) / (startY - middleY);
            double slope2 = (endX - middleX) / (middleY - endY);
            double equat1 = (startY + middleY) / 2 - slope1 * ((startX + middleX) / 2);
            double equat2 = (middleY + endY) / 2 - slope2 * ((middleX + endX) / 2);

            double centerX = (equat2 - equat1) / (slope1 - slope2);
            double centerY = slope1 * centerX + equat1;

            this.center = new Vector2(centerX, centerY);
            this.radius = Vector2.Distance(this.start,this.center);
        }
        public double thickness{
            get{
                return this._thickness;
            }
            set{
                this._thickness = CheckThicknessValue(value);
            }
        }

        public IGround Clone(){
            return new Curve(this.name, this.color, this.start.X, this.start.Y, this.middle.X, this.middle.Y, this.end.X, this.end.Y, this.thickness);
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

        private static double CheckThicknessValue(double thickness){
            if(thickness < 0) throw new Exception("厚さ(thickness)は0以上に設定する必要があります");

            return thickness;
        }
    }
}
