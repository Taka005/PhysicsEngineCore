using System.Windows;
using System.Windows.Media;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Views {
    public class DebugVisual : DrawingVisual{
        public void Clear() {
            DrawingContext context = this.RenderOpen();
            context.Close();
        }

        public void Draw(double fps,Vector2 mousePosition) {
            DrawingContext context = this.RenderOpen();

            FormattedText fpsText = new FormattedText(
                $"FPS: {fps:F0}",
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                16,
                Brushes.Black,
                VisualTreeHelper.GetDpi(this).PixelsPerDip
            );

            FormattedText mouseText = new FormattedText(
                $"MOUSE: ({mousePosition.X:F1},{mousePosition.Y:F1})",
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                16,
                Brushes.Black,
                VisualTreeHelper.GetDpi(this).PixelsPerDip
            );

            context.DrawText(fpsText, new Point(10, 10));
            context.DrawText(mouseText, new Point(10, 30));

            context.Close();
        }
    }
}
