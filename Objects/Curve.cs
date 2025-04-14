using PhysicsEngineCore.Utils;
using static System.Formats.Asn1.AsnWriter;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PhysicsEngineCore.Objects{
    class Curve: IGround{
        public readonly string name;
        public readonly string type = "line";
        public string color;
        public Vector2 start;
        public Vector2 middle;
        public Vector2 center;
        public Vector2 end;
        public double radius;
        private double _thickness;

        public Curve(string name, string color, double startX, double startY, double middleX, double middleY, double endX, double endY, double thickness) {
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
        public Vector2 SolvePosition(){

        }

        private bool isAngleBetween(double angle, double start, double end, bool clockwise){
            if(clockwise){
                return (angle >= start&&angle <= end)||(start > end&&(angle >= start||angle <= end));
            }else{
                return (angle <= start&&angle >= end)||(start < end&&(angle <= start||angle >= end));
            }
        }

        private double normalizeAngle(double angle){
            return (angle + 2 * Math.PI) % (2 * Math.PI);
        }

        private static double CheckThicknessValue(double thickness) {
            if(thickness < 0) throw new Exception("厚さ(thickness)は0以上に設定する必要があります");

            return thickness;
        }
    }
}
