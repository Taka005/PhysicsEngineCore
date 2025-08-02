using System.Windows;
using System.Windows.Media;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Views {
    public class DebugVisual : DrawingVisual{
        public void Clear() {
            DrawingContext context = this.RenderOpen();
            context.Close();
        }

        public void Draw(double fps,Vector2 mousePosition,int objectCount,int groundCount) {
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

            FormattedText objectCountText = new FormattedText(
                $"OBJECT COUNT: {objectCount:F0}",
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                16,
                Brushes.Black,
                VisualTreeHelper.GetDpi(this).PixelsPerDip
            );

            FormattedText groundCountText = new FormattedText(
                $"GROUND COUNT: {groundCount:F0}",
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                16,
                Brushes.Black,
                VisualTreeHelper.GetDpi(this).PixelsPerDip
            );

            context.DrawText(fpsText, new Point(10, 10));
            context.DrawText(mouseText, new Point(10, 30));
            context.DrawText(objectCountText, new Point(10, 50));
            context.DrawText(groundCountText, new Point(10, 70));

            context.Close();
        }
    }
}
