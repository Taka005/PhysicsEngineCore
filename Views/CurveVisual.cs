using System.Windows;
using System.Windows.Media;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Utils;
using PhysicsEngineCore.Views.Interfaces;

namespace PhysicsEngineCore.Views {
    class CurveVisual : DrawingVisual, IGroundVisual {
        private Curve groundData;
        private Brush brush;
        private Pen pen;

        public CurveVisual(Curve groundData) {
            this.groundData = groundData;
            this.brush = ParseColor.StringToBrush(this.groundData.color);
            this.pen = new Pen(this.brush, this.groundData.width);
        }

        public void Draw(DrawingContext context) {
            this.brush = ParseColor.StringToBrush(this.groundData.color);
            this.pen = new Pen(this.brush, this.groundData.width);

            double startAngle = Curve.NormalizeAngle(Math.Atan2(this.groundData.start.Y - this.groundData.center.Y, this.groundData.start.X - this.groundData.center.X));
            double endAngle = Curve.NormalizeAngle(Math.Atan2(this.groundData.end.Y - this.groundData.center.Y, this.groundData.end.X - this.groundData.center.X));
            double midAngle = Curve.NormalizeAngle(Math.Atan2(this.groundData.middle.Y - this.groundData.center.Y, this.groundData.middle.X - this.groundData.center.X));
            bool clockwise = (startAngle > endAngle) ? (midAngle > startAngle || midAngle < endAngle) : (midAngle > startAngle && midAngle < endAngle);

            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure {
                StartPoint = new Point(this.groundData.start.X, this.groundData.start.Y)
            };

            ArcSegment arcSegment = new ArcSegment {
                Point = new Point(this.groundData.end.X, this.groundData.end.Y),
                Size = new Size(this.groundData.radius, this.groundData.radius),
                IsLargeArc = IsMiddleOnLargeArc(startAngle, endAngle, midAngle),
                SweepDirection = clockwise ? SweepDirection.Clockwise : SweepDirection.Counterclockwise
            };

            pathFigure.Segments.Add(arcSegment);
            pathGeometry.Figures.Add(pathFigure);

            context.DrawGeometry(null, this.pen, pathGeometry);

            context.DrawEllipse(this.brush, null, new Point(this.groundData.start.X, this.groundData.start.Y), this.groundData.width / 2, this.groundData.width / 2);
            context.DrawEllipse(this.brush, null, new Point(this.groundData.end.X, this.groundData.end.Y), this.groundData.width / 2, this.groundData.width / 2);
        }

        private static bool IsMiddleOnLargeArc(double startAngle, double endAngle, double midAngle){
            double diff = Math.Abs(endAngle - startAngle);

            double clockwiseAngleDiff = endAngle - startAngle;
            if(clockwiseAngleDiff < 0) clockwiseAngleDiff += 2 * Math.PI;

            double counterClockwiseAngleDiff = startAngle - endAngle;
            if (counterClockwiseAngleDiff < 0) counterClockwiseAngleDiff += 2 * Math.PI;

            bool midInClockwiseArc = Curve.IsAngleBetween(midAngle, startAngle, endAngle, true);
            bool midInCounterClockwiseArc = Curve.IsAngleBetween(midAngle, startAngle, endAngle, false);

            if (midInClockwiseArc && clockwiseAngleDiff > Math.PI) return true;
            if (midInCounterClockwiseArc && counterClockwiseAngleDiff > Math.PI) return true;

            return false;
        }
    }
}
