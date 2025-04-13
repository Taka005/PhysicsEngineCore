using System.Threading;

namespace PhysicsEngineCore{
    public class Engine{
        private int pps;
        private bool isStarted = false;
        private bool isTrackingMode = false;
        private bool isDebugMode = false;
        private Timer loopTimer;
        private float playBackSpeed;
        private float trackingInterval;
        private int trackingCount = 0;
        private int trackingLimit;

        public Engine(int pps, float playBackSpeed, float trackingInterval, int trackingLimit){
            this.pps = pps;
            this.playBackSpeed = checkPlayBackSpeedValue(playBackSpeed);
            this.trackingInterval = checkTrackingIntervalValue(trackingInterval);
            this.trackingLimit = checkTrackingLimitValue(trackingLimit);
            this.loopTimer = new Timer(this.loop!, null, 0, (int)((1000 / this.pps) / this.playBackSpeed));
        }

        public void setPlayBackSpeed(float value){
            this.playBackSpeed = checkPlayBackSpeedValue(value);

            if(this.isStarted){
                this.loopTimer.Change(0, (int)((1000 / this.pps) / this.playBackSpeed));
            }
        }

        public void setTrackingInterval(float value){
            this.trackingInterval = checkTrackingIntervalValue(value);
        }

        public void setTrackingLimit(int value){
            this.trackingLimit = checkTrackingLimitValue(value);
        }

        public void start(){
            if(this.isStarted) throw new Exception("既にシステムは開始されています");

            this.isStarted = true;
            this.loopTimer.Change(0, (int)((1000 / this.pps) / this.playBackSpeed));
        }

        public void stop(){
            if(!this.isStarted) throw new Exception("既にシステムは停止しています");

            this.isStarted = false;
            this.trackingCount = 0;
            this.loopTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void loop(Object state){

        }

        

        private static float checkPlayBackSpeedValue(float playBackSpeed){
            if(playBackSpeed < 0) throw new Exception("再生速度(playBackSpeed)は0以上に設定する必要があります");

            return playBackSpeed;
        }

        private static float checkTrackingIntervalValue(float trackingInterval){
            if(trackingInterval < 0) throw new Exception("トラッキング間隔(trackingInterval)は0以上に設定する必要があります");

            return trackingInterval;
        }

        private static int checkTrackingLimitValue(int trackingLimit){
            if(trackingLimit < 0) throw new Exception("トラッキング数(trackingLimit)は0以上に設定する必要があります");

            return trackingLimit;
        }
    }
}
