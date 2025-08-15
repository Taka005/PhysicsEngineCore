namespace PhysicsEngineCore.Exceptions{
    class ImportException : Exception{
        public ImportException(string message,Exception? innerException = null) : base(message, innerException) {}
    }
}
