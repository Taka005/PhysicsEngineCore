using System.Windows;
using System.Windows.Media;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Utils;
using PhysicsEngineCore.Views.Interfaces;

namespace PhysicsEngineCore.Views {
    class BoosterVisual : DrawingVisual, IEffectVisual {
        private Booster effectData;
        private Brush brush;
        private Pen pen;

        public BoosterVisual(Booster effectData) {
            this.effectData = effectData;
            this.brush = ParseColor.StringToBrush(effectData.color);
            this.pen = new Pen(this.brush, 1);
        }

        public void Draw(DrawingContext context) {
            if(this.effectData.image == null) {
                this.brush = ParseColor.StringToBrush(this.effectData.color);
                this.pen = new Pen(this.brush, 3);

                double minX = Math.Min(this.effectData.start.X, this.effectData.end.X);
                double minY = Math.Min(this.effectData.start.Y, this.effectData.end.Y);
                double maxX = Math.Max(this.effectData.start.X, this.effectData.end.X);
                double maxY = Math.Max(this.effectData.start.Y, this.effectData.end.Y);

                context.DrawRectangle(null, this.pen, new Rect(minX, minY, maxX - minX, maxY - minY));
            }
        }
    }
}
