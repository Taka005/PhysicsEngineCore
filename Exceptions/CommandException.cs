namespace PhysicsEngineCore.Exceptions{
    class CommandException : Exception {

        public string? targetCommand { get; }

        public CommandException(string message, string? targetCommand = null) : base(message) {
            this.targetCommand = targetCommand;
        }
    }
}
