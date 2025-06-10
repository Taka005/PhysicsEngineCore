using System.Windows;
using System.Windows.Media;
using PhysicsEngineCore.Objects;

namespace PhysicsEngineCore.Views {
    class SquareVisual : DrawingVisual {
        private readonly Square objectData;
        private Brush brush;
        private Pen pen;

        public SquareVisual(Square objectData) {
            this.objectData = objectData;
            this.brush = Utility.ParseColor(objectData.color);
            this.pen = new Pen(this.brush, 1);
        }

        public void Draw() {
            DrawingContext context = this.RenderOpen();

            this.brush = Utility.ParseColor(this.objectData.color);
            this.pen = new Pen(this.brush, this.objectData.size / 2);

            this.objectData.entities.ForEach(source => {
                context.DrawEllipse(
                     this.brush,
                     null,
                     new Point(source.position.X, source.position.Y),
                     source.radius,
                     source.radius
                 );

                this.objectData.entities.ForEach(target => {
                    if(source.id == target.id) return;

                    context.DrawLine(
                        this.pen,
                        new Point(source.position.X, source.position.Y),
                        new Point(target.position.X, target.position.Y)
                    );
                });
            });

            context.Close();
        }
    }
}
