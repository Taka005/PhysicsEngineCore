using System.Diagnostics;
using System.Text.Json;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Options;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore {
    public class Engine : Core {
        public readonly static string SAVE_DATA_VERSION = "1";
        private bool isStarted = false;
        public bool isTrackingMode = false;
        public bool isDebugMode = false;
        private readonly Timer loopTimer;
        private float playBackSpeed = 1;
        private float trackingInterval = 100;
        private int trackingCount = 0;
        private int trackingLimit = 50000;
        private int movementLimit = 10000;
        private readonly List<IObject> tracks = [];
        private readonly ContentManager content = new ContentManager();

        public Engine(EngineOption? option) : base(option?.pps ?? 180, option?.gravity ?? 500, option?.friction ?? 0.0001) {
            if(option != null) {
                this.playBackSpeed = CheckPlayBackSpeedValue(option.playBackSpeed);
                this.trackingInterval = CheckTrackingIntervalValue(option.trackingInterval);
                this.trackingLimit = CheckTrackingLimitValue(option.trackingLimit);
                this.movementLimit = CheckMovementLimitValue(option.movementLimit);
            }

            this.loopTimer = new Timer(this.Loop!, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void SetPlayBackSpeed(float value) {
            this.playBackSpeed = CheckPlayBackSpeedValue(value);

            if(this.isStarted) {
                this.loopTimer.Change(0, (int)((1000 / this.pps) / this.playBackSpeed));
            }
        }

        public void SetTrackingInterval(float value) {
            this.trackingInterval = CheckTrackingIntervalValue(value);
        }

        public void SetTrackingLimit(int value) {
            this.trackingLimit = CheckTrackingLimitValue(value);
        }

        public void SetMovementLimit(int value) {
            this.movementLimit = CheckMovementLimitValue(value);
        }

        public void Clear(bool force = false) {
            this.content.objects.Clear();

            if(force) {
                this.content.grounds.Clear();
                this.ClearTrack();
            }
        }

        public void ClearTrack(){
            this.tracks.Clear();
        }

        public void Start() {
            if(this.isStarted) throw new Exception("既にシステムは開始されています");

            this.content.Sync();

            this.isStarted = true;
            this.loopTimer.Change(0, (int)((1000 / this.pps) / this.playBackSpeed));
        }

        public void Stop() {
            if(!this.isStarted) throw new Exception("既にシステムは停止しています");

            this.isStarted = false;
            this.loopTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void Loop(Object state) {
            this.Step();
        }

        public void Step(){
            this.trackingCount++;

            if(this.trackingCount >= this.trackingInterval / (1000 / this.pps)) {
                this.content.objects
                  .Where(obj => !obj.isStop)
                  .ToList()
                  .ForEach(obj => {
                      this.tracks.Add(obj.Clone());
                  });

                while(this.tracks.Count > this.trackingLimit) {
                    this.tracks.RemoveAt(0);
                }

                this.trackingCount = 0;
            }

            this.Update();
        }

        private void Update(){
            this.content.Sync();

            this.content.entities.ForEach(entity =>{
                this.UpdatePosition(entity);
                this.UpdateRotate(entity);
            });

            this.content.entities.ForEach(entity =>{
                int index = this.content.entities.IndexOf(entity);

                this.content.grounds.ForEach(ground =>{
                    this.SolveGroundPosition(entity, ground);
                });

                this.content.entities.Skip(index + 1).ToList().ForEach(target=>{
                    this.SolvePosition(entity, target);
                });

                entity.connection.targets.ForEach(target =>{
                    this.SolveConnection(entity,target);
                });
            });

            this.content.entities.ForEach(entity => {
                this.UpdateSpeed(entity);
                this.SolveSpeed(entity);
            });

            //this.content.objects.ForEach(obj=>{
            //    if(
            //        Math.Abs(obj.position.X) > this.movementLimit||
            //        Math.Abs(obj.position.Y) > this.movementLimit
            //    ){
            //        this.DeSpawnObject(obj.id);
            //    }
            //});
        }

        public IObject? SpawnObject<T>(T option){
            IObject? obj = null;

            if(option is CircleOption circleOption){
                obj =  new Circle(circleOption);
            }

            if(obj == null) throw new Exception("無効な物体が指定されています");

            this.content.AddObject(obj);

            if(!this.isStarted) this.content.Sync();

            return obj;
        }

        public IGround? SpawnGround<T>(T option){
            IGround? ground = null;

            if(option is LineOption lineOption){
                ground = new Line(lineOption);
            }else if(option is CurveOption curveOption){
                ground = new Curve(curveOption);
            }

            if(ground == null) throw new Exception("無効な物体が指定されています");

            this.content.AddGround(ground);

            if(!this.isStarted) this.content.Sync();

            return ground;
        }

        public void DeSpawnObject(string id){
            IObject? obj = this.GetObject(id);
            if(obj == null) return;

            this.content.RemoveObject(obj);
        }

        public void DeSpawnGround(string id){
            IGround? ground = this.GetGround(id);
            if(ground == null) return;

            this.content.RemoveGround(ground);
        }

        public IObject? GetObject(string id){
            return this.content.objects.Find(obj => obj.id == id);
        }

        public IGround? GetGround(string id){
            return this.content.grounds.Find(obj => obj.id == id);
        }

        public Entity? GetEntity(string id){
            return this.content.entities.Find(obj => obj.id == id);
        }

        public void Import(string rawSaveData) {
            SaveData? saveData = JsonSerializer.Deserialize<SaveData>(rawSaveData);
            if(saveData == null) throw new Exception("破損したセーブデータです");

            if(saveData.version != Engine.SAVE_DATA_VERSION) throw new Exception($"システムのバージョンは{Engine.SAVE_DATA_VERSION}ですが、{saveData.version}が読み込まれました");

            saveData.objects.circles.ForEach(obj=>{
                this.SpawnObject(obj);
            });

            saveData.objects.lines.ForEach(obj=>{
                this.SpawnGround(obj);
            });

            saveData.objects.curves.ForEach(obj=>{
                this.SpawnGround(obj);
            });

            this.pps = saveData.engine.pps;
            this.gravity = saveData.engine.gravity;
            this.friction = saveData.engine.friction;
            this.playBackSpeed = saveData.engine.playBackSpeed;
            this.trackingInterval = saveData.engine.trackingInterval;
            this.trackingLimit = saveData.engine.trackingLimit;
            this.movementLimit = saveData.engine.movementLimit;
        }

        public string Export(){
            EngineOption engineOption = new EngineOption {
                pps = this.pps,
                gravity = this.gravity,
                friction = this.friction,
                playBackSpeed = this.playBackSpeed,
                trackingInterval = this.trackingInterval,
                trackingLimit = this.trackingLimit,
                movementLimit = this.movementLimit
            };

            SaveData saveData = new SaveData {
                saveAt = DateTime.Now,
                engine = engineOption,
                objects = this.content.ToData()
            };

            return JsonSerializer.Serialize(saveData);
        }

        public List<IObject> GetObjectsAt(double posX, double posY) {
            Vector2 position = new Vector2(posX, posY);
            List<IObject> targets = [];

            this.content.objects.ForEach(obj => {
                List<Entity> entities = [..obj.entities.Where(entity =>{
                    Vector2 difference = entity.position - position;

                    double distance = difference.Length();

                    return distance <= entity.radius;
                })];

                if(entities.Count == 0) return;

                targets.Add(obj);
            });

            return targets;
        }

        public List<IGround> GetGroundsAt(double posX, double posY) {
            Vector2 position = new Vector2(posX, posY);
            List<IGround> targets = [];

            this.content.grounds.ForEach(ground => {
                Vector2 crossPosition = ground.SolvePosition(position);

                Vector2 difference = crossPosition - position;

                double distance = difference.Length();

                if(distance > ground.width / 2) return;

                targets.Add(ground);
            });

            return targets;
        }

        public List<Entity> GetEntitiesAt(double posX, double posY) {
            Vector2 position = new Vector2(posX, posY);
            List<Entity> targets = [];

            this.content.entities.ForEach(entity => {
                Vector2 difference = entity.position - position;

                double distance = difference.Length();

                if(distance > entity.radius) return;

                targets.Add(entity);
            });

            return targets;
        }

        private static float CheckPlayBackSpeedValue(float playBackSpeed) {
            if(playBackSpeed < 0) throw new Exception("再生速度(playBackSpeed)は0以上に設定する必要があります");

            return playBackSpeed;
        }

        private static float CheckTrackingIntervalValue(float trackingInterval) {
            if(trackingInterval < 0) throw new Exception("トラッキング間隔(trackingInterval)は0以上に設定する必要があります");

            return trackingInterval;
        }

        private static int CheckTrackingLimitValue(int trackingLimit) {
            if(trackingLimit < 0) throw new Exception("トラッキング数(trackingLimit)は0以上に設定する必要があります");

            return trackingLimit;
        }

        private static int CheckMovementLimitValue(int movementLimit) {
            if(movementLimit < 0) throw new Exception("マップの移動制限(movementLimit)は0以上に設定する必要があります");

            return movementLimit;
        }
    }
}
