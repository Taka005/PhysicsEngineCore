﻿using PhysicsEngineCore.Options;

namespace PhysicsEngineCore.Utils{
    public class SaveData{
        public DateTime saveAt { get; set; }
        public string version { get; set; } = Engine.SAVE_DATA_VERSION;

        public EngineOption engine { get; set; } = new EngineOption();
        public ObjectSaveData objects { get; set; } = new ObjectSaveData();
    }

     public class ObjectSaveData {
        public List<CircleOption> circles { get; set; } = [];
        public List<SquareOption> squares { get; set; } = [];
        public List<TriangleOption> triangles { get; set; } = [];
        public List<RopeOption> ropes { get; set; } = [];
        public List<LineOption> lines { get; set; } = [];
        public List<CurveOption> curves { get; set; } = [];
    }
}
