using System.Windows.Media;

namespace PhysicsEngineCore {
    /// <summary>
    /// Utilityクラス
    /// </summary>
    public static class Utility {
        /// <summary>
        /// 文字列からBrushに変換します
        /// </summary>
        /// <param name="colorString">Hexの色の文字列</param>
        /// <returns>変換したBrush</returns>
        public static Brush ParseColor(string colorString) {
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
    }
}
