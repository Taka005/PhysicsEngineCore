using System.Windows;
using System.Windows.Media;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Objects.Interfaces;
using PhysicsEngineCore.Utils;
using PhysicsEngineCore.Views.Interfaces;

namespace PhysicsEngineCore.Views {
    class SquareVisual : DrawingVisual, IObjectVisual {
        public IObject objectData { get; }
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
            if (this.objectData is Square square){
                if (square.image == null) {
                    this.brush = ParseColor.StringToBrush(square.color);

                    this.brush.Opacity = this.opacity;

                    this.pen = new Pen(this.brush, square.size / 2);

                    square.entities.ForEach(source => {
                        context.DrawEllipse(
                             this.brush,
                             null,
                             new Point(source.position.X, source.position.Y),
                             source.radius,
                             source.radius
                         );

                        square.entities.ForEach(target => {
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

                    Entity start = square.entities[0];
                    Entity end = square.entities[2];

                    double angle = (start.position - end.position).Angle();

                    angle += Math.PI;

                    transformGroup.Children.Add(new RotateTransform(angle * 180 / Math.PI, square.position.X, square.position.Y));

                    context.PushTransform(transformGroup);
                    context.PushOpacity(this.opacity);

                    context.DrawImage(
                        square.image.source,
                        new Rect(
                            square.position.X - square.size/2,
                            square.position.Y - square.size/2,
                            square.size,
                            square.size
                        )
                    );

                    context.Pop();
                    context.Pop();
                }
            }
        }
    }
}
