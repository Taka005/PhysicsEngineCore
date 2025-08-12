using System.Reflection;

namespace PhysicsEngineCore{
    class CommandRunner(Engine engine){
        private readonly Engine engine = engine;

        private readonly Dictionary<string, object> globalVariables = [];

        public void Execute(string command,Dictionary<string, object> localVariables) {
            string[] parts = command.Split(" ");
            string commandName = parts[0];

            if(commandName == "/set") {
                this.HandleSetCommand(parts.Skip(1).ToArray(),localVariables);
            } else {
                throw new Exception($"不明なコマンド: {commandName}");
            }
        }

        private void HandleSetCommand(string[] args, Dictionary<string, object> localVariables) {
            if(args.Length < 2) throw new Exception("Setコマンドの引数の数が正しくありません。引数は二つである必要があります");

            string varName = args[0];
            string value = args[1];
            bool isGlobal = args.Length > 2 && args[2].ToLower() == "global";

            string[] parts = varName.Split(":");

            if(parts.Length == 2) {
            
            }else if(parts.Length == 1) {
                if(isGlobal) {
                    globalVariables[varName] = value;
                } else {
                    localVariables[varName] = value;
                }
            } else {
                throw new Exception("変数名の形式が正しくありません。varName:propertyNameである必要があります");
            }
        }

        private object? GetObjectProperty(object obj, string propName) {
            PropertyInfo? prop = obj.GetType().GetProperty(propName);
            if(prop == null) throw new Exception($"プロパティ '{propName}' がオブジェクト '{obj.GetType().Name}' に存在しません");

            return prop.GetValue(obj);
        }
    }
}
