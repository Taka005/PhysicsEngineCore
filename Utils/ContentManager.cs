using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Options;

namespace PhysicsEngineCore.Utils{
    class ContentManager{
        internal readonly List<IObject> objects = [];
        internal readonly List<IGround> grounds = [];
        private readonly List<QueueObject> queueObjects = [];
        private readonly List<QueueGround> queueGrounds = [];
        public List<Entity> entities{
            get{
                return [.. this.objects.SelectMany(obj => obj.entities)];
            }
        }

        public void AddObject(IObject target){
            QueueObject queue = new QueueObject{
                command = CommandType.Add,
                target = target
            };

            this.queueObjects.Add(queue);
        }

        public void RemoveObject(IObject target){
            QueueObject queue = new QueueObject{
                command = CommandType.Remove,
                target = target
            };

            this.queueObjects.Add(queue);
        }

        public void AddGround(IGround target){
            QueueGround queue = new QueueGround{
                command = CommandType.Add,
                target = target
            };

            this.queueGrounds.Add(queue);
        }

        public void RemoveGround(IGround target){
            QueueGround queue = new QueueGround{
                command = CommandType.Add,
                target = target
            };

            this.queueGrounds.Add(queue);
        }

        public void Sync(){
            this.queueObjects.ToList().ForEach(obj =>{
                if(obj.target == null) return;

                if(obj.command == CommandType.Add){
                    this.objects.Add(obj.target);
                }else if(obj.command == CommandType.Remove){
                    this.objects.Remove(obj.target);
                }

                this.queueObjects.Remove(obj);
            });

            this.queueGrounds.ToList().ForEach(obj=>{
                if(obj.target == null) return;

                if(obj.command == CommandType.Add){
                    this.grounds.Add(obj.target);
                }else if(obj.command == CommandType.Remove){
                    this.grounds.Remove(obj.target);
                }

                this.queueGrounds.Remove(obj);
            });
        }

        public ContentManagerOption ToOption(){
            List<CircleOption> circleOptions = [.. this.objects.OfType<Circle>().Select(obj => obj.ToOption())];
            List<LineOption> lineOptions = [.. this.grounds.OfType<Line>().Select(obj => obj.ToOption())];
            List<CurveOption> curveOptions = [.. this.grounds.OfType<Curve>().Select(obj => obj.ToOption())];

            return new ContentManagerOption {
                circles = circleOptions,
                lines = lineOptions,
                curves = curveOptions
            };
        }
    }

    class QueueObject{
        public CommandType command;
        public IObject? target;
    }

    class QueueGround{
        public CommandType command;
        public IGround? target;
    }

    enum CommandType{
        Add = 0,
        Remove = 1
    }
}
