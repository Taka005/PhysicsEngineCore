using System.Windows;
using System.Windows.Media;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Views {
    class SquareVisual : DrawingVisual, IObjectVisual {
        private Square objectData;
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

        public IObject GetObjectData() {
            return this.objectData;
        }

        public void SetObjectData(IObject objectData) {
            if(objectData is Square square) {
                this.objectData = square;
            }else{
                throw new ArgumentException("無効なオブジェクトタイプが渡されました");
            }
        }

        public void Draw() {
            DrawingContext context = this.RenderOpen();

            this.brush = ParseColor.StringToBrush(this.objectData.color);

            this.brush.Opacity = this.opacity;

            this.pen = new Pen(this.brush, this.objectData.size / 2);

            this.objectData.entities.ForEach(source => {
                context.DrawEllipse(
                     this.brush,
                     null,
                     new Point(source.position.X, source.position.Y),
                     source.radius - 0.5,
                     source.radius - 0.5
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
