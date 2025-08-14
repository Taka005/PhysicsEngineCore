using System.Reflection;

namespace PhysicsEngineCore{
    public class CommandRunner(Engine engine){
        private readonly Engine engine = engine;

        private readonly Dictionary<string, object> globalVariables = [];

        /// <summary>
        /// コマンドを実行します
        /// </summary>
        /// <param name="command">実行するコマンド</param>
        /// <param name="localVariables">ローカル変数の辞書</param>
        /// <exception cref="Exception">存在しないコマンドの時にエラー</exception>
        public void Execute(string command,Dictionary<string, object> localVariables) {
            string[] parts = command.Split(" ");
            string commandName = parts[0];

            if(commandName == "/set") {
                this.HandleSetCommand([.. parts.Skip(1)], localVariables);
            }else if(commandName == "/get") {
                this.HandleGetCommand([.. parts.Skip(1)], localVariables);
            } else if(commandName == "/update") {
                this.HandleUpdateCommand([.. parts.Skip(1)], localVariables);
            } else {
                throw new Exception($"不明なコマンド: {commandName}");
            }
        }

        private void HandleSetCommand(string[] args, Dictionary<string, object> localVariables) {
            if(args.Length < 2) throw new Exception("Setコマンドの引数の数が正しくありません。引数は2つである必要があります");

            string varName = args[0];
            object? value = this.SolveVariable(args[1], localVariables);
            bool isGlobal = args.Length > 2 && args[2].ToLower() == "global";

            if(value == null) throw new Exception($"値 ' {args[1]} ' を解決できませんでした");

            if(isGlobal) {
                this.globalVariables[varName] = value;
            } else {
                localVariables[varName] = value;
            }
        }

        private void HandleGetCommand(string[] args, Dictionary<string, object> localVariables) {
            if(args.Length < 2) throw new Exception("Getコマンドの引数の数が正しくありません。引数は2つである必要があります");

            string varName = args[0];
            object? value = this.GetAnyObject(args[1]);
            bool isGlobal = args.Length > 2 && args[2].ToLower() == "global";

            if(value == null) throw new Exception($"オブジェクトID ' {args[1]} ' を解決できませんでした");

            if(isGlobal) {
                this.globalVariables[varName] = value;
            } else {
                localVariables[varName] = value;
            }
        }

        private void HandleUpdateCommand(string[] args, Dictionary<string, object> localVariables) {
            if(args.Length < 2) throw new Exception("Updateコマンドの引数の数が正しくありません。引数は2つである必要があります");

            string varName = args[0];
            string[] parts = varName.Split(":");
            object value = this.GetVariable(parts[0], localVariables);

            this.SetObjectProperty(value, parts[1], args[2]);
        }

        /// <summary>
        /// 変数または値を解決します
        /// </summary>
        /// <param name="varName">解決する変数または値</param>
        /// <param name="localVariables">ローカル変数の辞書</param>
        /// <returns>解決したときのオブジェクト</returns>
        private object? SolveVariable(string varName, Dictionary<string, object> localVariables) {
            if(varName.StartsWith("\"") && varName.EndsWith("\"")) {
                return varName[1..^1];
            }else {
                string[] parts = varName.Split(":");
                object value = this.GetVariable(parts[0], localVariables);

                return parts.Length > 1 ? this.GetObjectProperty(value, parts[1]) : value;
            }
        }

        /// <summary>
        /// 変数の値を取得します
        /// </summary>
        /// <param name="varName">取得する変数</param>
        /// <param name="localVariables">ローカル変数の辞書</param>
        /// <returns>取得したときの変数の値</returns>
        /// <exception cref="Exception">変数が存在しないときにエラー</exception>
        private object GetVariable(string varName, Dictionary<string, object> localVariables) {
            if(localVariables.TryGetValue(varName, out object? localValue)) {
                return localValue;
            } else if(this.globalVariables.TryGetValue(varName, out object? globalValue)) {
                return globalValue;
            } else {
                throw new Exception($"変数 '{varName}' が存在しませんでした");
            }
        }

        /// <summary>
        /// オブジェクトのプロパティを取得します
        /// </summary>
        /// <param name="obj">取得するオブジェクト</param>
        /// <param name="propName">取得するプロパティー</param>
        /// <returns>取得したプロパティーの値</returns>
        /// <exception cref="Exception">存在しないプロパティーの時にエラー</exception>
        private object? GetObjectProperty(object obj, string propName) {
            PropertyInfo? prop = obj.GetType().GetProperty(propName);
            if(prop == null) throw new Exception($"プロパティ '{propName}' がオブジェクト '{obj.GetType().Name}' に存在しません");

            return prop.GetValue(obj);
        }

        /// <summary>
        /// オブジェクトのプロパティを設定します
        /// </summary>
        /// <param name="obj">設定するオブジェクト</param>
        /// <param name="propName">設定するプロパティー</param>
        /// <param name="value">設定する値</param>
        /// <exception cref="Exception">存在しないプロパティー、書き込み不可の時にエラー</exception>
        private void SetObjectProperty(object obj, string propName, object value) {
            PropertyInfo? prop = obj.GetType().GetProperty(propName);
            if(prop == null) throw new Exception($"プロパティ '{propName}' がオブジェクト '{obj.GetType().Name}' に存在しません");

            if(!prop.CanWrite) throw new Exception($"プロパティ '{propName}' は書き込み不可です");

            prop.SetValue(obj, value);
        }

        /// <summary>
        /// 指定されたIDに関連するオブジェクトを取得します
        /// Object、Ground、Effectの順で優先的に取得されます
        /// </summary>
        /// <param name="id">オブジェクトID</param>
        /// <returns>取得されたオブジェクト</returns>
        private object? GetAnyObject(string id) {
            object? obj = this.engine.GetObject(id);
            object? ground = this.engine.GetGround(id);
            object? effect = this.engine.GetEffect(id);

            if(obj != null) return obj;
            if(ground != null) return ground;
            if(effect != null) return effect;

            return null;
        }
    }
}
