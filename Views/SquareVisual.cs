using System.Windows;
using System.Windows.Media;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Utils;
using PhysicsEngineCore.Views.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PhysicsEngineCore.Views {
    class SquareVisual : DrawingVisual, IObjectVisual {
        private Square objectData;
        private Brush brush;
        private Pen pen;
        private float _opacity = 1;

        public SquareVisual(Square objectData) {
            this.objectData = objectData;
            this.brush = ParseColor.StringToBrush(objectData.color);
            this.pen = new Pen(this.brush, 1);
        }

        public float opacity{
            get{
                return this._opacity;
            }
            set{
                if(value < 0 || value > 1) throw new ArgumentOutOfRangeException(nameof(value), "透明度は1から0の間である必要があります");

                this._opacity = value;
            }
        }

        public void DrawOwn() {
            using(DrawingContext context = this.RenderOpen()) {
                this.Draw(context);
            }
        }

        public void Draw(DrawingContext context) {
            if(this.objectData.image == null) {
                this.brush = ParseColor.StringToBrush(this.objectData.color);

                this.brush.Opacity = this.opacity;

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
            } else {
                TransformGroup transformGroup = new TransformGroup();

                Entity start = this.objectData.entities[0];
                Entity end = this.objectData.entities[2];

                double angle = (start.position - end.position).Angle();

                angle += Math.PI;

                transformGroup.Children.Add(new RotateTransform(angle * 180 / Math.PI, this.objectData.position.X, this.objectData.position.Y));

                context.PushTransform(transformGroup);
                context.PushOpacity(this.opacity);

                context.DrawImage(
                    this.objectData.image.source,
                    new Rect(
                        this.objectData.position.X - this.objectData.size/2,
                        this.objectData.position.Y - this.objectData.size/2,
                        this.objectData.size,
                        this.objectData.size
                    )
                );

                context.Pop();
                context.Pop();
            }
        }
    }
}
