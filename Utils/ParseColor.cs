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

        public static string ColorToString(Color? color) {
            if(color == null) {
                return "#00000000";
            } else {
                return color.Value.ToString();
            }
        }
    }
}
