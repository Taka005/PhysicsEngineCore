using PhysicsEngineCore.Objects;

namespace PhysicsEngineCore.Views {
    public interface IObjectVisual {
        float opacity { get; set; }

        IObject GetObjectData();

        void SetObjectData(IObject objectData);

        void Draw();
    }
}