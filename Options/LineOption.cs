using System.Text.Json;

namespace PhysicsEngineCore.Options{
    public class LineOption : IOption{
        public string? id { get; set; }
        public string? imageName { get; set; }
        public string color { get; set; } = "#F00000";
        public double startX { get; set; }
        public double startY { get; set; }
        public double endX { get; set; }
        public double endY { get; set; }
        public double width { get; set; }

        public static LineOption? ParseString(string jsonString){
            try{
                return JsonSerializer.Deserialize<LineOption>(jsonString);
            } catch{
                throw new ArgumentException("データの形式が無効です");
            }
        }
    }
}
