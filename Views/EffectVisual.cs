using System.Windows.Media;
using PhysicsEngineCore.Views.Interfaces;

namespace PhysicsEngineCore.Views {
    public class EffectVisual : DrawingVisual {

        public void Draw(List<IEffectVisual> visuals) {
            DrawingContext context = this.RenderOpen();

            foreach(IEffectVisual visual in visuals) {
                visual.Draw(context);
            }

            context.Close();
        }
    }
}
