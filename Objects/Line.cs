using System.Numerics;

namespace PhysicsEngineCore.Objects{
    class Line: IGround{
        public string name;
        public string type = "line";
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

        public float lenght{
            get{
                return Vector2.Distance(this.start,this.end);
            }
        }

        public IGround clone(){
            return new Line(this.name, this.color, this.start.X, this.start.Y, this.end.X, this.end.Y, this.thickness);
        }

        public Vector2 solvePosition(Vector2 position){
            float flag = ((position.X - this.start.X) * (this.end.X - this.start.X) + (position.Y - this.start.Y) * (this.end.Y - this.start.Y)) / (float)Math.Pow(lenght,2);

            if(flag <= 0){
                return this.start;
            }else if(flag >= 1){
                return this.end;
            }else{
                float crossX = this.start.X + flag * (this.end.X - this.start.X);
                float crossY = this.start.Y + flag * (this.end.Y - this.start.Y);

                return new Vector2(crossX, crossY);
            }
        }
    }
}
