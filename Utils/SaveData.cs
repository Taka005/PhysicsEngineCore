using PhysicsEngineCore.Options;

namespace PhysicsEngineCore.Utils{
    class SaveData{
        public DateTime saveAt { get; set; }
        public string version { get; set; } = Engine.SAVE_DATA_VERSION;

        public EngineOption engine { get; set; } = new EngineOption();
        public ContentManagerOption objects { get; set; } = new ContentManagerOption();
    }
}
