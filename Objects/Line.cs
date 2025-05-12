using PhysicsEngineCore.Options;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects{
    public class Line: IGround{
        public readonly string id;
        public readonly string type = "line";
        public string color;
        public Vector2 start;
        public Vector2 end;
        private double _width;

        Line(LineOption option){
            this.id = option.id ?? throw new ArgumentException(nameof(option.id));
            this.color = option.color;
            this.start = new Vector2(option.startX, option.startY);
            this.end = new Vector2(option.endX, option.endY);
            this._width = CheckWidthValue(option.width); 
        }

        public double width{
            get{
                return this._width;
            }
            set{
                this._width = CheckWidthValue(value);
            }
        }

        public double lenght{
            get{
                return Vector2.Distance(this.start,this.end);
            }
        }

        public IGround Clone(){
            LineOption lineOption = new LineOption{
                id = this.id,
                color = this.color,
                startX = this.start.X,
                startY = this.start.Y,
                endX = this.end.X,
                endY = this.end.Y,
                width = this.width,
            };

            return new Line(lineOption);
        }

        public Vector2 SolvePosition(Vector2 position){
            double flag = ((position.X - this.start.X) * (this.end.X - this.start.X) + (position.Y - this.start.Y) * (this.end.Y - this.start.Y)) / Vector2.DistanceSquared(this.start, this.end);

            if(flag <= 0){
                return this.start;
            }else if(flag >= 1){
                return this.end;
            }else{
                double crossX = this.start.X + flag * (this.end.X - this.start.X);
                double crossY = this.start.Y + flag * (this.end.Y - this.start.Y);

                return new Vector2(crossX, crossY);
            }
        }

        private static double CheckWidthValue(double width){
            if(width < 0) throw new Exception("厚さ(width)は0以上に設定する必要があります");

            return width;
        }
    }
}
