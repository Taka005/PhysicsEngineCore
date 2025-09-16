using System.Windows;
using System.Windows.Media;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Objects.Interfaces;
using PhysicsEngineCore.Utils;
using PhysicsEngineCore.Views.Interfaces;

namespace PhysicsEngineCore.Views {
    class TriangleVisual : DrawingVisual, IObjectVisual {
        public IObject objectData { get; }
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
            using(DrawingContext context = this.RenderOpen()){
                this.Draw(context);
            }
        }

        public void Draw(DrawingContext context) {
            if (this.objectData is Triangle triangle){
                if (triangle.image == null) {
                    this.brush = ParseColor.StringToBrush(triangle.color);

                    this.brush.Opacity = this.opacity;

                    this.pen = new Pen(this.brush, triangle.entityDiameter);

                    triangle.entities.ForEach(source => {
                        context.DrawEllipse(
                             this.brush,
                             null,
                             new Point(source.position.X, source.position.Y),
                             source.radius,
                             source.radius
                         );

                        triangle.entities.ForEach(target => {
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

                    Entity start = triangle.entities[1];
                    Entity end = triangle.entities[2];

                    double angle = (start.position - end.position).Angle();

                    angle += Math.PI;

                    transformGroup.Children.Add(new RotateTransform(angle * 180 / Math.PI, triangle.position.X, triangle.position.Y));

                    context.PushTransform(transformGroup);
                    context.PushOpacity(this.opacity);

                    context.DrawImage(
                        triangle.image.source,
                        new Rect(
                            triangle.position.X - triangle.size/2,
                            triangle.position.Y - triangle.size/2,
                            triangle.size,
                            triangle.size
                        )
                    );

                    context.Pop();
                    context.Pop();
                }
            }
        }
    }
}
