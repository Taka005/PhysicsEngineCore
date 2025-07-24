using System.Text.Json;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Options;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore {
    /// <summary>
    /// 物理エンジンのメインクラス
    /// </summary>
    public class Engine : Core {
        /// <summary>
        /// セーブデータのバージョン
        /// </summary>
        public readonly static string SAVE_DATA_VERSION = "1";

        /// <summary>
        /// スタートしているかどうか
        /// </summary>
        public bool isStarted = false;

        /// <summary>
        /// トラッキングモードが有効かどうか
        /// </summary>
        private bool _isTrackingMode = false;

        /// <summary>
        /// 動的トラッキングモードが有効かどうか
        /// </summary>
        public bool isDynamicTrackingMode = false;

        /// <summary>
        /// ループ用タイマー
        /// </summary>
        private readonly Timer loopTimer;

        /// <summary>
        /// 再生速度
        /// </summary>
        private float _playBackSpeed = 1;

        /// <summary>
        /// トラッキング間隔
        /// </summary>
        private float _trackingInterval = 100;

        /// <summary>
        /// トラッキング回数
        /// </summary>
        private double trackingCount = 0;

        /// <summary>
        /// トラッキングの制限数
        /// </summary>
        private int _trackingLimit = 100;

        /// <summary>
        /// 移動範囲
        /// </summary>
        private int _movementLimit = 10000;

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
        public Engine(EngineOption? option) : base(option?.pps ?? 180, option?.gravity ?? 500, option?.friction ?? 0) {
            if(option != null) {
                this._playBackSpeed = CheckPlayBackSpeedValue(option.playBackSpeed);
                this._trackingInterval = CheckTrackingIntervalValue(option.trackingInterval);
                this._trackingLimit = CheckTrackingLimitValue(option.trackingLimit);
                this._movementLimit = CheckMovementLimitValue(option.movementLimit);
            }

            this.loopTimer = new Timer(this.Loop!, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// 再生速度
        /// </summary>
        public float playBackSpeed {
            get {
                return this._playBackSpeed;
            }
        }

        /// <summary>
        /// トラッキング間隔
        /// </summary>
        public float trackingInterval {
            get {
                return this._trackingInterval;
            }
        }

        /// <summary>
        /// トラッキングの制限
        /// </summary>
        public int trackingLimit {
            get {
                return this._trackingLimit;
            }
        }

        /// <summary>
        /// マップの移動制限
        /// </summary>
        public int movementLimit {
            get {
                return this._movementLimit;
            }
        }

        /// <summary>
        /// トラッキングモードが有効かどうか
        /// </summary>
        public bool isTrackingMode {
            get {
                return this._isTrackingMode;
            }
            set {
                this._isTrackingMode = value;

                if(!value) {
                    this.render.ClearTracking();
                }
            }
        }

        /// <summary>
        /// ppsを設定します
        /// </summary>
        /// <param name="value">設定する値</param>
        /// <exception cref="Exception">1未満の場合にエラー</exception>
        public void SetPps(int value) {
            if(value <= 0) throw new Exception("ppsは1以上の値に設定する必要があります");

            this.pps = value;

            if(this.isStarted) {
                this.loopTimer.Change(0, (int)((1000 / this.pps) / this.playBackSpeed));
            }
        }

        /// <summary>
        /// 重力加速度を設定します
        /// </summary>
        /// <param name="value">設定する値</param>
        /// <exception cref="Exception">0未満の場合にエラー</exception>
        public void SetGravity(double value) {
            if(value < 0) throw new Exception("重力加速度(gravity)は0以上に設定する必要があります");

            this.gravity = value;
        }

        /// <summary>
        /// 再生速度を設定します
        /// </summary>
        /// <param name="value">設定する再生速度</param>
        public void SetPlayBackSpeed(float value) {
            this._playBackSpeed = CheckPlayBackSpeedValue(value);

            if(this.isStarted) {
                this.loopTimer.Change(0, (int)((1000 / this.pps) / this.playBackSpeed));
            }
        }

        /// <summary>
        /// トラッキング間隔を設定します
        /// </summary>
        /// <param name="value">設定するトラッキング間隔</param>
        public void SetTrackingInterval(float value) {
            this._trackingInterval = CheckTrackingIntervalValue(value);
        }

        /// <summary>
        /// トラッキング制限を設定します
        /// </summary>
        /// <param name="value">設定する回数</param>
        public void SetTrackingLimit(int value) {
            this._trackingLimit = CheckTrackingLimitValue(value);
        }

        /// <summary>
        /// 移動制限を設定します
        /// </summary>
        /// <param name="value">設定する距離</param>
        public void SetMovementLimit(int value) {
            this._movementLimit = CheckMovementLimitValue(value);
        }

        /// <summary>
        /// オブジェクトを全て削除します
        /// </summary>
        /// <param name="force">真の場合に地面と履歴も削除</param>
        public void Clear(bool force = false) {
            this.content.RemoveAllObjects();

            if(force) {
                this.content.RemoveAllGrounds();
                this.content.RemoveAllEffects();
                this.ClearTrack(true);
            }

            this.content.Sync();
        }

        /// <summary>
        /// トラッキングを全て削除します
        /// </summary>
        public void ClearTrack(bool isIncludeDraw = false) {
            lock(this.tracks) {
                this.trackingCount = 0;
                this.tracks.Clear();

                if(isIncludeDraw) {
                    this.render.ClearTracking();
                }
            }
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

             this.content.Sync();
        }

        /// <summary>
        /// ループさせます
        /// </summary>
        private void Loop(Object _) {
            this.Step();
        }

        /// <summary>
        /// シュミレーションを1フレーム進めます
        /// </summary>
        public void Step() {
            this.content.Sync();

            int trackingLimit = this.isDynamicTrackingMode ? this.trackingLimit*this.content.objectCount : this.trackingLimit;

            this.trackingCount += 1000 / (double)this.pps;

            lock(this.tracks) {
                if(this.trackingCount >= this.trackingInterval) {
                    foreach(IObject obj in this.content.objects.Where(obj => !obj.isStop)) {
                        this.tracks.Add(obj.Clone());
                    }

                    while(this.tracks.Count > trackingLimit) {
                        this.tracks.RemoveAt(0);
                    }

                    this.trackingCount %= this.trackingInterval;
                }
            }

            this.Update();
        }

        /// <summary>
        /// レンダリングを行いいます
        /// このメソッドはUIスレッドで実行する必要があります
        /// </summary>
        /// <param name="sender">データ</param>
        /// <param name="e">イベント</param>
        public void OnRendering(object? sender, EventArgs e) {
            this.render.Update();
            this.render.DrawObject(this.content.objects);
            this.render.DrawGround(this.content.grounds);
            this.render.DrawEffect(this.content.effects);

            if(this.isTrackingMode) {
                lock(this.tracks) {
                    this.render.DrawTracking(this.tracks);
                }
            }
        }

        /// <summary>
        /// オブジェクトを更新します
        /// </summary>
        private void Update() {
            List<Entity> entities = this.content.entities;

            foreach(Entity entity in entities) {
                foreach(IEffect effect in this.content.effects) {
                    effect.SetEffect(entity);
                }

                this.UpdatePosition(entity);
            }

            foreach(Entity entity in entities) {
                int index = entities.IndexOf(entity);

                foreach(IGround ground in this.content.grounds) {
                    this.SolveGroundPosition(entity, ground);
                }

                foreach(Entity target in entities.Skip(index + 1)) {
                    this.SolvePosition(entity, target);
                }

                foreach(Target target in entity.connection.targets) {
                    Entity? targetEntity = this.GetEntity(target.entityId);

                    if(targetEntity == null) {
                        entity.connection.Remove(target.entityId);
                    } else {
                        this.SolveConnection(entity, targetEntity, target.distance, target.stiffness);
                    }
                }
            }

            foreach(Entity entity in entities) {
                this.UpdateSpeed(entity);
                this.SolveSpeed(entity);
            }

            foreach(IObject obj in this.content.objects) {
                if(
                    Math.Abs(obj.position.X) > this.movementLimit ||
                    Math.Abs(obj.position.Y) > this.movementLimit
                ) {
                    this.DeSpawnObject(obj.id);
                }
            }
        }

        /// <summary>
        /// オブジェクトを生成します
        /// </summary>
        /// <param name="option">オブジェクトの初期化オプション</param>
        /// <returns>生成したオブジェクト</returns>
        /// <exception cref="Exception">存在しないオブジェクトのとき例外</exception>
        public IObject? SpawnObject(IOption option) {
            if(option.id == null) option.id = IdGenerator.CreateId(15);

            IObject? obj = null;

            if(option is CircleOption circleOption) {
                obj = new Circle(circleOption);
            } else if(option is RopeOption ropeOption) {
                obj = new Rope(ropeOption);
            } else if(option is SquareOption squareOption) {
                obj = new Square(squareOption);
            } else if(option is TriangleOption triangleOption) {
                obj = new Triangle(triangleOption);
            }

            if(obj == null) throw new Exception("無効な物体が指定されています");

            this.content.AddObject(obj);

            this.content.Sync();

            return obj;
        }

        /// <summary>
        /// グラウンドを生成します
        /// </summary>
        /// <param name="option">グラウンドの初期化オプション</param>
        /// <returns>生成したグラウンド</returns>
        /// <exception cref="Exception">存在しないグラウンドのとき例外</exception>
        public IGround? SpawnGround(IOption option) {
            if(option.id == null) option.id = IdGenerator.CreateId(15);

            IGround? ground = null;

            if(option is LineOption lineOption) {
                ground = new Line(lineOption);
            } else if(option is CurveOption curveOption) {
                ground = new Curve(curveOption);
            }

            if(ground == null) throw new Exception("無効な物体が指定されています");

            this.content.AddGround(ground);

            this.content.Sync();

            return ground;
        }

        public IEffect? SpawnEffect(IOption option) {
            if(option.id == null) option.id = IdGenerator.CreateId(15);

            IEffect? effect = null;

            if(option is BoosterOption boosterOption) {
                effect = new Booster(boosterOption);
            }

            if(effect == null) throw new Exception("無効な物体が指定されています");

            this.content.AddEffect(effect);

            this.content.Sync();

            return effect;
        }

        /// <summary>
        /// オブジェクトを削除します
        /// </summary>
        /// <param name="id">削除するオブジェクトのID</param>
        public void DeSpawnObject(string id) {
            IObject? obj = this.GetObject(id);
            if(obj == null) return;

            this.content.RemoveObject(obj);

            this.content.Sync();
        }

        /// <summary>
        /// グラウンドを削除します
        /// </summary>
        /// <param name="id">削除するグラウンドのID</param>
        public void DeSpawnGround(string id) {
            IGround? ground = this.GetGround(id);
            if(ground == null) return;

            this.content.RemoveGround(ground);

            this.content.Sync();
        }

        public void DeSpawnEffect(string id) {
            IEffect? effect = this.GetEffect(id);
            if(effect == null) return;

            this.content.RemoveEffect(effect);

            this.content.Sync();
        }

        /// <summary>
        /// 指定したIDのオブジェクトを取得します
        /// </summary>
        /// <param name="id">取得するオブジェクトのID</param>
        /// <returns>取得したオブジェクト</returns>
        public IObject? GetObject(string id) {
            return this.content.objects.Find(obj => obj.id == id);
        }

        /// <summary>
        /// 指定したIDのグラウンドを取得します
        /// </summary>
        /// <param name="id">取得するグラウンドのID</param>
        /// <returns>取得したグラウンド</returns>
        public IGround? GetGround(string id) {
            return this.content.grounds.Find(obj => obj.id == id);
        }

        public IEffect? GetEffect(string id) {
            return this.content.effects.Find(obj => obj.id == id);
        }

        /// <summary>
        /// 指定したIDのエンティティーを取得します
        /// </summary>
        /// <param name="id">取得するエンティティーのID</param>
        /// <returns>取得したエンティティー</returns>
        public Entity? GetEntity(string id) {
            return this.content.entities.Find(obj => obj.id == id);
        }

        /// <summary>
        /// セーブデータを読み込みます
        /// </summary>
        /// <param name="rawSaveData">JSON形式のセーブデータ</param>
        /// <exception cref="Exception">破損またはバージョンが異なる場合に例外</exception>
        public void Import(string rawSaveData) {
            SaveData? saveData;
            try {
                saveData = JsonSerializer.Deserialize<SaveData>(rawSaveData);
            } catch(JsonException ex) {
                throw new Exception("セーブデータの形式が不正です", ex);
            }

            if(saveData == null) throw new Exception("破損したセーブデータです");

            if(saveData.version != SAVE_DATA_VERSION) throw new Exception($"システムのバージョンは{SAVE_DATA_VERSION}ですが、{saveData.version}が読み込まれました");

            this.Clear(force: true);

            saveData.GetAllObjects().ForEach(obj => {
                this.SpawnObject(obj);
            });

            saveData.GetAllGrounds().ForEach(ground => {
                this.SpawnGround(ground);
            });

            saveData.GetAllEffects().ForEach(effect => {
                this.SpawnEffect(effect);
            });

            this.pps = saveData.engine.pps;
            this.gravity = saveData.engine.gravity;
            this.friction = saveData.engine.friction;

            this.SetPlayBackSpeed(saveData.engine.playBackSpeed);
            this.SetMovementLimit(saveData.engine.movementLimit);
            this.SetTrackingInterval(saveData.engine.trackingInterval);
            this.SetTrackingLimit(saveData.engine.trackingLimit);

            this.content.Sync();
        }

        /// <summary>
        /// セーブデータクラスに変換します
        /// </summary>
        /// <returns>変換されたセーブデータクラス</returns>
        public SaveData toSaveData() {
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
        public string Export() {
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

        public List<IEffect> GetEffectsAt(double posX, double posY) {
            Vector2 position = new Vector2(posX, posY);

            List<IEffect> targets = [];

            this.content.effects.ForEach(effect => {
                if(effect is Booster booster) {
                    Vector2 difference = position - (booster.start + booster.end) / 2;

                    if(difference.Length() >= (booster.start - booster.end).Length()/2) return;

                    targets.Add(booster);
                }
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
