using System.Windows;
using System.Windows.Media;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Views {
    class CircleVisual : DrawingVisual, IObjectVisual {
        private Circle objectData;
        private Brush brush;
        private Pen pen;
        private float _opacity;

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
                this.brush.Opacity = value;
                this.pen.Brush.Opacity = value;
            }
        }

        public IObject GetObjectData() {
            return this.objectData;
        }

        public void SetObjectData(IObject objectData) {
            if(objectData is Circle circle) {
                this.objectData = circle;
            } else {
                throw new ArgumentException("無効なオブジェクトタイプが渡されました");
            }
        }

        public void Draw() {
            DrawingContext context = this.RenderOpen();

            this.brush = ParseColor.StringToBrush(this.objectData.color);
            this.pen = new Pen(this.brush, 1);

            this.brush.Opacity = this.opacity;
            this.pen.Brush.Opacity = this.opacity;

            context.DrawEllipse(
                this.brush,
                this.pen,
                new Point(this.objectData.position.X, this.objectData.position.Y),
                this.objectData.radius,
                this.objectData.radius
            );

            context.Close();
        }
    }
}
