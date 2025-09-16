using System.Windows.Media;
using System.Windows;
using PhysicsEngineCore.Views.Interfaces;

namespace PhysicsEngineCore.Views{
    public class ObjectVisual : DrawingVisual{
        public bool isDrawVector = false;

        private readonly Pen vectorPen = new Pen(Brushes.Black, 1);

        public void Draw(List<IObjectVisual> visuals){
            using (DrawingContext context = this.RenderOpen()){
                foreach (IObjectVisual visual in visuals){
                    visual.Draw(context);

                    if(this.isDrawVector){
                        if (visual.objectData.velocity.IsZero()) continue;

                        Point startPoint = new Point(visual.objectData.position.X, visual.objectData.position.Y);
                        Point endPoint = new Point(visual.objectData.position.X + visual.objectData.velocity.X, visual.objectData.position.Y + visual.objectData.velocity.Y);

                        context.DrawLine(this.vectorPen, startPoint, endPoint);
                    }
                }
            }
        }
    }
}
