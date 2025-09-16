using System.Windows;
using System.Windows.Media;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Objects.Interfaces;
using PhysicsEngineCore.Utils;
using PhysicsEngineCore.Views.Interfaces;

namespace PhysicsEngineCore.Views {
    class CurveVisual : DrawingVisual, IGroundVisual {
        public IGround groundData { get; }
        private Brush brush;
        private Pen pen;

        public CurveVisual(Curve groundData) {
            this.groundData = groundData;
            this.brush = ParseColor.StringToBrush(this.groundData.color);
            this.pen = new Pen(this.brush, this.groundData.width);
        }

        public void Draw(DrawingContext context) {
            if (this.groundData is Curve curve){
                this.brush = ParseColor.StringToBrush(curve.color);
                this.pen = new Pen(this.brush, curve.width);

                double startAngle = Curve.NormalizeAngle(Math.Atan2(curve.start.Y - curve.center.Y, curve.start.X - curve.center.X));
                double endAngle = Curve.NormalizeAngle(Math.Atan2(curve.end.Y - curve.center.Y, curve.end.X - curve.center.X));
                double midAngle = Curve.NormalizeAngle(Math.Atan2(curve.middle.Y - curve.center.Y, curve.middle.X - curve.center.X));
                bool clockwise = (startAngle > endAngle) ? (midAngle > startAngle || midAngle < endAngle) : (midAngle > startAngle && midAngle < endAngle);

                PathGeometry pathGeometry = new PathGeometry();
                PathFigure pathFigure = new PathFigure {
                    StartPoint = new Point(curve.start.X, curve.start.Y)
                };

                ArcSegment arcSegment = new ArcSegment {
                    Point = new Point(curve.end.X, curve.end.Y),
                    Size = new Size(curve.radius, curve.radius),
                    IsLargeArc = Curve.IsMiddleOnLargeArc(startAngle, endAngle, midAngle),
                    SweepDirection = clockwise ? SweepDirection.Clockwise : SweepDirection.Counterclockwise
                };

                pathFigure.Segments.Add(arcSegment);
                pathGeometry.Figures.Add(pathFigure);

                context.DrawGeometry(null, this.pen, pathGeometry);

                context.DrawEllipse(this.brush, null, new Point(curve.start.X, curve.start.Y), curve.width / 2, curve.width / 2);
                context.DrawEllipse(this.brush, null, new Point(curve.end.X, curve.end.Y), curve.width / 2, curve.width / 2);
            }
        }
    }
}
