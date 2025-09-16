using System.Windows;
using System.Windows.Media;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Objects.Interfaces;
using PhysicsEngineCore.Utils;
using PhysicsEngineCore.Views.Interfaces;

namespace PhysicsEngineCore.Views {
    class LineVisual : DrawingVisual, IGroundVisual {
        public IGround groundData { get; }
        private Brush brush;
        private Pen pen;

        public LineVisual(Line groundData) {
            this.groundData = groundData;
            this.brush = ParseColor.StringToBrush(groundData.color);
            this.pen = new Pen(this.brush, 1);
        }

        public void Draw(DrawingContext context) {
            if (this.groundData is Line line){
                this.brush = ParseColor.StringToBrush(line.color);
                this.pen = new Pen(this.brush, line.width);

                context.DrawLine(
                    this.pen,
                    new Point(line.start.X, line.start.Y),
                    new Point(line.end.X, line.end.Y)
                );

                context.DrawEllipse(
                    this.brush,
                    null,
                    new Point(line.start.X, line.start.Y),
                    line.width / 2,
                    line.width / 2
                );

                context.DrawEllipse(
                    this.brush,
                    null,
                    new Point(line.end.X, line.end.Y),
                    line.width / 2,
                    line.width / 2
                );
            }
        }
    }
}
