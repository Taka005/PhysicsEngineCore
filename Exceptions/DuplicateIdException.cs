namespace PhysicsEngineCore.Exceptions{
    class DuplicateIdException : Exception{

        public readonly string id;

        public DuplicateIdException(string id) : base($"ID ' {id} ' は既に使用されています") {
            this.id = id;
        }
    }
}
