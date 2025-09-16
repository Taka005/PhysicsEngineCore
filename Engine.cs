using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using PhysicsEngineCore.Exceptions;
using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Objects.Interfaces;
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
        /// デフォルトのIDの長さ
        /// </summary>
        public readonly static int DEFAULT_ID_LENGTH = 12;

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
        /// 演算更新時に実行されるスクリプト
        /// </summary>
        public string updateScript = "";

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
        /// アップデートスクリプトの実行回数
        /// </summary>
        private double scriptExecuteCount = 0;

        /// <summary>
        /// アップデートスクリプトの実行間隔
        /// </summary>
        private float _scriptExecuteInterval = 1000;

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
        public readonly Render render;

        /// <summary>
        /// コマンドランナー
        /// </summary>
        public readonly CommandRunner command;

        /// <summary>
        /// コンテンツマネージャー
        /// </summary>
        private readonly ContentManager content = new ContentManager();

        /// <summary>
        /// アセットマネージャー
        /// </summary>
        public readonly AssetsManager assets = new AssetsManager();

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="option">エンジンの初期化クラス</param>
        public Engine(EngineOption? engineOption,RenderOption? renderOption) : base(engineOption?.pps ?? 180, engineOption?.gravity ?? 500, engineOption?.friction ?? 0) {
            if(engineOption != null) {
                this.SetPlayBackSpeed(engineOption.playBackSpeed);
                this.SetTrackingInterval(engineOption.trackingInterval);
                this.SetScriptExecuteInterval(engineOption.scriptExecutionInterval);
                this.SetTrackingLimit(engineOption.trackingLimit);
                this.SetMovementLimit(engineOption.movementLimit);
            }

            this.render = new Render(renderOption);
            this.command = new CommandRunner(this);

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
        /// スクリプトの実行間隔
        /// </summary>
        public float scriptExecuteInterval {
            get {
                return this._scriptExecuteInterval;
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
            if(value <= 0) throw new ArgumentOutOfRangeException(nameof(value), "ppsは1以上の値に設定する必要があります");

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
            if(value < 0) throw new ArgumentOutOfRangeException(nameof(value), "重力加速度(gravity)は0以上に設定する必要があります");

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
        /// アップデートスクリプトの実行間隔を設定します
        /// </summary>
        /// <param name="value"></param>
        public void SetScriptExecuteInterval(float value) {
            this._scriptExecuteInterval = CheckScriptExecuteIntervalValue(value);
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
                this.ClearTrack();
            }

            this.content.Sync();
        }

        /// <summary>
        /// トラッキングを全て削除します
        /// </summary>
        public void ClearTrack() {
            lock(this.tracks) {
                this.trackingCount = 0;
                this.tracks.Clear();
            }
        }

        /// <summary>
        /// シュミレーションを開始します
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Start() {
            if(this.isStarted) throw new EngineSystemException("既にシステムは開始しています");

            this.content.Sync();

            this.isStarted = true;
            this.loopTimer.Change(0, (int)((1000 / this.pps) / this.playBackSpeed));
        }

        /// <summary>
        /// シュミレーションを停止します
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Stop() {
            if(!this.isStarted) throw new EngineSystemException("既にシステムは停止しています");

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

            this.scriptExecuteCount += 1000 / (double)this.pps;

            if(this.scriptExecuteCount >= this.scriptExecuteInterval) {
                try {
                    this.command.ExecuteMultiLine(this.updateScript);
                } catch(Exception ex) {
                    Debug.WriteLine($"スクリプトの実行中にエラーが発生しました: {ex.Message}");
                }

                this.scriptExecuteCount %= this.scriptExecuteInterval;
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
            this.render.Update(this.content.objectCount,this.content.groundCount);
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
                    effect.SetEffect(entity,this.deltaTime);
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
            if(option.id == null) option.id = IdGenerator.CreateId(DEFAULT_ID_LENGTH);

            if(this.GetObject(option.id) != null) throw new DuplicateIdException(option.id);

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

            if(obj == null) throw new InvalidObjectException();

            if(option.imageName != null) {
                Image? image = this.assets.Get(option.imageName);

                if(image != null) {
                    obj.image = image;
                }
            }

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
            if(option.id == null) option.id = IdGenerator.CreateId(DEFAULT_ID_LENGTH);

            if(this.GetGround(option.id) != null) throw new DuplicateIdException(option.id);

            IGround? ground = null;

            if(option is LineOption lineOption) {
                ground = new Line(lineOption);
            } else if(option is CurveOption curveOption) {
                ground = new Curve(curveOption);
            }

            if(ground == null) throw new InvalidObjectException();

            if(option.imageName != null) {
                Image? image = this.assets.Get(option.imageName);

                if(image != null) {
                    ground.image = image;
                }
            }

            this.content.AddGround(ground);

            this.content.Sync();

            return ground;
        }

        /// <summary>
        /// エフェクトを生成します
        /// </summary>
        /// <param name="option">エフェクトの初期化オプション</param>
        /// <returns>生成したエフェクト</returns>
        /// <exception cref="Exception">存在しないエフェクトのとき例外</exception>
        public IEffect? SpawnEffect(IOption option) {
            if(option.id == null) option.id = IdGenerator.CreateId(DEFAULT_ID_LENGTH);

            if(this.GetEffect(option.id) != null) throw new DuplicateIdException(option.id);

            IEffect? effect = null;

            if(option is BoosterOption boosterOption) {
                effect = new Booster(boosterOption);
            }

            if(effect == null) throw new InvalidObjectException();

            if(option.imageName != null) {
                Image? image = this.assets.Get(option.imageName);

                if(image != null) {
                    effect.image = image;
                }
            }

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

        /// <summary>
        /// エフェクトを削除します
        /// </summary>
        /// <param name="id">削除するエフェクトのID</param>
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

        /// <summary>
        /// 指定したIDのエフェクトを取得します
        /// </summary>
        /// <param name="id">取得するエフェクトのID</param>
        /// <returns>取得したエフェクト</returns>
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
        /// 指定したX座標に最も近いグリッドの交差点の位置を取得します
        /// </summary>
        /// <param name="posX">指定するX座標</param>
        /// <returns>計算されたX座標</returns>
        public double GetNearGridCrossPositionX(double posX) {
             double mapX = posX / this.render.scale - this.render.offsetX / this.render.scale;

            double nearestGridMapX = Math.Round(mapX / this.render.gridInterval) * this.render.gridInterval;

            double nearestGridX = (nearestGridMapX + this.render.offsetX / this.render.scale) * this.render.scale;

            return nearestGridX;
        }

        /// <summary>
        /// 指定したY座標に最も近いグリッドの交差点の位置を取得します
        /// </summary>
        /// <param name="posY">指定するY座標</param>
        /// <returns>計算されたY座標</returns>
        public double GetNearGridCrossPositionY(double posY) {
            double mapY = posY / this.render.scale - this.render.offsetY / this.render.scale;

            double nearestGridMapY = Math.Round(mapY / this.render.gridInterval) * this.render.gridInterval;

            double nearestGridY = (nearestGridMapY + this.render.offsetY / this.render.scale) * this.render.scale;

            return nearestGridY;
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
                throw new ImportException("セーブデータの形式が不正です", ex);
            }

            if(saveData == null) throw new ImportException("破損したセーブデータが読み込まれました");

            if(saveData.version != SAVE_DATA_VERSION) throw new ImportException($"システムのバージョンは{SAVE_DATA_VERSION}ですが、{saveData.version}が読み込まれました");

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
            this.SetScriptExecuteInterval(saveData.engine.scriptExecutionInterval);
            this.SetTrackingLimit(saveData.engine.trackingLimit);

            this.render.scale = saveData.render.scale;
            this.render.offsetX = saveData.render.offsetX;
            this.render.offsetY = saveData.render.offsetY;

            this.content.Sync();
        }

        /// <summary>
        /// マップデータをインポートします
        /// </summary>
        /// <param name="fileStream">マップの圧縮ストリーム</param>
        /// <exception cref="Exception">破損時にエラー</exception>
        public void ImportMap(FileStream fileStream) {
            using(ZipArchive archive = new ZipArchive(fileStream, ZipArchiveMode.Read)) {

                ZipArchiveEntry? mapEntry = archive.GetEntry("map.json");

                if(mapEntry == null) throw new ImportException("マップファイルデータが破損しています");

                this.assets.Clear();

                foreach(ZipArchiveEntry entry in archive.Entries) {
                    bool isInAssetsFolder = entry.FullName.StartsWith("assets/", StringComparison.OrdinalIgnoreCase) || entry.FullName.StartsWith("assets\\", StringComparison.OrdinalIgnoreCase);

                    if(entry.FullName.EndsWith("/") || entry.FullName.EndsWith("\\")) continue;

                    bool isImageFile = AssetsManager.imageExtensions.Any(ext => entry.FullName.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
                    if(isInAssetsFolder && isImageFile) {
                        using(Stream entryStream = entry.Open()) {
                            using(MemoryStream memoryStream = new MemoryStream()) {
                                entryStream.CopyTo(memoryStream);
                                memoryStream.Position = 0;

                                this.assets.Add(Path.GetFileName(entry.FullName), memoryStream);
                            }
                        }
                    }
                }

                ZipArchiveEntry? updateScriptEntry = archive.GetEntry("updateScript.txt");

                if(updateScriptEntry != null) {
                    using(Stream updateScriptStream = updateScriptEntry.Open()) {
                        using(StreamReader reader = new StreamReader(updateScriptStream)) {
                            string rawSaveData = reader.ReadToEnd();

                            this.updateScript = rawSaveData;
                        }
                    }
                }

                using(Stream mapStream = mapEntry.Open()) {
                    using(StreamReader reader = new StreamReader(mapStream)) {
                        string rawSaveData = reader.ReadToEnd();

                        this.Import(rawSaveData);
                    }
                }
            }
        }

        /// <summary>
        /// セーブデータクラスに変換します
        /// </summary>
        /// <returns>変換されたセーブデータクラス</returns>
        public SaveData ToSaveData() {
            EngineOption engineOption = new EngineOption {
                pps = this.pps,
                gravity = this.gravity,
                friction = this.friction,
                playBackSpeed = this.playBackSpeed,
                trackingInterval = this.trackingInterval,
                scriptExecutionInterval = this.scriptExecuteInterval,
                trackingLimit = this.trackingLimit,
                movementLimit = this.movementLimit
            };

            RenderOption renderOption = new RenderOption {
                scale = this.render.scale,
                offsetX = this.render.offsetX,
                offsetY = this.render.offsetY
            };

            SaveData saveData = new SaveData {
                saveAt = DateTime.Now,
                engine = engineOption,
                render = renderOption,
                objects = this.content.ToData()
            };

            return saveData;
        }

        /// <summary>
        /// JSON形式のセーブデータに変換します
        /// </summary>
        /// <returns>変換されたJSON形式のセーブデータ</returns>
        public string Export() {
            return JsonSerializer.Serialize(this.ToSaveData());
        }

        /// <summary>
        /// マップデータをエクスポートします
        /// </summary>
        /// <returns>圧縮されたマップストリーム</returns>
        public Stream ExportMap() {
            MemoryStream memoryStream = new MemoryStream();

            using(ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true)) {
                ZipArchiveEntry mapEntry = archive.CreateEntry("map.json");

                using(StreamWriter writer = new StreamWriter(mapEntry.Open())) {
                    writer.Write(this.Export());
                }

                if(!string.IsNullOrEmpty(this.updateScript)) {
                    ZipArchiveEntry updateScriptEntry = archive.CreateEntry("updateScript.txt");

                    using(StreamWriter writer = new StreamWriter(updateScriptEntry.Open())) {
                        writer.Write(this.updateScript);
                    }
                }

                foreach(Image image in this.assets.images) {
                    ZipArchiveEntry imageEntry = archive.CreateEntry($"assets/{image.filename}");

                    using(Stream imageStream = image.source.StreamSource) {
                        imageStream.CopyTo(imageEntry.Open());
                    }
                }
            }

            memoryStream.Position = 0;

            return memoryStream;
        }

        /// <summary>
        /// 指定した位置にあるオブジェクトを取得します
        /// </summary>
        /// <param name="posX">対象のX座標</param>
        /// <param name="posY">対象のY座標</param>
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
        /// 指定した範囲にあるオブジェクトを取得します
        /// </summary>
        /// <param name="start">対象の始点ベクトル</param>
        /// <param name="end">対象の終点ベクトル</param>
        /// <returns>存在したオブジェクトのリスト</returns>
        public List<IObject> GetObjectsInRect(Vector2 start, Vector2 end){
            List<IObject> targets = [];

            this.content.objects.ForEach(obj => {
                List<Entity> entities = [..obj.entities.Where(entity =>{
                     Vector2 difference = entity.position - (start + end) / 2;

                    return (
                        Math.Abs(difference.X) < Math.Abs(start.X - end.X) / 2 &&
                        Math.Abs(difference.Y) < Math.Abs(start.Y - end.Y) / 2
                    );
                })];

                if (entities.Count == 0) return;

                targets.Add(obj);
            });

            return targets;
        }

        /// <summary>
        /// 指定した位置にあるグランドを取得します
        /// </summary>
        /// <param name="posX">対象のX座標</param>
        /// <param name="posY">対象のY座標</param>
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
        /// 指定した座標に端があるグラウンドを取得します
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <returns></returns>
        public List<(IGround ground,GroundEdgeType type)> GetGroundsEdgeAt(double posX, double posY) {
            Vector2 position = new Vector2(posX, posY);
            List<(IGround ground,GroundEdgeType type)> targets = [];

            this.content.grounds.ForEach(ground=>{
                if(ground is Line line) {
                    Vector2 differenceStart = line.start - position;
                    Vector2 differenceEnd = line.end - position;

                    double distanceStart = differenceStart.Length();
                    double distanceEnd = differenceEnd.Length();

                    if(distanceStart <= line.width / 2){
                        targets.Add((ground, GroundEdgeType.Start));
                    }else if(distanceEnd <= line.width / 2){
                        targets.Add((ground, GroundEdgeType.End));
                    }                    
                }else if(ground is Curve curve){
                    Vector2 differenceStart = curve.start - position;
                    Vector2 differenceMiddle = curve.middle - position;
                    Vector2 differenceEnd = curve.end - position;

                    double distanceStart = differenceStart.Length();
                    double distanceMiddle = differenceMiddle.Length();
                    double distanceEnd = differenceEnd.Length();

                    if(distanceStart <= curve.width / 2){
                        targets.Add((ground, GroundEdgeType.Start));
                    }else if(distanceMiddle <= curve.width / 2){
                        targets.Add((ground, GroundEdgeType.Middle));
                    }else if(distanceEnd <= curve.width / 2){
                        targets.Add((ground, GroundEdgeType.End));
                    }
                }
            });

            return targets;
        }

        /// <summary>
        /// 指定した位置にあるエフェクトを取得します
        /// </summary>
        /// <param name="posX">対象のX座標</param>
        /// <param name="posY">対象のY座標</param>
        /// <returns>存在したエフェクトのリスト</returns>
        public List<IEffect> GetEffectsAt(double posX, double posY) {
            Vector2 position = new Vector2(posX, posY);

            List<IEffect> targets = [];

            this.content.effects.ForEach(effect => {
                if(effect is Booster booster) {
                    Vector2 difference = position - (booster.start + booster.end) / 2;

                    if(
                        Math.Abs(difference.X) >= Math.Abs(booster.start.X - booster.end.X)/2||
                        Math.Abs(difference.Y) >= Math.Abs(booster.start.Y - booster.end.Y)/2
                    ) return;

                    targets.Add(booster);
                }
            });

            return targets;
        }

        /// <summary>
        /// 指定した位置にあるエンティティーを取得します
        /// </summary>
        /// <param name="posX">対象のX座標</param>
        /// <param name="posY">対象のY座標</param>
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
        /// 指定した範囲にあるエンティティーを取得します
        /// </summary>
        /// <param name="start">対象の始点ベクトル</param>
        /// <param name="end">対象の終点ベクトル</param>
        /// <returns>存在したエンティティーのリスト</returns>
        public List<Entity> GetEntitiesInRect(Vector2 start, Vector2 end){
            List<Entity> targets = [];

            foreach (var entity in this.content.entities){
                Vector2 difference = entity.position - (start + end) / 2;

                if(
                    Math.Abs(difference.X) >= Math.Abs(start.X - end.X) / 2 ||
                    Math.Abs(difference.Y) >= Math.Abs(start.Y - end.Y) / 2
                ) continue;

                targets.Add(entity);
            }

            return targets;
        }

        /// <summary>
        /// 再生速度が正しい値かチェックします
        /// </summary>
        /// <param name="playBackSpeed">再生速度</param>
        /// <returns>正しい再生速度</returns>
        /// <exception cref="Exception">0未満であったときに例外</exception>
        private static float CheckPlayBackSpeedValue(float playBackSpeed) {
            if(playBackSpeed < 0) throw new ArgumentOutOfRangeException(nameof(playBackSpeed), "再生速度(playBackSpeed)は0以上に設定する必要があります");

            return playBackSpeed;
        }

        /// <summary>
        /// トラッキング間隔が正しい値かチェックします
        /// </summary>
        /// <param name="trackingInterval">間隔</param>
        /// <returns>正しい間隔</returns>
        /// <exception cref="Exception">0未満であったときに例外</exception>
        private static float CheckTrackingIntervalValue(float trackingInterval) {
            if(trackingInterval < 0) throw new ArgumentOutOfRangeException(nameof(trackingInterval), "トラッキング間隔(trackingInterval)は0以上に設定する必要があります");

            return trackingInterval;
        }

        /// <summary>
        /// アップデートスクリプトの実行間隔が正しい値かチェックします
        /// </summary>
        /// <param name="trackingInterval">間隔</param>
        /// <returns>正しい間隔</returns>
        /// <exception cref="Exception">0未満であったときに例外</exception>
        private static float CheckScriptExecuteIntervalValue(float scriptExecuteInterval) {
            if(scriptExecuteInterval < 0) throw new ArgumentOutOfRangeException(nameof(scriptExecuteInterval), "スクリプトの実行間隔(scriptExecuteInterval)は0以上に設定する必要があります");

            return scriptExecuteInterval;
        }

        /// <summary>
        /// トラッキング数が正しい値かチェックします
        /// </summary>
        /// <param name="trackingLimit">回数</param>
        /// <returns>正しい回数</returns>
        /// <exception cref="Exception">0未満であったときに例外</exception>
        private static int CheckTrackingLimitValue(int trackingLimit) {
            if(trackingLimit < 0) throw new ArgumentOutOfRangeException(nameof(trackingLimit), "トラッキング数(trackingLimit)は0以上に設定する必要があります");

            return trackingLimit;
        }

        /// <summary>
        /// 移動制限が正しい値かチェックします
        /// </summary>
        /// <param name="mass">距離</param>
        /// <returns>正しい距離</returns>
        /// <exception cref="Exception">0未満であったときに例外</exception>
        private static int CheckMovementLimitValue(int movementLimit) {
            if(movementLimit < 0) throw new ArgumentOutOfRangeException(nameof(movementLimit), "マップの移動制限(movementLimit)は0以上に設定する必要があります");

            return movementLimit;
        }
    }
}
