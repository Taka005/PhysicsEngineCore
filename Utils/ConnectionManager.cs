using PhysicsEngineCore.Objects;

namespace PhysicsEngineCore.Utils{
    class ConnectionManager{
        public List<Target> targets = [];

        public Target? Get(string entityName){
            return this.targets.FirstOrDefault(target => target.entity.name == entityName);
        }
        public void Add(Entity entity, double distance, double stiffness){
            if(this.Get(entity.name) != null) throw new Exception("同じ名前のターゲットが既に存在します");

            Target target = new Target(entity, distance, stiffness);

            this.targets.Add(target);
        }

        public void Remove(string entityName){
            Target? target = this.Get(entityName);
            if(target == null) throw new Exception("ターゲットが見つかりません");

            this.targets.RemoveAll(target=>target.entity.name == entityName);
        }
    }
}
