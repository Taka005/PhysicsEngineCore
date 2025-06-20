﻿using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Options;

namespace PhysicsEngineCore.Utils {

    /// <summary>
    /// オブジェクトを管理します
    /// </summary>
    class ContentManager() {
        private readonly List<IObject> _objects = [];
        private readonly List<IGround> _grounds = [];
        private readonly List<IObject> tracks = [];
        private readonly List<QueueObject> queueObjects = [];
        private readonly List<QueueGround> queueGrounds = [];
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
        /// 現在のグラウンドリストのコピーを返します。
        /// </summary>
        public List<IGround> grounds {
            get {
                lock(this.lockObject) {
                    return [.. this._grounds];
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

        public void RemoveAllObjects() {
            lock(this.lockObject) {
                foreach(IObject obj in this._objects) {
                    this.RemoveObject(obj);
                }
            }
        }

        public void RemoveAllGrounds() {
            lock(this.lockObject) {
                foreach(IGround ground in this._grounds) {
                    this.RemoveGround(ground);
                }
            }
        }


        /// <summary>
        /// 待機列にあるオブジェクトを処理します
        /// </summary>
        public void Sync() {
            lock(this.lockObject) {
                if(this.queueObjects.Count == 0 && this.queueGrounds.Count == 0) return;

                List<QueueObject> currentQueueObjects = [.. this.queueObjects];
                List<QueueGround> currentQueueGrounds = [.. this.queueGrounds];

                this.queueObjects.Clear();
                this.queueGrounds.Clear();

                foreach(QueueObject obj in currentQueueObjects) {
                    if(obj.target == null) continue;

                    if(obj.command == CommandType.Add) {
                        this._objects.Add(obj.target);
                    } else if(obj.command == CommandType.Remove) {
                        this._objects.RemoveAll(target => target.id == obj.target.id);
                    }
                }

                foreach(QueueGround ground in currentQueueGrounds) {
                    if(ground.target == null) continue;

                    if(ground.command == CommandType.Add) {
                        this._grounds.Add(ground.target);
                    } else if(ground.command == CommandType.Remove) {
                        this._grounds.RemoveAll(target => target.id == ground.target.id);
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
