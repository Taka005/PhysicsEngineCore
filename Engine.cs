using PhysicsEngineCore.Objects;
using PhysicsEngineCore.Options;
using PhysicsEngineCore.Utils;

namespace PhysicsEngineCore{
    public class Engine: Core{
        private bool isStarted = false;
        public bool isTrackingMode = false;
        public bool isDebugMode = false;
        private readonly Timer loopTimer;
        private float playBackSpeed = 1;
        private float trackingInterval = 100;
        private int trackingCount = 0;
        private int trackingLimit = 50000;
        private int movementLimit = 10000;
        private List<IObject> objects = [];
        private List<IGround> grounds = [];

        Engine(EngineOption? option): base(option?.pps ?? 180, option?.gravity ?? 500, option?.friction ?? 0.0001){
            ArgumentNullException.ThrowIfNull(option);

            this.playBackSpeed = CheckPlayBackSpeedValue(option.playBackSpeed);
            this.trackingInterval = CheckTrackingIntervalValue(option.trackingInterval);
            this.trackingLimit = CheckTrackingLimitValue(option.trackingLimit);
            this.movementLimit = CheckMovementLimitValue(option.movementLimit);
            this.loopTimer = new Timer(this.Loop!, null, 0, (int)((1000 / this.pps) / this.playBackSpeed));
        }

        public void SetPlayBackSpeed(float value){
            this.playBackSpeed = CheckPlayBackSpeedValue(value);

            if(this.isStarted){
                this.loopTimer.Change(0, (int)((1000 / this.pps) / this.playBackSpeed));
            }
        }

        public void SetTrackingInterval(float value){
            this.trackingInterval = CheckTrackingIntervalValue(value);
        }

        public void SetTrackingLimit(int value){
            this.trackingLimit = CheckTrackingLimitValue(value);
        }

        public void SetMovementLimit(int value){
            this.movementLimit = CheckMovementLimitValue(value);
        }

        public void Start(){
            if(this.isStarted) throw new Exception("既にシステムは開始されています");

            this.isStarted = true;
            this.loopTimer.Change(0, (int)((1000 / this.pps) / this.playBackSpeed));
        }

        public void Stop(){
            if(!this.isStarted) throw new Exception("既にシステムは停止しています");

            this.isStarted = false;
            this.trackingCount = 0;
            this.loopTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void Loop(Object state){

        }

        public void Import(string saveData){

        }
        public string Export(){
            return "";
        }

        public List<Entity> getEntitiesAt(double posX,double posY){
            Vector2 position = new Vector2(posX,posY);
            List<Entity> targets = [];

            this.entities.forEach(entity=>{
                Vector2 difference = entity.position - position;

                double distance = difference.Length();

                if(distance > entity.size) return;

                targets.push(entity);
            });

            return targets;
        }

        private static float CheckPlayBackSpeedValue(float playBackSpeed){
            if(playBackSpeed < 0) throw new Exception("再生速度(playBackSpeed)は0以上に設定する必要があります");

            return playBackSpeed;
        }

        private static float CheckTrackingIntervalValue(float trackingInterval){
            if(trackingInterval < 0) throw new Exception("トラッキング間隔(trackingInterval)は0以上に設定する必要があります");

            return trackingInterval;
        }

        private static int CheckTrackingLimitValue(int trackingLimit){
            if(trackingLimit < 0) throw new Exception("トラッキング数(trackingLimit)は0以上に設定する必要があります");

            return trackingLimit;
        }

        private static int CheckMovementLimitValue(int movementLimit){
            if(movementLimit < 0) throw new Exception("マップの移動制限(movementLimit)は0以上に設定する必要があります");

            return movementLimit;
        }
    }
}
