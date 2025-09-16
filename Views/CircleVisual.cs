using System.Windows;
using System.Windows.Media;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Objects.Interfaces;
using PhysicsEngineCore.Utils;
using PhysicsEngineCore.Views.Interfaces;

namespace PhysicsEngineCore.Views {
    class CircleVisual : DrawingVisual, IObjectVisual {
        public IObject objectData { get; }
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
            if(this.objectData is Circle circle){
                if (circle.image == null) {
                    this.brush = ParseColor.StringToBrush(circle.color);

                    this.brush.Opacity = this.opacity;

                    this.pen = new Pen(this.brush, 1);

                    context.DrawEllipse(
                        this.brush,
                        this.pen,
                        new Point(circle.position.X, circle.position.Y),
                        circle.radius - 0.5,
                        circle.radius - 0.5
                    );
                } else {
                    TransformGroup transformGroup = new TransformGroup();

                    double angleRad = Math.Atan2(circle.velocity.Y, -circle.velocity.X);

                    transformGroup.Children.Add(new RotateTransform(angleRad * 180 / Math.PI, circle.position.X, circle.position.Y));

                    context.PushTransform(transformGroup);
                    context.PushOpacity(this.opacity);
                    context.PushClip(new EllipseGeometry(new Point(circle.position.X, circle.position.Y), circle.radius, circle.radius));

                    context.DrawImage(
                        circle.image.source,
                        new Rect(
                            circle.position.X - circle.radius,
                            circle.position.Y - circle.radius,
                            circle.diameter,
                            circle.diameter
                        )
                    );

                    context.Pop();
                    context.Pop();
                    context.Pop();
                }
            }
        }
    }
}
