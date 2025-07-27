using System.Windows.Media;
using PhysicsEngineCore.Views.Interfaces;

namespace PhysicsEngineCore.Views {
    public class ObjectVisual : DrawingVisual {

        public void Draw(List<IObjectVisual> visuals) {
            DrawingContext context = this.RenderOpen();

            foreach(IObjectVisual visual in visuals) {
                visual.Draw(context);
            }

            context.Close();
        }
    }
}
