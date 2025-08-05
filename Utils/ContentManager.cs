using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Objects.Interfaces;
using PhysicsEngineCore.Options;

namespace PhysicsEngineCore.Utils {

    /// <summary>
    /// オブジェクトを管理します
    /// </summary>
    class ContentManager() {
        private readonly List<IObject> _objects = [];
        private readonly List<IGround> _grounds = [];
        private readonly List<IEffect> _effects = [];
        private readonly List<QueueObject> queueObjects = [];
        private readonly List<QueueGround> queueGrounds = [];
        private readonly List<QueueEffect> queueEffects = [];
        private readonly object lockObject = new object();

        /// <summary>
        /// 全てのエンティティー
        /// </summary>
        public List<Entity> entities {
            get {
                lock(this.lockObject) {
                    return [.. this._objects.SelectMany(obj => obj.entities)];
                }
            }
        }

        /// <summary>
        /// 現在のオブジェクトの数を返します
        /// </summary>
        public int objectCount {
            get {
                lock(this.lockObject) {
                    return this._objects.Count;
                }
            }
        }

        /// <summary>
        /// 現在のグラウンドの数を返します
        /// </summary>
        public int groundCount {
            get {
                lock(this.lockObject) {
                    return this._grounds.Count;
                }
            }
        }

        /// <summary>
        /// 現在のエフェクトの数を返します
        /// </summary>
        public int effectCount {
            get {
                lock(this.lockObject) {
                    return this._effects.Count;
                }
            }
        }

        /// <summary>
        /// 現在のオブジェクトリストのコピーを返します
        /// </summary>
        public List<IObject> objects {
            get {
                lock(this.lockObject) {
                    return [.. this._objects];
                }
            }
        }

        /// <summary>
        /// 現在のグラウンドリストのコピーを返します
        /// </summary>
        public List<IGround> grounds {
            get {
                lock(this.lockObject) {
                    return [.. this._grounds];
                }
            }
        }

        /// <summary>
        /// 現在のエフェクトリストのコピーを返します
        /// </summary>
        public List<IEffect> effects {
            get {
                lock(this.lockObject) {
                    return [.. this._effects];
                }
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
        /// エフェクトの追加を待機列に追加します
        /// </summary>
        /// <param name="target">追加するエフェクト
        public void AddEffect(IEffect target) {
            if(target == null) throw new ArgumentNullException(nameof(target), "エフェクトがNULLです");

            QueueEffect queue = new QueueEffect {
                command = CommandType.Add,
                target = target
            };

            lock(this.lockObject) {
                this.queueEffects.Add(queue);
            }
        }

        /// <summary>
        /// エフェクトの削除を待機列に追加します
        /// </summary>
        /// <param name="target">削除するエフェクト</param>
        public void RemoveEffect(IEffect target) {
            if(target == null) throw new ArgumentNullException(nameof(target), "エフェクトがNULLです");

            QueueEffect queue = new QueueEffect {
                command = CommandType.Remove,
                target = target
            };

            lock(this.lockObject) {
                this.queueEffects.Add(queue);
            }
        }

        /// <summary>
        /// 全てのオブジェクトの削除を待機列に追加します
        /// </summary>
        public void RemoveAllObjects() {
            QueueObject queue = new QueueObject {
                command = CommandType.ClearAll
            };

            this.queueObjects.Add(queue);
        }

        /// <summary>
        /// 全てのグラウンドの削除を待機列に追加します
        /// </summary>
        public void RemoveAllGrounds() {
            QueueGround queue = new QueueGround {
                command = CommandType.ClearAll
            };

            this.queueGrounds.Add(queue);
        }

        /// <summary>
        /// 全てのエフェクトの削除を待機列に追加します
        /// </summary>
        public void RemoveAllEffects() {
            QueueEffect queue = new QueueEffect {
                command = CommandType.ClearAll
            };

            this.queueEffects.Add(queue);
        }


        /// <summary>
        /// 待機列にあるオブジェクトを処理します
        /// </summary>
        public void Sync() {
            lock(this.lockObject) {
                if(
                    this.queueObjects.Count == 0 &&
                    this.queueGrounds.Count == 0 &&
                    this.queueEffects.Count == 0
                ) return;

                List<QueueObject> currentQueueObjects = [.. this.queueObjects];
                List<QueueGround> currentQueueGrounds = [.. this.queueGrounds];
                List<QueueEffect> currentQueueEffects = [.. this.queueEffects];

                this.queueObjects.Clear();
                this.queueGrounds.Clear();
                this.queueEffects.Clear();

                foreach(QueueObject obj in currentQueueObjects) {
                    if(obj.command == CommandType.ClearAll) {
                        this._objects.Clear();
                    }

                    if(obj.target == null) continue;

                    if(obj.command == CommandType.Add) {
                        this._objects.Add(obj.target);
                    } else if(obj.command == CommandType.Remove) {
                        this._objects.RemoveAll(target => target.id == obj.target.id);
                    }
                }

                foreach(QueueGround ground in currentQueueGrounds) {
                    if(ground.command == CommandType.ClearAll) {
                        this._grounds.Clear();
                    }

                    if(ground.target == null) continue;

                    if(ground.command == CommandType.Add) {
                        this._grounds.Add(ground.target);
                    } else if(ground.command == CommandType.Remove) {
                        this._grounds.RemoveAll(target => target.id == ground.target.id);
                    }                
                }

                foreach(QueueEffect effect in currentQueueEffects) {
                    if(effect.command == CommandType.ClearAll) {
                        this._effects.Clear();
                    }

                    if(effect.target == null) continue;

                    if(effect.command == CommandType.Add) {
                        this._effects.Add(effect.target);
                    } else if(effect.command == CommandType.Remove) {
                        this._effects.RemoveAll(target => target.id == effect.target.id);
                    }
                }
            }
        }

        /// <summary>
        /// セーブデータに変換します
        /// </summary>
        /// <returns>変換されたセーブデータ</returns>
        public ObjectSaveData ToData() {
            List<CircleOption> circleOptions = [.. this._objects.OfType<Circle>().Select(obj => obj.ToOption())];
            List<SquareOption> squareOptions = [.. this._objects.OfType<Square>().Select(obj => obj.ToOption())];
            List<TriangleOption> triangleOptions = [.. this._objects.OfType<Triangle>().Select(obj => obj.ToOption())];
            List<RopeOption> ropeOptions = [.. this._objects.OfType<Rope>().Select(obj => obj.ToOption())];
            List<LineOption> lineOptions = [.. this._grounds.OfType<Line>().Select(obj => obj.ToOption())];
            List<CurveOption> curveOptions = [.. this._grounds.OfType<Curve>().Select(obj => obj.ToOption())];
            List<BoosterOption> effectOptions = [.. this._effects.OfType<Booster>().Select(obj => obj.ToOption())];

            return new ObjectSaveData {
                circles = circleOptions,
                squares = squareOptions,
                triangles = triangleOptions,
                ropes = ropeOptions,
                lines = lineOptions,
                curves = curveOptions,
                boosters = effectOptions
            };
        }

        class QueueObject {
            public string id = IdGenerator.CreateId(Engine.DEFAULT_ID_LENGTH);
            public CommandType command;
            public IObject? target;
        }

        class QueueGround {
            public string id = IdGenerator.CreateId(Engine.DEFAULT_ID_LENGTH);
            public CommandType command;
            public IGround? target;
        }

        class QueueEffect {
            public string id = IdGenerator.CreateId(Engine.DEFAULT_ID_LENGTH);
            public CommandType command;
            public IEffect? target;
        }

        enum CommandType {
            Add = 0,
            Remove = 1,
            ClearAll = 2
        }
    }
}
