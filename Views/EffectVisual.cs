using System.Windows.Media;
using System.Windows;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Views.Interfaces;

namespace PhysicsEngineCore.Views {
    public class EffectVisual : DrawingVisual {

        public bool isDrawEdge = false;

        private readonly Pen edgePen = new Pen(Brushes.Black, 1);

        public void Draw(List<IEffectVisual> visuals) {
            using(DrawingContext context = this.RenderOpen()) {
                foreach(IEffectVisual visual in visuals) {
                    visual.Draw(context);

                    if (this.isDrawEdge){
                        if (visual.effectData is Booster booster){
                            context.DrawEllipse(
                                null,
                                this.edgePen,
                                new Point(booster.start.X, booster.start.Y),
                                1.5 - 0.5,
                                1.5 - 0.5
                            );

                            context.DrawEllipse(
                                null,
                                this.edgePen,
                                new Point(booster.end.X, booster.end.Y),
                                1.5 - 0.5,
                                1.5 - 0.5
                            );
                        }
                    }
                }
            }
        }
    }
}
