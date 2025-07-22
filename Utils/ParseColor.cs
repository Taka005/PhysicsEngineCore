using System.Windows.Media;

namespace PhysicsEngineCore.Utils {
    public class ParseColor {
        public static Brush StringToBrush(string colorString) {
            Brush fillBrush;
            try {
                BrushConverter brushConverter = new BrushConverter();
                Brush? convertedBrush = brushConverter.ConvertFromString(colorString) as Brush;

                fillBrush = convertedBrush ?? Brushes.Transparent;
            } catch {
                fillBrush = Brushes.Transparent;
            }

            return fillBrush;
        }

        public static Color StringToColor(string colorString) {
            Color color;
            try {
                BrushConverter brushConverter = new BrushConverter();
                Brush? convertedBrush = brushConverter.ConvertFromString(colorString) as Brush;

                if(convertedBrush is SolidColorBrush solidColorBrush) {
                    color = solidColorBrush.Color;
                } else {
                    color = Colors.Transparent;
                }
            } catch {
                color = Colors.Transparent;
            }

            return color;
        }
    }
}
