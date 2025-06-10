using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Options;

namespace PhysicsEngineCore.Utils {

    /// <summary>
    /// オブジェクトを管理します
    /// </summary>
    class ContentManager() {
        internal readonly List<IObject> objects = [];
        internal readonly List<IGround> grounds = [];
        internal readonly List<IObject> tracks = [];
        private readonly List<QueueObject> queueObjects = [];
        private readonly List<QueueGround> queueGrounds = [];
        private readonly object lockObject = new object();

        /// <summary>
        /// 全てのエンティティー
        /// </summary>
        public List<Entity> entities {
            get {
                lock(this.lockObject) {
                    return [.. this.objects.SelectMany(obj => obj.entities)];
                }
            }
        }

        /// <summary>
        /// 現在のオブジェクトリストのコピーを返します
        /// </summary>
        public List<IObject> getObjectsCopy() {
            lock(this.lockObject) {
                return [.. this.objects];
            }
        }

        /// <summary>
        /// 現在のグラウンドリストのコピーを返します。
        /// </summary>
        public List<IGround> getGroundsCopy() {
            lock(this.lockObject) {
                return [.. this.grounds];
            }
        }

        /// <summary>
        /// オブジェクトの追加を待機列に追加します
        /// </summary>
        /// <param name="target">追加するオブジェクト</param>
        public void AddObject(IObject target) {
            if(target == null) throw new ArgumentNullException(nameof(target), "オブジェクトがNULLです");

            QueueObject queue = new QueueObject {
                command = CommandType.Add,
                target = target
            };

            lock(this.lockObject) {
                this.queueObjects.Add(queue);
            }
        }

        /// <summary>
        /// オブジェクトの削除を待機列に追加します
        /// </summary>
        /// <param name="target">削除するオブジェクト</param>
        public void RemoveObject(IObject target) {
            if(target == null) throw new ArgumentNullException(nameof(target), "オブジェクトがNULLです");

            QueueObject queue = new QueueObject {
                command = CommandType.Remove,
                target = target
            };

            lock(this.lockObject) {
                this.queueObjects.Add(queue);
            }
        }

        /// <summary>
        /// グラウンドの追加を待機列に追加します
        /// </summary>
        /// <param name="target">追加するグラウンド</param>
        public void AddGround(IGround target) {
            if(target == null) throw new ArgumentNullException(nameof(target), "グラウンドがNULLです");

            QueueGround queue = new QueueGround {
                command = CommandType.Add,
                target = target
            };

            lock(this.lockObject) {
                this.queueGrounds.Add(queue);
            }
        }

        /// <summary>
        /// グラウンドの削除を待機列に追加します
        /// </summary>
        /// <param name="target">削除するグラウンド</param>
        public void RemoveGround(IGround target) {
            if(target == null) throw new ArgumentNullException(nameof(target), "グラウンドがNULLです");

            QueueGround queue = new QueueGround {
                command = CommandType.Remove,
                target = target
            };

            lock(this.lockObject) {
                this.queueGrounds.Add(queue);
            }
        }

        /// <summary>
        /// 待機列にあるオブジェクトを処理します
        /// </summary>
        public void Sync() {
            lock(this.lockObject) {
                if(this.queueObjects.Count == 0 && this.queueGrounds.Count == 0) return;

                List<QueueObject> currentQueueObjects = [.. this.queueObjects];
                this.queueObjects.Clear();

                List<QueueGround> currentQueueGrounds = [.. this.queueGrounds];
                this.queueGrounds.Clear();

                foreach(QueueObject obj in currentQueueObjects) {
                    if(obj.target == null) return;

                    if(obj.command == CommandType.Add) {
                        this.objects.Add(obj.target);
                    } else if(obj.command == CommandType.Remove) {
                        this.objects.RemoveAll(target => target.id == obj.id);
                    }
                }

                foreach(QueueGround ground in currentQueueGrounds) {
                    if(ground.target == null) return;

                    if(ground.command == CommandType.Add) {
                        this.grounds.Add(ground.target);
                    } else if(ground.command == CommandType.Remove) {
                        this.grounds.RemoveAll(target => target.id == ground.id);
                    }
                }
            }
        }

        /// <summary>
        /// セーブデータに変換します
        /// </summary>
        /// <returns>変換されたセーブデータ</returns>
        public ObjectSaveData ToData() {
            List<CircleOption> circleOptions = [.. this.objects.OfType<Circle>().Select(obj => obj.ToOption())];
            List<SquareOption> squareOptions = [.. this.objects.OfType<Square>().Select(obj => obj.ToOption())];
            List<TriangleOption> triangleOptions = [.. this.objects.OfType<Triangle>().Select(obj => obj.ToOption())];
            List<RopeOption> ropeOptions = [.. this.objects.OfType<Rope>().Select(obj => obj.ToOption())];
            List<LineOption> lineOptions = [.. this.grounds.OfType<Line>().Select(obj => obj.ToOption())];
            List<CurveOption> curveOptions = [.. this.grounds.OfType<Curve>().Select(obj => obj.ToOption())];

            return new ObjectSaveData {
                circles = circleOptions,
                squares = squareOptions,
                triangles = triangleOptions,
                ropes = ropeOptions,
                lines = lineOptions,
                curves = curveOptions
            };
        }

        class QueueObject {
            public string id = IdGenerator.CreateId(10);
            public CommandType command;
            public IObject? target;
        }

        class QueueGround {
            public string id = IdGenerator.CreateId(10);
            public CommandType command;
            public IGround? target;
        }

        enum CommandType {
            Add = 0,
            Remove = 1
        }
    }
}
