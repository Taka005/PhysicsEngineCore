using System.Windows.Media;

namespace PhysicsEngineCore.Objects{
    public interface IObject: IBaseObject{
        string id { get; }
        string color { get; set; }
        DrawingVisual visual { get; }

        void Draw();
        IObject Clone();

        string ToJson();
    }
}
