using System.Numerics;

namespace PhysicsEngineCore.Objects{
    class Line{
        public string name;
        public string color;
        public Vector2 start;
        public Vector2 end;
        public float thickness;

        public Line(string name, string color, float startX, float startY, float endX, float endY, float thickness){
            this.name = name;
            this.color = color;
            this.start = new Vector2(startX, startY);
            this.end = new Vector2(endX, endY);
            this.thickness = thickness; 
        }
    }
}
