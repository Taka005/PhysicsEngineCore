using System.Windows;
using System.Windows.Media;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Objects.Interfaces;
using PhysicsEngineCore.Utils;
using PhysicsEngineCore.Views.Interfaces;

namespace PhysicsEngineCore.Views {
    class RopeVisual : DrawingVisual, IObjectVisual {
        public IObject objectData { get; }
        private Brush brush;
        private Pen pen;
        private float _opacity = 1;

        public RopeVisual(Rope objectData) {
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
            if (this.objectData is Rope rope){
                if (rope.image == null) {
                    this.brush = ParseColor.StringToBrush(rope.color);
           
                    this.brush.Opacity = this.opacity;

                    this.pen = new Pen(this.brush, rope.width);

                    Entity? target = null;

                    rope.entities.ForEach(entity => {
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
                } else {
                    context.PushOpacity(this.opacity);

                    rope.entities.ForEach(entity => {
                        TransformGroup transformGroup = new TransformGroup();

                        double angleRad = Math.Atan2(rope.velocity.Y, -rope.velocity.X);

                        transformGroup.Children.Add(new RotateTransform(angleRad * 180 / Math.PI, entity.position.X, entity.position.Y));

                        context.PushTransform(transformGroup);
                        context.PushClip(new EllipseGeometry(new Point(entity.position.X, entity.position.Y), entity.radius, entity.radius));

                        context.DrawImage(
                            rope.image.source,
                            new Rect(
                                entity.position.X - entity.radius,
                                entity.position.Y - entity.radius,
                                entity.diameter,
                                entity.diameter
                            )
                        );

                        context.Pop();
                        context.Pop();
                    });

                    context.Pop();
                }
            }
        }
    }
}