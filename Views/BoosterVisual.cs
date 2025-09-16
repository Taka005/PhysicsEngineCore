using System.Windows;
using System.Windows.Media;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Objects.Interfaces;
using PhysicsEngineCore.Utils;
using PhysicsEngineCore.Views.Interfaces;

namespace PhysicsEngineCore.Views {
    class BoosterVisual : DrawingVisual, IEffectVisual {
        public IEffect effectData { get; }
        private Brush brush;
        private Pen pen;

        public BoosterVisual(Booster effectData) {
            this.effectData = effectData;
            this.brush = ParseColor.StringToBrush(effectData.color);
            this.pen = new Pen(this.brush, 1);
        }

        public void Draw(DrawingContext context) {
            if (this.effectData is Booster booster){
                if (booster.image == null) {
                    this.brush = ParseColor.StringToBrush(booster.color);
                    this.pen = new Pen(this.brush, 3);

                    double minX = Math.Min(booster.start.X, booster.end.X);
                    double minY = Math.Min(booster.start.Y, booster.end.Y);
                    double maxX = Math.Max(booster.start.X, booster.end.X);
                    double maxY = Math.Max(booster.start.Y, booster.end.Y);

                    context.DrawRectangle(null, this.pen, new Rect(minX, minY, maxX - minX, maxY - minY));
                } else {
                    double minX = Math.Min(booster.start.X, booster.end.X);
                    double minY = Math.Min(booster.start.Y, booster.end.Y);
                    double maxX = Math.Max(booster.start.X, booster.end.X);
                    double maxY = Math.Max(booster.start.Y, booster.end.Y);

                    context.DrawImage(booster.image.source, new Rect(minX, minY, maxX - minX, maxY - minY));
                }
            }
        }
    }
}
