namespace PhysicsEngineCore.Utils {

    /// <summary>
    /// エンティティーの接続を表すクラス
    /// </summary>
    /// <param name="entityId">接続先のエンティティーID</param>
    /// <param name="distance">接続距離</param>
    /// <param name="stiffness">接続の剛性</param>
    public class Target(string entityId, double distance, double stiffness) {
        public string entityId { get; set; } = entityId;
        public double distance { get; set; } = distance;
        public double stiffness { get; set; } = stiffness;
    }
}