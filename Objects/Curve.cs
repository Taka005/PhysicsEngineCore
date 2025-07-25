﻿using System.Text.Json;
using System.Windows.Shapes;
using PhysicsEngineCore.Objects.Interfaces;
using PhysicsEngineCore.Options;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore.Objects {
    /// <summary>
    /// 曲線を表すクラス
    /// </summary>
    public class Curve : IGround {
        private string _id;
        private readonly string _trackingId = IdGenerator.CreateId(15);
        private string _color;
        public Vector2 start;
        public Vector2 middle;
        public Vector2 center;
        public Vector2 end;
        public double radius;
        private double _width;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="option">曲線の初期化クラス</param>
        public Curve(CurveOption option) {
            this._id = option.id ?? throw new ArgumentException(nameof(option.id));
            this._color = option.color;
            this.start = new Vector2(option.startX, option.startY);
            this.middle = new Vector2(option.middleX, option.middleY);
            this.end = new Vector2(option.endX, option.endY);
            this._width = CheckWidthValue(option.width);

            double slope1 = (option.middleX - option.startX) / (option.startY - option.middleY);
            double slope2 = (option.endX - option.middleX) / (option.middleY - option.endY);
            double equat1 = (option.startY + option.middleY) / 2 - slope1 * ((option.startX + option.middleX) / 2);
            double equat2 = (option.middleY + option.endY) / 2 - slope2 * ((option.middleX + option.endX) / 2);

            double centerX = (equat2 - equat1) / (slope1 - slope2);
            double centerY = slope1 * centerX + equat1;

            this.center = new Vector2(centerX, centerY);
            this.radius = Vector2.Distance(this.start, this.center);
        }

        /// <summary>
        /// オブジェクトの固有ID
        /// </summary>
        public string id {
            get {
                return this._id;
            }
        }

        /// <summary>
        /// クラスの固有ID
        /// トラッキング用
        /// </summary>
        public string trackingId {
            get{
                return this._trackingId;
            }
        }

        /// <summary>
        /// オブジェクトの色
        /// Hexの値です
        /// </summary>
        public string color {
            get {
                return this._color;
            }
            set {
                this._color = value;
            }
        }

        /// <summary>
        /// 線の幅
        /// </summary>
        public double width {
            get {
                return this._width;
            }
            set {
                this._width = CheckWidthValue(value);
            }
        }

        /// <summary>
        /// クラスのデータをJSON形式の文字列に変換します
        /// </summary>
        /// <returns>JSON形式の文字列</returns>
        public string ToJson() {
            return JsonSerializer.Serialize(this.ToOption());
        }

        /// <summary>
        /// 同じ状態のクラスを複製します
        /// </summary>
        /// <returns>複製されたクラス</returns>
        public IGround Clone() {
            return new Curve(this.ToOption());
        }

        
        /// <summary>
        /// 他のオブジェクトと同じ状態かどうかを比較します
        /// </summary>
        /// <param name="target">対象のオブジェクト</param>
        /// <returns>同じかどうか</returns>
        public bool Equals(IGround target) {
            if(target is not Curve curve) return false;

            return this.id == curve.id &&
                   this.color == curve.color &&
                   this.width == curve.width &&
                   this.start.Equals(curve.start) && 
                   this.middle.Equals(curve.middle) &&
                   this.end.Equals(curve.end);
        }

        /// <summary>
        /// クラスの引数に変換します
        /// </summary>
        /// <returns>クラスの引数</returns>
        public CurveOption ToOption() {
            return new CurveOption {
                id = this.id,
                color = this.color,
                startX = this.start.X,
                startY = this.start.Y,
                middleX = this.middle.X,
                middleY = this.middle.Y,
                endX = this.end.X,
                endY = this.end.Y,
                width = this.width
            };
        }


        /// <summary>
        /// ある点から曲線に垂直で下した時の交点を返します
        /// 曲線の端のほうが近い場合は、端の座標を返します
        /// </summary>
        /// <param name="position">任意の座標</param>
        /// <returns>交差した座標</returns>
        public Vector2 SolvePosition(Vector2 position) {
            Vector2 difference = position - this.center;

            double distance = difference.Length();
            if(distance == 0) return this.start;

            double scale = this.radius / distance;

            Vector2 cross = this.center + difference * scale;

            double startAngle = NormalizeAngle(Math.Atan2(this.start.Y - this.center.Y, this.start.X - this.center.X));
            double midAngle = NormalizeAngle(Math.Atan2(this.middle.Y - this.center.Y, this.middle.X - this.center.X));
            double endAngle = NormalizeAngle(Math.Atan2(this.end.Y - this.center.Y, this.end.X - this.center.X));
            double crossAngle = NormalizeAngle(Math.Atan2(cross.Y - this.center.Y, cross.X - this.center.X));

            bool isClockwise = (startAngle > endAngle) ? (midAngle > startAngle || midAngle < endAngle) : (midAngle > startAngle && midAngle < endAngle);

            if(!IsAngleBetween(crossAngle, startAngle, endAngle, isClockwise)) {
                double startDistance = Vector2.DistanceSquared(this.start, position);
                double endDistance = Vector2.DistanceSquared(this.end, position);

                if(startDistance < endDistance) {
                    return this.start;
                } else {
                    return this.end;
                }
            }

            return cross;
        }

        /// <summary>
        /// 指定された角度に入っているかをチェックします
        /// </summary>
        /// <param name="angle">判定する角度</param>
        /// <param name="start">開始角度</param>
        /// <param name="end">終了角度</param>
        /// <param name="isClockwise">時計周りかどうか</param>
        /// <returns>時計回りなら真を返す</returns>
        public static bool IsAngleBetween(double angle, double start, double end, bool isClockwise) {
            if(isClockwise) {
                return (angle >= start && angle <= end) || (start > end && (angle >= start || angle <= end));
            } else {
                return (angle <= start && angle >= end) || (start < end && (angle <= start || angle >= end));
            }
        }

        /// <summary>
        /// 角度を正規化します
        /// </summary>
        /// <param name="angle">正規化する角度</param>
        /// <returns>正規化された角度</returns>
        public static double NormalizeAngle(double angle) {
            return (angle + 2 * Math.PI) % (2 * Math.PI);
        }

        /// <summary>
        /// 幅が正しい値かチェックします
        /// </summary>
        /// <param name="width">幅</param>
        /// <returns>正しい幅</returns>
        /// <exception cref="Exception">0未満であったときに例外</exception>
        private static double CheckWidthValue(double width) {
            if(width < 0) throw new Exception("厚さ(width)は0以上に設定する必要があります");

            return width;
        }
    }
}
