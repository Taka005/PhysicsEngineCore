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

        public Engine(int pps, double gravity, double friction, float playBackSpeed, float trackingInterval, int trackingLimit, int movementLimit): base(pps, gravity, friction) {
            this.playBackSpeed = CheckPlayBackSpeedValue(playBackSpeed);
            this.trackingInterval = CheckTrackingIntervalValue(trackingInterval);
            this.trackingLimit = CheckTrackingLimitValue(trackingLimit);
            this.movementLimit = CheckMovementLimitValue(movementLimit);
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
