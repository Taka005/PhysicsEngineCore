using PhysicsEngineCore.Objects.Interfaces;
using System.Windows.Media;

namespace PhysicsEngineCore.Views.Interfaces {
    public interface IObjectVisual {
        IObject objectData { get; }

        float opacity { get; set; }

        void DrawOwn();

        void Draw(DrawingContext context);
    }
}