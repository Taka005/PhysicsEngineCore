using System.Threading;


namespace PhysicsEngineCore {
    public class Engine(float playBackSpeed, float trackingInterval, int trackingLimit){
        private float _playBackSpeed = checkPlayBackSpeedValue(playBackSpeed);
        private float _trackingInterval = checkTrackingIntervalValue(trackingInterval);
        private int _trackingLimit = checkTrackingLimitValue(trackingLimit);

        public float playBackSpeed{
            get{
                return _playBackSpeed;
            }
            set{
                _playBackSpeed = checkPlayBackSpeedValue(value);
            }
        }

        public float trackingInterval{
            get{
                return _trackingInterval;
            }
            set{
                _trackingInterval = checkTrackingIntervalValue(value);
            }
        }
        public int trackingLimit{
            get{
                return _trackingLimit;
            }
            set{
                _trackingLimit = checkTrackingLimitValue(value);
            }
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
