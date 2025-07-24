using PhysicsEngineCore.Objects;

namespace PhysicsEngineCore.Views {
    public interface IEffectVisual {

        IEffect GetEffectData();

        void SetEffectData(IEffect effectData);

        void Draw();
    }
}