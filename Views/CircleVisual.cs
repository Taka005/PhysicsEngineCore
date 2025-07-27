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

        public void Draw() {
            DrawingContext context = this.RenderOpen();

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

            context.Close();
        }
    }
}
