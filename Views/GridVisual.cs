using System.Windows.Media;
using System.Windows;

namespace PhysicsEngineCore.Views {
    public class GridVisual : DrawingVisual{
        private readonly Pen pen = new Pen(Brushes.LightGray,0.5);

        public void Clear() {
            DrawingContext context = this.RenderOpen();
            context.Close();
        }

        public void Draw(double gridInterval,double scale,double offsetX,double offsetY) {
            DrawingContext context = this.RenderOpen();

            double viewportWidth = 800;
            double viewportHeight = 800;

            double worldStartX = -offsetX / scale;
            double worldStartY = -offsetY / scale;
            double worldEndX = worldStartX + viewportWidth / scale;
            double worldEndY = worldStartY + viewportHeight / scale;

            double startX = Math.Floor(worldStartX / gridInterval) * gridInterval;
            double startY = Math.Floor(worldStartY / gridInterval) * gridInterval;

            for (double posX = startX; posX <= worldEndX; posX += gridInterval) {
                double screenX = (posX + offsetX / scale) * scale;
                context.DrawLine(this.pen, new Point(screenX, 0), new Point(screenX, viewportHeight));
            }

            for (double posY = startY; posY <= worldEndY; posY += gridInterval) {
                double screenY = (posY + offsetY / scale) * scale;
                context.DrawLine(this.pen, new Point(0, screenY), new Point(viewportWidth, screenY));
            }

            context.Close();
        }
    }
}
