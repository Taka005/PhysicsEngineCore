using PhysicsEngineCore.Objects;

namespace PhysicsEngineCore.Views {
    public interface IGroundVisual {

        IGround GetGroundData();

        void SetGroundData(IGround groundData);

        void Draw();
    }
}