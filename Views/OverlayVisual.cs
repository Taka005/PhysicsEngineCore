using System.Windows.Media;
using PhysicsEngineCore.Utils;
using System.Windows;

namespace PhysicsEngineCore.Views {
    public class OverlayVisual : DrawingVisual {
        private readonly List<VectorData> vectors = [];
        private readonly Pen pen = new Pen(Brushes.Black, 1);

        public void UpdateVectors(List<VectorData> vectors) {
            this.vectors.Clear();
            this.vectors.AddRange(vectors);
            this.Draw();
        }

        public void Clear(){
            this.vectors.Clear();
            this.Draw();
        }

        public void Draw() {
            DrawingContext context = this.RenderOpen();

            foreach(VectorData? vectorData in vectors) {
                Point startPoint = new Point(vectorData.position.X, vectorData.position.Y);
                Point endPoint = new Point(vectorData.position.X + vectorData.velocity.X, vectorData.position.Y + vectorData.velocity.Y);

                context.DrawLine(this.pen, startPoint, endPoint);
            }

            context.Close();
        }
    }

    public class VectorData(Vector2 position,Vector2 velocity) {
        public Vector2 position = position;
        public Vector2 velocity = velocity;
    }
}