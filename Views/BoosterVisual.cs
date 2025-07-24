using System.Windows;
using System.Windows.Media;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Utils;

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

        public IEffect GetEffectData() {
            return this.effectData;
        }

        public void SetEffectData(IEffect effectData) {
            if(effectData is Booster booster) {
                this.effectData = booster;
            } else {
                throw new ArgumentException("無効なオブジェクトタイプが渡されました");
            }
        }

        public void Draw() {
            DrawingContext context = this.RenderOpen();

            this.brush = ParseColor.StringToBrush(this.effectData.color);
            this.pen = new Pen(this.brush, 3);

       

            context.Close();
        }
    }
}
