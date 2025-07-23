using System.Windows;
using System.Windows.Media;

namespace PhysicsEngineCore.Views {
    public class DebugVisual : DrawingVisual{
        public void Draw(double fps) {
            DrawingContext context = this.RenderOpen();

            FormattedText formattedText = new FormattedText(
                $"FPS: {fps:F0}",
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                16,
                Brushes.Black,
                VisualTreeHelper.GetDpi(this).PixelsPerDip
            );

            Point textLocation = new Point(10, 10);

            context.DrawText(formattedText, textLocation);

            context.Close();
        }
    }
}
