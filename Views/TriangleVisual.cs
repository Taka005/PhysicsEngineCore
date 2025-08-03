using System.Windows;
using System.Windows.Media;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Utils;
using PhysicsEngineCore.Views.Interfaces;

namespace PhysicsEngineCore.Views {
    class TriangleVisual : DrawingVisual, IObjectVisual {
        private Triangle objectData;
        private Brush brush;
        private Pen pen;
        private float _opacity = 1;

        public TriangleVisual(Triangle objectData) {
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
            DrawingContext context = this.RenderOpen();

            this.Draw(context);

            context.Close();
        }

        public void Draw(DrawingContext context) {
            if(this.objectData.image == null) {
                this.brush = ParseColor.StringToBrush(this.objectData.color);

                this.brush.Opacity = this.opacity;

                this.pen = new Pen(this.brush, this.objectData.entityDiameter);

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
                double scale = this.objectData.size / Math.Min(this.objectData.image.width, this.objectData.image.height);

                TransformGroup transformGroup = new TransformGroup();
                transformGroup.Children.Add(new ScaleTransform(scale, scale));

                double angleRad = Math.Atan2(this.objectData.velocity.Y, this.objectData.velocity.X);

                transformGroup.Children.Add(new RotateTransform(angleRad * 180 / Math.PI, this.objectData.position.X, this.objectData.position.Y));

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
            }
        }
    }
}
