using System.Windows;
using System.Windows.Media;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Utils;
using PhysicsEngineCore.Views.Interfaces;

namespace PhysicsEngineCore.Views {
    class LineVisual : DrawingVisual, IGroundVisual {
        private Line groundData;
        private Brush brush;
        private Pen pen;

        public LineVisual(Line groundData) {
            this.groundData = groundData;
            this.brush = ParseColor.StringToBrush(groundData.color);
            this.pen = new Pen(this.brush, 1);
        }

        public void Draw(DrawingContext context) {
            this.brush = ParseColor.StringToBrush(this.groundData.color);
            this.pen = new Pen(this.brush, this.groundData.width);

            context.DrawLine(
                this.pen,
                new Point(this.groundData.start.X, this.groundData.start.Y),
                new Point(this.groundData.end.X, this.groundData.end.Y)
            );

            context.DrawEllipse(
                this.brush,
                null,
                new Point(this.groundData.start.X, this.groundData.start.Y),
                this.groundData.width / 2,
                this.groundData.width / 2
            );

            context.DrawEllipse(
                this.brush,
                null,
                new Point(this.groundData.end.X, this.groundData.end.Y),
                this.groundData.width / 2,
                this.groundData.width / 2
            );
        }
    }
}
