using System.Windows.Media;
using System.Windows;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Views.Interfaces;

namespace PhysicsEngineCore.Views{
    public class GroundVisual : DrawingVisual{
        public bool isDrawEdge = false;

        private readonly Pen edgePen = new Pen(Brushes.Black, 1);

        public void Draw(List<IGroundVisual> visuals){
            using (DrawingContext context = this.RenderOpen()){
                foreach (IGroundVisual visual in visuals){
                    visual.Draw(context);

                    if (this.isDrawEdge){
                        if(visual.groundData is Line line){
                            context.DrawEllipse(
                                null,
                                this.edgePen,
                                new Point(line.start.X, line.start.Y),
                                line.width / 2 - 0.5,
                                line.width / 2 - 0.5
                            );

                            context.DrawEllipse(
                                null,
                                this.edgePen,
                                new Point(line.end.X, line.end.Y),
                                line.width / 2 - 0.5,
                                line.width / 2 - 0.5
                            );
                        }else if(visual.groundData is Curve curve){
                            context.DrawEllipse(
                                null,
                                this.edgePen,
                                new Point(curve.start.X, curve.start.Y),
                                curve.width / 2 - 0.5,
                                curve.width / 2 - 0.5
                            );

                            context.DrawEllipse(
                                null,
                                this.edgePen,
                                new Point(curve.middle.X, curve.middle.Y),
                                curve.width / 2 - 0.5,
                                curve.width / 2 - 0.5
                            );

                            context.DrawEllipse(
                                null,
                                this.edgePen,
                                new Point(curve.center.X, curve.center.Y),
                                curve.width / 2 - 0.5,
                                curve.width / 2 - 0.5
                            );

                            context.DrawEllipse(
                                null,
                                this.edgePen,
                                new Point(curve.end.X, curve.end.Y),
                                curve.width / 2 - 0.5,
                                curve.width / 2 - 0.5
                            );
                        }
                    }
                }
            }
        }
    }
}
