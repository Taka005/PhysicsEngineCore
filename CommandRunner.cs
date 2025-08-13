using System.Diagnostics.Eventing.Reader;
using System.Reflection;

namespace PhysicsEngineCore{
    class CommandRunner(Engine engine){
        private readonly Engine engine = engine;

        private readonly Dictionary<string, object> globalVariables = [];

        public void Execute(string command,Dictionary<string, object> localVariables) {
            string[] parts = command.Split(" ");
            string commandName = parts[0];

            if(commandName == "/set") {
                this.HandleSetCommand(parts.Skip(1).ToArray(), localVariables);
            }else if(commandName == "/get") {

            } else {
                throw new Exception($"不明なコマンド: {commandName}");
            }
        }

        private void HandleSetCommand(string[] args, Dictionary<string, object> localVariables) {
            if(args.Length < 2) throw new Exception("Setコマンドの引数の数が正しくありません。引数は2つである必要があります");

            string varName = args[0];
            object? value = this.SolveVariable(args[1], localVariables);
            bool isGlobal = args.Length > 2 && args[2].ToLower() == "global";

            if(value == null) throw new Exception($"' {args[1]} ' を解決できませんでした");

            if(isGlobal) {
                this.globalVariables[varName] = value;
            } else {
                localVariables[varName] = value;
            }
        }

        private void HandleGetCommand(string[] args, Dictionary<string, object> localVariables) {

        }

        private object? SolveVariable(string varName, Dictionary<string, object> localVariables) {
            string[] parts = varName.Split(":");
            Object? value;

            if(localVariables.TryGetValue(parts[0], out object? localValue)) {
                value = localValue;
            }else if(this.globalVariables.TryGetValue(parts[0], out object? globalValue)) {
                value = globalValue;
            } else {
                value = varName;
            }

            return parts.Length > 1 ? this.GetObjectProperty(value, parts[1]) : value;
        }

        private object? GetObjectProperty(object obj, string propName) {
            PropertyInfo? prop = obj.GetType().GetProperty(propName);
            if(prop == null) throw new Exception($"プロパティ '{propName}' がオブジェクト '{obj.GetType().Name}' に存在しません");

            return prop.GetValue(obj);
        }
    }
}
