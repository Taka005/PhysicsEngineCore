using PhysicsEngineCore.Objects.Interfaces;
using System.Windows.Media;

namespace PhysicsEngineCore.Views.Interfaces {
    public interface IEffectVisual {
        IEffect effectData { get; }

        void Draw(DrawingContext context);
    }
}