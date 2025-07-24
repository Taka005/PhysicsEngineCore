using PhysicsEngineCore.Options;

namespace PhysicsEngineCore.Utils {
    public class SaveData {
        public DateTime saveAt { get; set; }
        public string version { get; set; } = Engine.SAVE_DATA_VERSION;

        public EngineOption engine { get; set; } = new EngineOption();
        public ObjectSaveData objects { get; set; } = new ObjectSaveData();

        public List<IOption> GetAllObjects() {
            List<IOption> allObjects = [];

            allObjects.AddRange(this.objects.circles);
            allObjects.AddRange(this.objects.squares);
            allObjects.AddRange(this.objects.triangles);
            allObjects.AddRange(this.objects.ropes);

            return allObjects;
        }

        public List<IOption> GetAllGrounds() {
            List<IOption> allGrounds = [];

            allGrounds.AddRange(this.objects.lines);
            allGrounds.AddRange(this.objects.curves);

            return allGrounds;
        }

        public List<IOption> GetAllEffects() {
            List<IOption> allEffects = [];

            allEffects.AddRange(this.objects.boosters);

            return allEffects;
        }
    }

    public class ObjectSaveData {
        public List<CircleOption> circles { get; set; } = [];
        public List<SquareOption> squares { get; set; } = [];
        public List<TriangleOption> triangles { get; set; } = [];
        public List<RopeOption> ropes { get; set; } = [];
        public List<LineOption> lines { get; set; } = [];
        public List<CurveOption> curves { get; set; } = [];
        public List<BoosterOption> boosters { get; set; } = [];
    }
}
