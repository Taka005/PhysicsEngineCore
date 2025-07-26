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

            double mapStartX = -offsetX / scale;
            double mapStartY = -offsetY / scale;
            double mapEndX = mapStartX + viewportWidth / scale;
            double mapEndY = mapStartY + viewportHeight / scale;

            double startX = Math.Floor(mapStartX / gridInterval) * gridInterval;
            double startY = Math.Floor(mapStartY / gridInterval) * gridInterval;

            for (double posX = startX; posX <= mapEndX; posX += gridInterval) {
                double screenX = (posX + offsetX / scale) * scale;

                context.DrawLine(this.pen, new Point(screenX, 0), new Point(screenX, viewportHeight));
            }

            for (double posY = startY; posY <= mapEndY; posY += gridInterval) {
                double screenY = (posY + offsetY / scale) * scale;

                context.DrawLine(this.pen, new Point(0, screenY), new Point(viewportWidth, screenY));
            }

            context.Close();
        }
    }
}
