using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Objects.Interfaces;
using PhysicsEngineCore.Utils;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;


namespace PhysicsEngineCore.Renders{
    public class GroundRender{
        public static void DrawLine(DrawingContext context, Line line){
            Brush brush = ParseColor.StringToBrush(line.color);
            Pen pen = new Pen(brush, line.width);

            context.DrawLine(
                pen,
                new Point(line.start.X, line.start.Y),
                new Point(line.end.X, line.end.Y)
            );

            context.DrawEllipse(
                brush,
                null,
                new Point(line.start.X, line.start.Y),
                line.width / 2,
                line.width / 2
            );

            context.DrawEllipse(
                brush,
                null,
                new Point(line.end.X, line.end.Y),
                line.width / 2,
                line.width / 2
            );
        }

        public static void DrawCurve(DrawingContext context, Curve curve){
            Brush brush = ParseColor.StringToBrush(curve.color);
            Pen pen = new Pen(brush, curve.width);

            double startAngle = Curve.NormalizeAngle(Math.Atan2(curve.start.Y - curve.center.Y, curve.start.X - curve.center.X));
            double endAngle = Curve.NormalizeAngle(Math.Atan2(curve.end.Y - curve.center.Y, curve.end.X - curve.center.X));
            double midAngle = Curve.NormalizeAngle(Math.Atan2(curve.middle.Y - curve.center.Y, curve.middle.X - curve.center.X));
            bool clockwise = (startAngle > endAngle) ? (midAngle > startAngle || midAngle < endAngle) : (midAngle > startAngle && midAngle < endAngle);

            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure{
                StartPoint = new Point(curve.start.X, curve.start.Y)
            };

            ArcSegment arcSegment = new ArcSegment{
                Point = new Point(curve.end.X, curve.end.Y),
                Size = new Size(curve.radius, curve.radius),
                IsLargeArc = Curve.IsMiddleOnLargeArc(startAngle, endAngle, midAngle),
                SweepDirection = clockwise ? SweepDirection.Clockwise : SweepDirection.Counterclockwise
            };

            pathFigure.Segments.Add(arcSegment);
            pathGeometry.Figures.Add(pathFigure);

            context.DrawGeometry(null, pen, pathGeometry);

            context.DrawEllipse(brush, null, new Point(curve.start.X, curve.start.Y), curve.width / 2, curve.width / 2);
            context.DrawEllipse(brush, null, new Point(curve.end.X, curve.end.Y), curve.width / 2, curve.width / 2);
        }

        public static void DrawEdge(DrawingContext context, IGround ground){
            Pen edgePen = new Pen(Brushes.Black, 1);

            if (ground is Line line){
                context.DrawEllipse(
                    null,
                    edgePen,
                    new Point(line.start.X, line.start.Y),
                    line.width / 2 - 0.5,
                    line.width / 2 - 0.5
                );

                context.DrawEllipse(
                    null,
                    edgePen,
                    new Point(line.end.X, line.end.Y),
                    line.width / 2 - 0.5,
                    line.width / 2 - 0.5
                );
            }else if (ground is Curve curve){
                context.DrawEllipse(
                    null,
                    edgePen,
                    new Point(curve.start.X, curve.start.Y),
                    curve.width / 2 - 0.5,
                    curve.width / 2 - 0.5
                );

                context.DrawEllipse(
                    null,
                    edgePen,
                    new Point(curve.middle.X, curve.middle.Y),
                    curve.width / 2 - 0.5,
                    curve.width / 2 - 0.5
                );

                context.DrawEllipse(
                    null,
                    edgePen,
                    new Point(curve.center.X, curve.center.Y),
                    curve.width / 2 - 0.5,
                    curve.width / 2 - 0.5
                );

                context.DrawEllipse(
                    null,
                    edgePen,
                    new Point(curve.end.X, curve.end.Y),
                    curve.width / 2 - 0.5,
                    curve.width / 2 - 0.5
                );
            }
        }
    }
}
