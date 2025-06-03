namespace PhysicsEngineCore.Utils{

    /// <summary>
    /// エンティティーの接続を表すクラス
    /// </summary>
    /// <param name="entityId">接続先のエンティティーID</param>
    /// <param name="distance">接続距離</param>
    /// <param name="stiffness">接続の剛性</param>
    public class Target(string entityId, double distance, double stiffness){
        public readonly string entityId = entityId;
        public double distance = distance;
        public double stiffness = stiffness;
    }
}