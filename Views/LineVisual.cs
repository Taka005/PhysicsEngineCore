using System.Windows;
using System.Windows.Media;
using PhysicsEngineCore.Objects;

namespace PhysicsEngineCore.Views {
    class LineVisual : DrawingVisual {
        private readonly Line groundData;
        private Brush brush;
        private Pen pen;

        public LineVisual(Line groundData) {
            this.groundData = groundData;
            this.brush = Utility.ParseColor(groundData.color);
            this.pen = new Pen(this.brush, 1);
        }

        public void Draw() {
            DrawingContext context = this.RenderOpen();

            this.brush = Utility.ParseColor(this.groundData.color);
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

            context.Close();
        }
    }
}
