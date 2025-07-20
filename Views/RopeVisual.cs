using System.Windows;
using System.Windows.Media;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Views {
    class RopeVisual : DrawingVisual, IObjectVisual {
        private Rope objectData;
        private Brush brush;
        private Pen pen;
        private float _opacity;

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
                this.brush.Opacity = value;
            }
        }

        public IObject GetObjectData() {
            return this.objectData;
        }

        public void SetObjectData(IObject objectData) {
            if(objectData is Rope rope) {
                this.objectData = rope;
            } else {
                throw new ArgumentException("無効なオブジェクトタイプが渡されました");
            }
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