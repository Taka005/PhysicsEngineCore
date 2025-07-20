using System.Windows;
using System.Windows.Media;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Views {
    class RopeVisual : DrawingVisual, IDraw {
        private readonly Rope objectData;
        private Brush brush;
        private Pen pen;

        public RopeVisual(Rope objectData) {
            this.objectData = objectData;
            this.brush = ParseColor.StringToBrush(objectData.color);
            this.pen = new Pen(this.brush, 1);
        }

        public void Draw() {
            DrawingContext context = this.RenderOpen();

            this.brush = ParseColor.StringToBrush(this.objectData.color);
            this.pen = new Pen(this.brush, this.objectData.width);

            Entity? target = null;

            this.objectData.entities.ForEach(entity => {
                if(target != null) {
                    context.DrawLine(
                        this.pen,
                        new Point(entity.position.X, entity.position.Y),
                        new Point(target.position.X, target.position.Y)
                    );

                    context.DrawEllipse(
                        this.brush,
                        null,
                        new Point(entity.position.X, entity.position.Y),
                        entity.radius,
                        entity.radius
                    );
                } else {
                    context.DrawEllipse(
                        this.brush,
                        null,
                        new Point(entity.position.X, entity.position.Y),
                        entity.radius,
                        entity.radius
                    );
                }

                if(this.objectData.entities.IndexOf(entity) == this.objectData.entities.Count - 1) {
                    context.DrawEllipse(
                        this.brush,
                        null,
                        new Point(entity.position.X, entity.position.Y),
                        entity.radius,
                        entity.radius
                    );
                }

                target = entity;
            });

            context.Close();
        }
    }
}