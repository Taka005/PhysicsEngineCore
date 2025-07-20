using System.Windows;
using System.Windows.Media;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Views {
    class TriangleVisual : DrawingVisual, IObjectVisual {
        private Triangle objectData;
        private Brush brush;
        private Pen pen;
        private float _opacity;

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
                this.brush.Opacity = value;
                this.pen.Brush.Opacity = value;
            }
        }

        public IObject GetObjectData() {
            return this.objectData;
        }

        public void SetObjectData(IObject objectData) {
            if(objectData is Triangle triangle) {
                this.objectData = triangle;
            }else{
                throw new ArgumentException("無効なオブジェクトタイプが渡されました");
            }
        }

        public void Draw() {
            DrawingContext context = this.RenderOpen();

            this.brush = ParseColor.StringToBrush(this.objectData.color);
            this.pen = new Pen(this.brush, this.objectData.size / 2);

            this.brush.Opacity = this.opacity;
            this.pen.Brush.Opacity = this.opacity;

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
