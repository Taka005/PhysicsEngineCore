namespace PhysicsEngineCore.Utils{
    public struct Vector2 : IEquatable<Vector2>{
        public double X;
        public double Y;

        public Vector2(double x, double y){
            this.X = x;
            this.Y = y;
        }

        public static readonly Vector2 Zero = new Vector2(0.0, 0.0);
        public static readonly Vector2 One = new Vector2(1.0, 1.0);
        public static readonly Vector2 UnitX = new Vector2(1.0, 0.0);
        public static readonly Vector2 UnitY = new Vector2(0.0, 1.0);

        public static Vector2 operator +(Vector2 left, Vector2 right){
            return new Vector2(left.X + right.X, left.Y + right.Y);
        }

        public static Vector2 operator -(Vector2 left, Vector2 right){
            return new Vector2(left.X - right.X, left.Y - right.Y);
        }

        public static Vector2 operator -(Vector2 vector){
            return new Vector2(-vector.X, -vector.Y);
        }

        public static Vector2 operator *(Vector2 vector, double scalar){
            return new Vector2(vector.X * scalar, vector.Y * scalar);
        }

        public static Vector2 operator *(double scalar, Vector2 vector){
            return new Vector2(vector.X * scalar, vector.Y * scalar);
        }

        public static Vector2 operator /(Vector2 vector, double scalar){
            return new Vector2(vector.X / scalar, vector.Y / scalar);
        }

        public static bool operator ==(Vector2 left, Vector2 right){
            return Math.Abs(left.X - right.X) < 1e-10 && Math.Abs(left.Y - right.Y) < 1e-10;
        }

        public static bool operator !=(Vector2 left, Vector2 right){
            return !(left == right);
        }

        public double Length(){
            return Math.Sqrt(this.LengthSquared());
        }

        public double LengthSquared(){
            return this.X * this.X + this.Y * this.Y;
        }

        public static double Distance(Vector2 vector1, Vector2 vector2){
            double deltaX = vector2.X - vector1.X;
            double deltaY = vector2.Y - vector1.Y;

            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }

        public static double DistanceSquared(Vector2 vector1, Vector2 vector2){
            double deltaX = vector2.X - vector1.X;
            double deltaY = vector2.Y - vector1.Y;

            return deltaX * deltaX + deltaY * deltaY;
        }

        public static double Dot(Vector2 vector1, Vector2 vector2){
            return vector1.X * vector2.X + vector1.Y * vector2.Y;
        }

        public static double Cross(Vector2 vector1, Vector2 vector2){
            return vector1.X * vector2.Y - vector1.Y * vector2.X;
        }

        public Vector2 Normalized(){
            double length = this.Length();

            if(length > 1e-10){
                return new Vector2(this.X / length, this.Y / length);
            }

            return Vector2.Zero;
        }

        public void Normalize(){
            double length = this.Length();

            if(length > 1e-10){
                this.X /= length;
                this.Y /= length;
            }
        }

        public double Angle(){
            return Math.Atan2(this.Y, this.X);
        }

        public static double AngleBetween(Vector2 from, Vector2 to){
            double dot = Dot(from, to);
            double lengths = from.Length() * to.Length();
            if(lengths < 1e-10) return 0.0;

            return Math.Acos(Math.Clamp(dot / lengths, -1.0, 1.0));
        }

        public Vector2 Perpendicular(){
            return new Vector2(-this.Y, this.X);
        }

        public Vector2 Rotate(double radians){
            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);
            return new Vector2(this.X * cos - this.Y * sin, this.X * sin + this.Y * cos);
        }

        public static Vector2 Lerp(Vector2 start, Vector2 end, double t){
            t = Math.Clamp(t, 0.0, 1.0);

            return new Vector2(start.X + (end.X - start.X) * t, start.Y + (end.Y - start.Y) * t);
        }

        public static Vector2 LerpUnclamped(Vector2 start, Vector2 end, double t){
            return new Vector2(start.X + (end.X - start.X) * t, start.Y + (end.Y - start.Y) * t);
        }

        public static Vector2 Reflect(Vector2 vector, Vector2 normal){
            Vector2 normalizedNormal = normal.Normalized();
            double dot = Dot(vector, normalizedNormal);

            return vector - 2.0 * dot * normalizedNormal;
        }

        public static Vector2 Project(Vector2 vector, Vector2 onto){
            Vector2 normalizedOnto = onto.Normalized();
            double dot = Dot(vector, normalizedOnto);

            return dot * normalizedOnto;
        }

        public Vector2 ClampMagnitude(double maxLength){
            double length = this.Length();

            if(length > maxLength){
                return this.Normalized() * maxLength;
            }

            return this;
        }

        public bool IsZero(){
            return Math.Abs(this.X) < 1e-10 && Math.Abs(this.Y) < 1e-10;
        }

        public bool IsNormalized(){
            return Math.Abs(this.Length() - 1.0) < 1e-10;
        }

        public static Vector2 FromAngle(double radians){
            return new Vector2(Math.Cos(radians), Math.Sin(radians));
        }

        public override string ToString(){
            return $"({this.X:0.###}, {this.Y:0.###})";
        }

        public override bool Equals(object? obj){
            if(obj is Vector2 vector){
                return this == vector;
            }

            return false;
        }

        public bool Equals(Vector2 other){
            return this == other;
        }

        public override int GetHashCode(){
            return HashCode.Combine(this.X, this.Y);
        }

        public Vector2 WithX(double x){
            return new Vector2(x, this.Y);
        }

        public Vector2 WithY(double y){
            return new Vector2(this.X, y);
        }

        public static Vector2 Min(Vector2 a, Vector2 b){
            return new Vector2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
        }

        public static Vector2 Max(Vector2 a, Vector2 b){
            return new Vector2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
        }

        public static Vector2 Abs(Vector2 value){
            return new Vector2(Math.Abs(value.X), Math.Abs(value.Y));
        }

        public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max){
            return new Vector2(
                Math.Clamp(value.X, min.X, max.X),
                Math.Clamp(value.Y, min.Y, max.Y)
            );
        }

        public bool IsNaN(){
            return double.IsNaN(this.X) || double.IsNaN(this.Y);
        }

        public bool IsFinite(){
            return double.IsFinite(this.X) && double.IsFinite(this.Y);
        }

        public bool NearlyEquals(Vector2 other, double epsilon = 1e-10){
            return Math.Abs(this.X - other.X) < epsilon &&
                   Math.Abs(this.Y - other.Y) < epsilon;
        }

        public void Deconstruct(out double x, out double y){
            x = this.X;
            y = this.Y;
        }

        public double RadianTo(Vector2 target){
            return Math.Atan2(target.Y - this.Y, target.X - this.X);
        }

        public double DegreeTo(Vector2 target){
            return this.RadianTo(target) * (180.0 / Math.PI);
        }
    }
}
