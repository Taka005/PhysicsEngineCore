using PhysicsEngineCore.Objects;

namespace PhysicsEngineCore.Utils{
    public class ConnectionManager(List<Target>? targets){
        public List<Target> targets = targets ?? [];

        public Target? Get(string entityName){
            return this.targets.FirstOrDefault(target => target.entity.id == entityName);
        }
        public void Add(Entity entity, double distance, double stiffness){
            if(this.Get(entity.id) != null) throw new Exception("同じIDのターゲットが既に存在します");

            Target target = new Target(entity, distance, stiffness);

            this.targets.Add(target);
        }

        public void Remove(string entityName){
            Target? target = this.Get(entityName);
            if(target == null) throw new Exception("ターゲットが見つかりません");

            this.targets.RemoveAll(target=>target.entity.id == entityName);
        }
    }
}
