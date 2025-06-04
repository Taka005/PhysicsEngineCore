using System.Text.Json;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Options;
using PhysicsEngineCore.Utils;
using PhysicsEngineRender;

namespace PhysicsEngineCore {
    public class Engine : Core {
        /// <summary>
        /// セーブデータのバージョン
        /// </summary>
        public readonly static string SAVE_DATA_VERSION = "1";

        /// <summary>
        /// スタートしているかどうか
        /// </summary>
        private bool isStarted = false;

        /// <summary>
        /// トラッキングモードが有効かどうか
        /// </summary>
        public bool isTrackingMode = false;

        /// <summary>
        /// デバッグモードが有効かどうか
        /// </summary>
        public bool isDebugMode = false;

        /// <summary>
        /// ループ用タイマー
        /// </summary>
        private readonly Timer loopTimer;

        /// <summary>
        /// 再生速度
        /// </summary>
        private float playBackSpeed = 1;

        /// <summary>
        /// トラッキング間隔
        /// </summary>
        private float trackingInterval = 100;

        /// <summary>
        /// トラッキング回数
        /// </summary>
        private int trackingCount = 0;

        /// <summary>
        /// トラッキングの制限数
        /// </summary>
        private int trackingLimit = 50000;

        /// <summary>
        /// 移動範囲
        /// </summary>
        private int movementLimit = 10000;

        /// <summary>
        /// トラッキングされたオブジェクトのリスト
        /// </summary>
        private readonly List<IObject> tracks = [];

        /// <summary>
        /// レンダー
        /// </summary>
        public readonly Render render = new Render();

        /// <summary>
        /// コンテンツマネージャー
        /// </summary>
        private readonly ContentManager content = new ContentManager();

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="option">エンジンの初期化クラス</param>
        public Engine(EngineOption? option) : base(option?.pps ?? 180, option?.gravity ?? 500, option?.friction ?? 0.0001) {
            if(option != null) {
                this.playBackSpeed = CheckPlayBackSpeedValue(option.playBackSpeed);
                this.trackingInterval = CheckTrackingIntervalValue(option.trackingInterval);
                this.trackingLimit = CheckTrackingLimitValue(option.trackingLimit);
                this.movementLimit = CheckMovementLimitValue(option.movementLimit);
            }

            this.loopTimer = new Timer(this.Loop!, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// 再生速度を設定します
        /// </summary>
        /// <param name="value">設定する再生速度</param>
        public void SetPlayBackSpeed(float value) {
            this.playBackSpeed = CheckPlayBackSpeedValue(value);

            if(this.isStarted) {
                this.loopTimer.Change(0, (int)((1000 / this.pps) / this.playBackSpeed));
            }
        }

        /// <summary>
        /// トラッキング間隔を設定します
        /// </summary>
        /// <param name="value">設定するトラッキング間隔</param>
        public void SetTrackingInterval(float value) {
            this.trackingInterval = CheckTrackingIntervalValue(value);
        }

        /// <summary>
        /// トラッキング制限を設定します
        /// </summary>
        /// <param name="value">設定する回数</param>
        public void SetTrackingLimit(int value) {
            this.trackingLimit = CheckTrackingLimitValue(value);
        }

        /// <summary>
        /// 移動制限を設定します
        /// </summary>
        /// <param name="value">設定する距離</param>
        public void SetMovementLimit(int value) {
            this.movementLimit = CheckMovementLimitValue(value);
        }

        /// <summary>
        /// オブジェクトを全て削除します
        /// </summary>
        /// <param name="force">真の場合に地面と履歴も削除</param>
        public void Clear(bool force = false) {
            this.content.objects.Clear();

            if(force) {
                this.content.grounds.Clear();
                this.ClearTrack();
            }
        }

        /// <summary>
        /// トラッキングを全て削除します
        /// </summary>
        public void ClearTrack(){
            this.tracks.Clear();
        }

        /// <summary>
        /// シュミレーションを開始します
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Start() {
            if(this.isStarted) throw new Exception("既にシステムは開始されています");

            this.content.Sync();

            this.isStarted = true;
            this.loopTimer.Change(0, (int)((1000 / this.pps) / this.playBackSpeed));
        }

        /// <summary>
        /// シュミレーションを停止します
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Stop() {
            if(!this.isStarted) throw new Exception("既にシステムは停止しています");

            this.isStarted = false;
            this.loopTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// ループさせます
        /// </summary>
        /// <param name="state">ステート</param>
        private void Loop(Object state) {
            this.Step();
        }

        /// <summary>
        /// シュミレーションを1フレーム進めます
        /// </summary>
        public void Step(){
            this.content.Sync();

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

        /// <summary>
        /// レンダリングを行いいます
        /// このメソッドはUIスレッドで実行する必要があります
        /// </summary>
        /// <param name="sender">データ</param>
        /// <param name="e">イベント</param>
        public void OnRendering(object? sender, EventArgs e){
            this.render.DrawObject(this.content.objects);
            this.render.DrawGround(this.content.grounds);
        }

        /// <summary>
        /// オブジェクトを更新します
        /// </summary>
        private void Update(){
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
                    Entity? targetEntity = GetEntity(target.entityId);
                    if(targetEntity == null){
                        entity.connection.Remove(target.entityId);
                    }else{
                        this.SolveConnection(entity, targetEntity, target.distance, target.stiffness);
                    }
                });
            });

            this.content.entities.ForEach(entity => {
                this.UpdateSpeed(entity);
                this.SolveSpeed(entity);
            });

            this.content.objects.ForEach(obj => {
                if(
                    Math.Abs(obj.position.X) > this.movementLimit ||
                    Math.Abs(obj.position.Y) > this.movementLimit
                ) {
                    this.DeSpawnObject(obj.id);
                }
            });
        }

        public IObject? SpawnObject<T>(T option){
            IObject? obj = null;

            if(option is CircleOption circleOption){
                obj = new Circle(circleOption);
            }else if(option is RopeOption ropeOption){
                obj = new Rope(ropeOption);
            } else if(option is SquareOption squareOption){
                obj = new Square(squareOption);
            }

            if(obj == null) throw new Exception("無効な物体が指定されています");

            this.content.AddObject(obj);

            if(!this.isStarted) this.content.Sync();

            return obj;
        }

        public IGround? SpawnGround<T>(T option){
            IGround? ground = null;

            if(option is LineOption lineOption) {
                ground = new Line(lineOption);
            } else if(option is CurveOption curveOption) {
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

            if(!this.isStarted) this.content.Sync();
        }

        public void DeSpawnGround(string id){
            IGround? ground = this.GetGround(id);
            if(ground == null) return;

            this.content.RemoveGround(ground);

            if(!this.isStarted) this.content.Sync();
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

        /// <summary>
        /// セーブデータを読み込みます
        /// </summary>
        /// <param name="rawSaveData">JSON形式のセーブデータ</param>
        /// <exception cref="Exception">破損またはバージョンが異なる場合に例外</exception>
        public void Import(string rawSaveData) {
            SaveData? saveData = JsonSerializer.Deserialize<SaveData>(rawSaveData);
            if(saveData == null) throw new Exception("破損したセーブデータです");

            if(saveData.version != SAVE_DATA_VERSION) throw new Exception($"システムのバージョンは{SAVE_DATA_VERSION}ですが、{saveData.version}が読み込まれました");

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

        /// <summary>
        /// セーブデータクラスに変換します
        /// </summary>
        /// <returns>変換されたセーブデータクラス</returns>
        public SaveData toSaveData(){
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

            return saveData;
        }

        /// <summary>
        /// JSON形式のセーブデータに変換します
        /// </summary>
        /// <returns>変換されたJSON形式のセーブデータ</returns>
        public string Export(){
            return JsonSerializer.Serialize(this.toSaveData());
        }

        /// <summary>
        /// 指定した位置にあるオブジェクトを取得します
        /// </summary>
        /// <param name="posX">対象のX座標</param>
        /// <param name="posY">対象のX座標</param>
        /// <returns>存在したオブジェクトのリスト</returns>
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

        /// <summary>
        /// 指定した位置にあるグランドを取得します
        /// </summary>
        /// <param name="posX">対象のX座標</param>
        /// <param name="posY">対象のX座標</param>
        /// <returns>存在したグランドのリスト</returns>
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

        /// <summary>
        /// 指定した位置にあるエンティティーを取得します
        /// </summary>
        /// <param name="posX">対象のX座標</param>
        /// <param name="posY">対象のX座標</param>
        /// <returns>存在したエンティティーのリスト</returns>
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

        /// <summary>
        /// 再生速度が正しい値かチェックします
        /// </summary>
        /// <param name="mass">再生速度</param>
        /// <returns>正しい再生速度</returns>
        /// <exception cref="Exception">0未満であったときに例外</exception>
        private static float CheckPlayBackSpeedValue(float playBackSpeed) {
            if(playBackSpeed < 0) throw new Exception("再生速度(playBackSpeed)は0以上に設定する必要があります");

            return playBackSpeed;
        }

        /// <summary>
        /// トラッキング間隔が正しい値かチェックします
        /// </summary>
        /// <param name="mass">間隔</param>
        /// <returns>正しい間隔</returns>
        /// <exception cref="Exception">0未満であったときに例外</exception>
        private static float CheckTrackingIntervalValue(float trackingInterval) {
            if(trackingInterval < 0) throw new Exception("トラッキング間隔(trackingInterval)は0以上に設定する必要があります");

            return trackingInterval;
        }

        /// <summary>
        /// トラッキング数が正しい値かチェックします
        /// </summary>
        /// <param name="mass">回数</param>
        /// <returns>正しい回数</returns>
        /// <exception cref="Exception">0未満であったときに例外</exception>
        private static int CheckTrackingLimitValue(int trackingLimit) {
            if(trackingLimit < 0) throw new Exception("トラッキング数(trackingLimit)は0以上に設定する必要があります");

            return trackingLimit;
        }

        /// <summary>
        /// 移動制限が正しい値かチェックします
        /// </summary>
        /// <param name="mass">距離</param>
        /// <returns>正しい距離</returns>
        /// <exception cref="Exception">0未満であったときに例外</exception>
        private static int CheckMovementLimitValue(int movementLimit) {
            if(movementLimit < 0) throw new Exception("マップの移動制限(movementLimit)は0以上に設定する必要があります");

            return movementLimit;
        }
    }
}
