using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Options;

namespace PhysicsEngineCore.Utils{

    /// <summary>
    /// オブジェクトを管理します
    /// </summary>
    class ContentManager(){
        internal readonly List<IObject> objects = [];
        internal readonly List<IGround> grounds = [];
        private readonly List<QueueObject> queueObjects = [];
        private readonly List<QueueGround> queueGrounds = [];

        /// <summary>
        /// 全てのエンティティー
        /// </summary>
        public List<Entity> entities{
            get{
                return [.. this.objects.SelectMany(obj => obj.entities)];
            }
        }

        /// <summary>
        /// オブジェクトの追加を待機列に追加します
        /// </summary>
        /// <param name="target">追加するオブジェクト</param>
        public void AddObject(IObject target){
            QueueObject queue = new QueueObject{
                command = CommandType.Add,
                target = target
            };

            this.queueObjects.Add(queue);
        }

        /// <summary>
        /// オブジェクトの削除を待機列に追加します
        /// </summary>
        /// <param name="target">削除するオブジェクト</param>
        public void RemoveObject(IObject target){
            QueueObject queue = new QueueObject{
                command = CommandType.Remove,
                target = target
            };

            this.queueObjects.Add(queue);
        }
        
        /// <summary>
        /// グラウンドの追加を待機列に追加します
        /// </summary>
        /// <param name="target">追加するグラウンド</param>
        public void AddGround(IGround target){
            QueueGround queue = new QueueGround{
                command = CommandType.Add,
                target = target
            };

            this.queueGrounds.Add(queue);
        }

        /// <summary>
        /// グラウンドの削除を待機列に追加します
        /// </summary>
        /// <param name="target">削除するグラウンド</param>
        public void RemoveGround(IGround target){
            QueueGround queue = new QueueGround{
                command = CommandType.Add,
                target = target
            };

            this.queueGrounds.Add(queue);
        }

        /// <summary>
        /// 待機列にあるオブジェクトを処理します
        /// </summary>
        public void Sync(){
            if(this.queueObjects.Count == 0 && this.queueGrounds.Count == 0) return;

            this.queueObjects.ToList().ForEach(obj =>{
                if(obj.target == null) return;

                if(obj.command == CommandType.Add){
                    this.objects.Add(obj.target);
                }else if(obj.command == CommandType.Remove){
                    this.objects.RemoveAll(target => target.id == obj.id);
                }

                this.queueObjects.RemoveAll(queue => queue.id == obj.id);
            });

            this.queueGrounds.ToList().ForEach(obj=>{
                if(obj.target == null) return;

                if(obj.command == CommandType.Add){
                    this.grounds.Add(obj.target);
                }else if(obj.command == CommandType.Remove){
                    this.grounds.RemoveAll(target => target.id == obj.id);
                }

                this.queueGrounds.RemoveAll(queue => queue.id == obj.id);
            });
        }

        /// <summary>
        /// セーブデータに変換します
        /// </summary>
        /// <returns>変換されたセーブデータ</returns>
        public ObjectSaveData ToData(){
            List<CircleOption> circleOptions = [.. this.objects.OfType<Circle>().Select(obj => obj.ToOption())];
            List<SquareOption> squareOptions = [.. this.objects.OfType<Square>().Select(obj => obj.ToOption())];
            List<RopeOption> ropeOptions = [.. this.objects.OfType<Rope>().Select(obj => obj.ToOption())];
            List<LineOption> lineOptions = [.. this.grounds.OfType<Line>().Select(obj => obj.ToOption())];
            List<CurveOption> curveOptions = [.. this.grounds.OfType<Curve>().Select(obj => obj.ToOption())];

            return new ObjectSaveData {
                circles = circleOptions,
                squares = squareOptions,
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
