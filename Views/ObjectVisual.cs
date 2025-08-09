using System.Windows.Media;
using PhysicsEngineCore.Views.Interfaces;

namespace PhysicsEngineCore.Views {
    public class GroundVisual : DrawingVisual {

        public void Draw(List<IGroundVisual> visuals) {
            using(DrawingContext context = this.RenderOpen()) {
                foreach(IGroundVisual visual in visuals) {
                    visual.Draw(context);
                }
            }
        }
    }
}
