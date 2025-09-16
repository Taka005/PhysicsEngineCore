using PhysicsEngineCore.Objects.Interfaces;
using System.Windows.Media;

namespace PhysicsEngineCore.Views.Interfaces {
    public interface IGroundVisual {
        IGround groundData { get; }

        void Draw(DrawingContext context);
    }
}