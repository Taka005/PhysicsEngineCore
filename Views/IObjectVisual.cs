using PhysicsEngineCore.Objects;

namespace PhysicsEngineCore.Views {
    public interface IObjectVisual {
        IObject GetObjectData();

        void SetObjectData(IObject objectData);

        void Draw();
    }
}