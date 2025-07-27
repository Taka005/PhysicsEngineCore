using System.Windows.Media;

namespace PhysicsEngineCore.Views.Interfaces {
    public interface IObjectVisual {
        float opacity { get; set; }

        void DrawOwn();

        void Draw(DrawingContext context);
    }
}