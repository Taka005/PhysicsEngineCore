using System.Windows;
using System.Windows.Media;
using PhysicsEngineCore.Objects;

namespace PhysicsEngineCore.Views {
    class CircleVisual : DrawingVisual {
        private readonly Circle objectData;
        private Brush brush;
        private Pen pen;

        public CircleVisual(Circle objectData) {
            this.objectData = objectData;
            this.brush = Utility.ParseColor(objectData.color);
            this.pen = new Pen(this.brush, 1);
        }

        public void Draw() {
            DrawingContext context = this.RenderOpen();

            this.brush = Utility.ParseColor(this.objectData.color);
            this.pen = new Pen(this.brush, 1);

            context.DrawEllipse(
                this.brush,
                this.pen,
                new Point(this.objectData.position.X, this.objectData.position.Y),
                this.objectData.radius,
                this.objectData.radius
            );

            context.Close();
        }
    }
}
