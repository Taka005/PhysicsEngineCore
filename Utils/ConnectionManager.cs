using PhysicsEngineCore.Objects;

namespace PhysicsEngineCore.Utils {

    /// <summary>
    /// エンティティーの接続の管理
    /// </summary>
    /// <param name="targets">初期のターゲットのリスト</param>
    public class ConnectionManager(List<Target>? targets) {
        public readonly List<Target> _targets = targets ?? [];

        public List<Target> targets{
            get{
                lock (this._targets){
                    return [.. this._targets];
                }
            }
        }

        /// <summary>
        /// ターゲットを取得します
        /// </summary>
        /// <param name="entityId">取得するエンティティーID</param>
        /// <returns>取得したターゲットを返す</returns>
        public Target? Get(string entityId) {
            return this.targets.FirstOrDefault(target => target.entityId == entityId);
        }

        /// <summary>
        /// ターゲットを追加します
        /// </summary>
        /// <param name="entity">追加するエンティティー</param>
        /// <param name="distance">接続する距離</param>
        /// <param name="stiffness">接続する剛性</param>
        /// <exception cref="Exception">同じエンティティーのターゲットがある場合に例外</exception>
        public void Add(Entity entity, double distance, double stiffness) {
            if(this.Get(entity.id) != null) throw new Exception("同じIDのターゲットが既に存在します");

            Target target = new Target(entity.id, distance, stiffness);

            lock(this._targets){
                this._targets.Add(target);
            }
            
        }

        /// <summary>
        /// ターゲットを削除します
        /// </summary>
        /// <param name="entityId">削除する</param>
        /// <exception cref="Exception"></exception>
        public void Remove(string entityId) {
            Target? target = this.Get(entityId);
            if(target == null) throw new Exception("ターゲットが見つかりません");

            lock (this._targets){
                this._targets.RemoveAll(target => target.entityId == entityId);
            }
        }
    }
}
