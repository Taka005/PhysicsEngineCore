using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Options;

namespace PhysicsEngineCore.Utils{
    class SaveData{
        public DateTime saveAt { get; set; }
        public string version { get; set; } = "1";

        public EngineOption? engine { get; set; }
        public List<IObject> objects { get; set; } = [];
        public List<IGround> grounds { get; set; } = [];
    }
}
