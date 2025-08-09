using System.Windows;
using System.Windows.Media;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Utils;
using PhysicsEngineCore.Views.Interfaces;

namespace PhysicsEngineCore.Views {
    class CircleVisual : DrawingVisual, IObjectVisual {
        private Circle objectData;
        private Brush brush;
        private Pen pen;
        private float _opacity = 1;

        public CircleVisual(Circle objectData) {
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

                this.pen = new Pen(this.brush, 1);

                context.DrawEllipse(
                    this.brush,
                    this.pen,
                    new Point(this.objectData.position.X, this.objectData.position.Y),
                    this.objectData.radius - 0.5,
                    this.objectData.radius - 0.5
                );
            } else {
                TransformGroup transformGroup = new TransformGroup();

                double angleRad = Math.Atan2(this.objectData.velocity.Y, -this.objectData.velocity.X);

                transformGroup.Children.Add(new RotateTransform(angleRad * 180 / Math.PI, this.objectData.position.X, this.objectData.position.Y));

                context.PushTransform(transformGroup);
                context.PushOpacity(this.opacity);
                context.PushClip(new EllipseGeometry(new Point(this.objectData.position.X, this.objectData.position.Y), this.objectData.radius, this.objectData.radius));

                context.DrawImage(
                    this.objectData.image.source,
                    new Rect(
                        this.objectData.position.X - this.objectData.radius,
                        this.objectData.position.Y - this.objectData.radius,
                        this.objectData.diameter,
                        this.objectData.diameter
                    )
                );

                context.Pop();
                context.Pop();
                context.Pop();
            }
        }
    }
}
