using PhysicsEngineCore.Objects;

namespace PhysicsEngineCore.Utils{
    class ContentManager{
        private readonly List<IObject> objects = [];
        private readonly List<IGround> grounds = [];
        private readonly List<QueueObject> queueObjects = [];
        private readonly List<QueueGround> queueGrounds = [];

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
            QueueGround queue = new QueueGround {
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
            this.queueObjects.ForEach(obj =>{
                if(obj.target == null) return;

                if(obj.command == CommandType.Add){
                    this.objects.Add(obj.target);
                }else if(obj.command == CommandType.Remove){
                    this.objects.Remove(obj.target);
                }
            });

            this.queueGrounds.ForEach(obj=>{
                if(obj.target == null) return;

                if(obj.command == CommandType.Add){
                    this.grounds.Add(obj.target);
                }else if(obj.command == CommandType.Remove){
                    this.grounds.Remove(obj.target);
                }
            });
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
