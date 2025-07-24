using System.Windows.Media;
using PhysicsEngineCore.Utils;
using System.Windows;

namespace PhysicsEngineCore.Views {
    public class VectorVisual : DrawingVisual {
        private readonly Pen pen = new Pen(Brushes.Black, 1);

        public void Clear() {
            DrawingContext context = this.RenderOpen();
            context.Close();
        }

        public void Draw(List<VectorData> vectors) {
            DrawingContext context = this.RenderOpen();

            foreach(VectorData? vectorData in vectors) {
                if(vectorData.velocity.IsZero()) continue;

                Point startPoint = new Point(vectorData.position.X, vectorData.position.Y);
                Point endPoint = new Point(vectorData.position.X + vectorData.velocity.X, vectorData.position.Y + vectorData.velocity.Y);

                context.DrawLine(this.pen, startPoint, endPoint);
            }

            context.Close();
        }
    }

    public class VectorData(Vector2 position, Vector2 velocity) {
        public Vector2 position = position;
        public Vector2 velocity = velocity;
    }
}