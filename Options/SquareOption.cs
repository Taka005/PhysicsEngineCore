using System.Text.Json;

namespace PhysicsEngineCore.Options{
    public class SquareOption() : IOption{
        public string? id { get; set; }
        public string? imageName { get; set; }
        public double posX { get; set; }
        public double posY { get; set; }
        public double size { get; set; }
        public double mass { get; set; }
        public double stiffness { get; set; }
        public double velocityX { get; set; } = 0;
        public double velocityY { get; set; } = 0;
        public string color { get; set; } = "#F00000";
        public List<EntityOption> entities { get; set; } = [];

        public static RopeOption? ParseString(string jsonString){
            try{
                return JsonSerializer.Deserialize<RopeOption>(jsonString);
            }catch{
                throw new ArgumentException("データの形式が無効です");
            }
        }
    }
}