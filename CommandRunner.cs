using System.Reflection;
using PhysicsEngineCore.Exceptions;

namespace PhysicsEngineCore {
    public class CommandRunner{

        private readonly Engine engine;

        private readonly Dictionary<string, object> globalVariables = [];

        private readonly Dictionary<string, PropertyInfo> propertyCache = [];
        private readonly Dictionary<string, FieldInfo> fieldCache = [];

        public CommandRunner(Engine engine) {
            this.engine = engine;

            this.Clear();
        }

        /// <summary>
        /// コマンドを全てリセットします
        /// </summary>
        public void Clear() {
            this.globalVariables.Clear();

            this.globalVariables.Add("gravity", this.engine.gravity);
            this.globalVariables.Add("friction", this.engine.friction);
            this.globalVariables.Add("pps", this.engine.pps);
            this.globalVariables.Add("pi", Math.PI);
            this.globalVariables.Add("e", Math.E);
        }

        /// <summary>
        /// コマンドを複数行実行します
        /// </summary>
        /// <param name="commandList">実行するコマンドのリスト</param>
        public void ExecuteMultiLine(string commandList) {
            if(string.IsNullOrWhiteSpace(commandList)) return;

            Dictionary<string, object> localVariables = [];

            string[] commands = commandList.Split(["\n", "\r"], StringSplitOptions.RemoveEmptyEntries);

            foreach(string command in commands) {
                this.Execute(command, localVariables);
            }
        }

        /// <summary>
        /// コマンドを実行します
        /// </summary>
        /// <param name="command">実行するコマンド</param>
        /// <param name="localVariables">ローカル変数の辞書</param>
        /// <returns>コマンドの実行結果</returns>
        /// <exception cref="Exception">存在しないコマンドの時にエラー</exception>
        public string? Execute(string command,Dictionary<string, object> localVariables) {
            string[] parts = command.Split(" ");
            string commandName = parts[0];

            if(commandName == "/set") {
                this.HandleSetCommand([.. parts.Skip(1)], localVariables);
            } else if(commandName == "/get") {
                this.HandleGetCommand([.. parts.Skip(1)], localVariables);
            } else if(commandName == "/update") {
                this.HandleUpdateCommand([.. parts.Skip(1)], localVariables);
            } else if(commandName == "/calc") {
                this.HandleCalcCommand([.. parts.Skip(1)], localVariables);
            } else if(commandName == "/clear") {
                this.Clear();
            } else if(commandName == "/func") {
                this.HandleFuncCommand([.. parts.Skip(1)], localVariables);
            }else if(commandName == "/if") {
                return this.HandleIfCommand([.. parts.Skip(1)], localVariables);
            } else if(commandName == "/console") {
                return this.HandleConsoleCommand([.. parts.Skip(1)], localVariables);
            } else if(commandName == "/help") {
                return "利用可能なコマンド\n" +
                    "/set <変数名> <値|変数> [global] - 変数を設定します。globalを指定するとグローバル変数になります\n" +
                    "/get <変数名> <オブジェクトID> [global] - オブジェクトを取得し、変数に設定します\n" +
                    "/update <変数名:プロパティー名> <値|変数> - オブジェクトのプロパティを更新します\n" +
                    "/calc <結果変数名> <値1|変数1> <演算子> <値2|変数2> - 数学的な計算を行い、計算結果を結果変数に設定します\n" +
                    "/func <結果変数名> <関数名> <値|変数> - 関数を値を入力し、計算結果を結果変数に設定します\n" +
                    "/if <条件式> <コマンド...> - 条件式が真の場合、指定されたコマンドを実行します\n" +
                    "/console <変数名> - 変数をコンソールに出力します\n" +
                    "/clear - グローバル変数をリセットします\n" +
                    "変数以外は\"\"で囲む必要があります";
            } else {
                throw new CommandException($"不明なコマンド: {commandName}");
            }

            return null;
        }

        /// <summary>
        /// Setコマンドを制御します
        /// </summary>
        /// <param name="args">引数の配列</param>
        /// <param name="localVariables">ローカル変数の辞書</param>
        /// <exception cref="Exception">不整合な引数の場合にエラー</exception>
        private void HandleSetCommand(string[] args, Dictionary<string, object> localVariables) {
            if(args.Length < 2) throw new CommandException("Setコマンドの引数の数が正しくありません。引数は2つである必要があります","/set");

            string varName = args[0];
            object? value = this.SolveVariable(args[1], localVariables);
            bool isGlobal = args.Length > 2 && args[2].ToLower() == "global";

            if(value == null) throw new CommandException($"値 ' {args[1]} ' を解決できませんでした","/set");

            if(localVariables.ContainsKey(varName) || this.globalVariables.ContainsKey(varName)) {
                this.SetVariable(varName, value, localVariables);
            } else {
                if(isGlobal) {
                    this.globalVariables[varName] = value;
                } else {
                    localVariables[varName] = value;
                }
            }
        }

        /// <summary>
        /// Calcコマンドを制御します
        /// </summary>
        /// <param name="args">引数の配列</param>
        /// <param name="localVariables">ローカル変数の辞書</param>
        /// <exception cref="Exception">不整合な引数の場合にエラー</exception>
        private void HandleCalcCommand(string[] args, Dictionary<string, object> localVariables) {
            if(args.Length != 4) throw new CommandException("Setコマンドの引数の数が正しくありません。引数は2つである必要があります", "/calc");

            string resultVarName = args[0];
            object? strValue = this.SolveVariable(args[1],localVariables);
            string operatorSymbol = args[2];
            object? strValue2 = this.SolveVariable(args[3], localVariables);

            if(strValue == null || strValue2 == null) throw new CommandException("値を解決できませんでした", "/calc");

            if(double.TryParse(strValue.ToString(), out double value) && double.TryParse(strValue2.ToString(), out double value2)) {
                double result = 0;
                switch(operatorSymbol) {
                    case "+":
                        result = value + value2;

                        break;
                    case "-":
                        result = value - value2;

                        break;
                    case "*":
                        result = value * value2;

                        break;
                    case "/":
                        if(value2 == 0) throw new CommandException("0で割ることはできません", "/calc");

                        result = value / value2;

                        break;
                    case "%":
                        if(value2 == 0) throw new CommandException("0で割ることはできません", "/calc");

                        result = value % value2;

                        break;
                    case "^":
                        result = Math.Pow(value, value2);

                        break;
                    default:
                        throw new CommandException($"サポートされていない演算子です {operatorSymbol}", "/calc");
                }

                this.SetVariable(resultVarName, result, localVariables);
            }
        }

        /// <summary>
        /// Funcコマンドを制御します
        /// </summary>
        /// <param name="args">引数の配列</param>
        /// <param name="localVariables">ローカル変数の辞書</param>
        /// <exception cref="Exception">不整合な引数の場合にエラー</exception>
        private void HandleFuncCommand(string[] args, Dictionary<string, object> localVariables) {
            if(args.Length != 3) throw new CommandException("Funcコマンドの引数の数が正しくありません。引数は3つである必要があります", "/func");

            string resultVarName = args[0];
            string operatorFunc = args[1].ToLower();
            object? strValue = this.SolveVariable(args[2], localVariables);

            if(strValue == null) throw new CommandException("値を解決できませんでした", "/func");

            if(double.TryParse(strValue.ToString(), out double value)) {
                double result = 0;
                switch(operatorFunc) {
                    case "sin":
                        result = Math.Sin(value * (Math.PI / 180));

                        break;
                    case "cos":
                        result = Math.Cos(value * (Math.PI / 180));

                        break;
                    case "tan":
                        result = Math.Tan(value * (Math.PI / 180));

                        break;
                    case "abs":
                        result = Math.Abs(value);

                        break;
                    case "sqrt":
                        if(value < 0) throw new CommandException("負の数の平方根は計算できません", "/func");

                        result = Math.Sqrt(value);

                        break;
                    default:
                        throw new CommandException($"サポートされていない関数です: {operatorFunc}", "/func");
                }

                this.SetVariable(resultVarName, result, localVariables);
            }
        }

        /// <summary>
        /// Consoleコマンドを制御します
        /// </summary>
        /// <param name="args">引数の配列</param>
        /// <param name="localVariables">ローカル変数の辞書</param>
        /// <exception cref="Exception">不整合な引数の場合にエラー</exception>
        private string? HandleConsoleCommand(string[] args, Dictionary<string, object> localVariables) {
            if(args.Length != 1) throw new CommandException("Consoleコマンドの引数の数が正しくありません。引数は1つである必要があります", "/console");
            object? varData = this.SolveVariable(args[0], localVariables);

            if(varData == null) throw new CommandException($"変数 '{args[0]}' を解決できませんでした", "/console");

            return varData.ToString();
        }

        /// <summary>
        /// Getコマンドを制御します
        /// </summary>
        /// <param name="args">引数の配列</param>
        /// <param name="localVariables">ローカル変数の辞書</param>
        /// <exception cref="Exception">不整合な引数の場合にエラー</exception>
        private void HandleGetCommand(string[] args, Dictionary<string, object> localVariables) {
            if(args.Length < 2) throw new CommandException("Getコマンドの引数の数が正しくありません。引数は2つである必要があります", "/get");

            string varName = args[0];
            object? value = this.GetAnyObject(args[1]);
            bool isGlobal = args.Length > 2 && args[2].ToLower() == "global";

            if(value == null) throw new CommandException($"オブジェクトID ' {args[1]} ' を解決できませんでした", "/get");

            if(isGlobal) {
                this.globalVariables[varName] = value;
            } else {
                localVariables[varName] = value;
            }
        }

        /// <summary>
        /// Updateコマンドを制御します
        /// </summary>
        /// <param name="args">引数の配列</param>
        /// <param name="localVariables">ローカル変数の辞書</param>
        /// <exception cref="Exception">不整合な引数の場合にエラー</exception>
        private void HandleUpdateCommand(string[] args, Dictionary<string, object> localVariables) {
            if(args.Length < 2) throw new CommandException("Updateコマンドの引数の数が正しくありません。引数は2つである必要があります", "/update");

            string fullPropertyPath = args[0];
            object? valueToSet = this.SolveVariable(args[1],localVariables);

            if(valueToSet == null) throw new CommandException($"値 ' {args[1]} ' を解決できませんでした", "/update");

            string[] pathParts = fullPropertyPath.Split(":");
            if(pathParts.Length < 2) throw new CommandException("プロパティパスの形式が正しくありません。'変数名:プロパティ名'の形式で指定してください", "/update");

            object currentObject = this.GetVariable(pathParts[0], localVariables);

            for(int i = 1;i < pathParts.Length - 1;i++) {
                object? nextObject = this.GetObjectProperty(currentObject, pathParts[i]);

                if(nextObject == null) throw new CommandException($"プロパティ'{pathParts[i]}'が見つからないか、nullです", "/update");

                currentObject = nextObject;
            }

            this.SetObjectProperty(currentObject, pathParts[^1], valueToSet);
        }

        /// <summary>
        /// Ifコマンドを制御します
        /// </summary>
        /// <param name="args">引数の配列</param>
        /// <param name="localVariables">ローカル変数の辞書</param>
        /// <exception cref="Exception">不整合な引数の場合にエラー</exception>
        private string? HandleIfCommand(string[] args, Dictionary<string, object> localVariables) {
            if(args.Length < 3) throw new CommandException("Ifコマンドの引数の数が正しくありません。'/if <条件式> <コマンド...>' の形式で指定してください", "/if");

            string condition = args[0];
            string commandToExecute = string.Join(" ", args.Skip(1));

            if(!this.EvaluateCondition(condition, localVariables)) return null;

            return this.Execute(commandToExecute, localVariables);
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
                string[] pathParts = varName.Split(":");
                object? value = this.GetVariable(pathParts[0], localVariables);

                if(pathParts.Length > 1) {
                    for(int i = 1;i < pathParts.Length - 1;i++) {
                        value = this.GetObjectProperty(value, pathParts[i]);

                        if(value == null) throw new CommandException($"プロパティ'{pathParts[i]}'が見つからないか、nullです");
                    }

                    return this.GetObjectProperty(value, pathParts[^1]);
                } else {
                    return value;
                }
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
                throw new CommandException($"変数 '{varName}' が存在しませんでした");
            }
        }

        /// <summary>
        /// 変数の値を設定します
        /// </summary>
        /// <param name="varName">設定する変数</param>
        /// <param name="value">設定する値</param>
        /// <param name="localVariables">ローカル変数の辞書</param>
        private void SetVariable(string varName, object value, Dictionary<string, object> localVariables) {
            if(localVariables.ContainsKey(varName)) {
                localVariables[varName] = value;
            } else {
                this.globalVariables[varName] = value;
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
            Type objType = obj.GetType();

            string cacheKey = $"{objType.FullName}:{propName}";

            if(propertyCache.TryGetValue(cacheKey, out PropertyInfo? prop)){
                return prop.GetValue(obj);
            }

            if(fieldCache.TryGetValue(cacheKey, out FieldInfo? field)){
                return field.GetValue(obj);
            }

            prop = objType.GetProperty(propName);
            if(prop != null){
                propertyCache[cacheKey] = prop;

                return prop.GetValue(obj);
            }

            field = objType.GetField(propName);
            if(field != null){
                fieldCache[cacheKey] = field;

                return field.GetValue(obj);
            }

            throw new CommandException($"プロパティまたはフィールド '{propName}' がオブジェクト '{objType.Name}' に存在しません");
        }

        /// <summary>
        /// オブジェクトのプロパティを設定します
        /// </summary>
        /// <param name="obj">設定するオブジェクト</param>
        /// <param name="propName">設定するプロパティー</param>
        /// <param name="value">設定する値</param>
        /// <exception cref="Exception">存在しないプロパティー、書き込み不可の時にエラー</exception>
        private void SetObjectProperty(object obj, string propName, object strValue) {
            PropertyInfo? prop = obj.GetType().GetProperty(propName);
            if(prop != null) {
                if(!prop.CanWrite) throw new CommandException($"プロパティ '{propName}' は書き込み不可です");

                try {
                    object convertedValue = Convert.ChangeType(strValue, prop.PropertyType);
                    prop.SetValue(obj, convertedValue);

                    return;
                } catch {
                    throw new CommandException($"値 '{strValue}' をプロパティ '{propName}' の型 '{prop.PropertyType.Name}' に変換できませんでした");
                }
            }

            FieldInfo? field = obj.GetType().GetField(propName);
            if(field != null) {
                if(field.IsInitOnly) throw new CommandException($"フィールド '{propName}' は読み取り専用です");

                try {
                    object convertedValue = Convert.ChangeType(strValue, field.FieldType);
                    field.SetValue(obj, convertedValue);

                    return;
                } catch{
                    throw new CommandException($"値 '{strValue}' をフィールド '{propName}' の型 '{field.FieldType.Name}' に変換できませんでした");
                }
            }

            throw new CommandException($"プロパティまたはフィールド '{propName}' がオブジェクト '{obj.GetType().Name}' に存在しません");
        }


        /// <summary>
        /// 指定されたIDに関連するオブジェクトを取得します
        /// Object、Ground、Effectの順で優先的に取得されます
        /// </summary>
        /// <param name="id">オブジェクトID</param>
        /// <returns>取得されたオブジェクト</returns>
        private object? GetAnyObject(string id) {
            return (object?)this.engine.GetObject(id) ?? (object?)this.engine.GetGround(id) ?? this.engine.GetEffect(id);
        }

        /// <summary>
        /// 条件式を評価します
        /// </summary>
        /// <param name="condition">評価する条件式</param>
        /// <param name="localVariables">ローカル変数の辞書</param>
        /// <returns>条件式の評価</returns>
        /// <exception cref="CommandException">不整合な引数の場合にエラー</exception>
        private bool EvaluateCondition(string condition, Dictionary<string, object> localVariables) {
            bool negate = condition.StartsWith("!");
            string varName = negate ? condition[1..] : condition;

            if(
                !varName.Contains("==") &&
                !varName.Contains("!=") &&
                !varName.Contains(">") &&
                !varName.Contains("<")
            ) {
                bool exists = localVariables.ContainsKey(varName) || this.globalVariables.ContainsKey(varName);

                return negate ? !exists : exists;
            }

            string[] parts;
            string op;

            if(condition.Contains("==")) {
                parts = condition.Split("==");
                op = "==";
            } else if(condition.Contains("!=")) {
                parts = condition.Split("!=");
                op = "!=";
            }else if (condition.Contains(">")){
                parts = condition.Split(">");
                op = ">";
            }else if (condition.Contains("<")){
                parts = condition.Split("<");
                op = "<";
            } else {
                throw new CommandException($"サポートされていない演算子です: {condition}");
            }

            if(parts.Length != 2) throw new CommandException("条件式の形式が正しくありません");

            object? left = this.SolveVariable(parts[0], localVariables);
            object? right = this.SolveVariable(parts[1], localVariables);

            if(left == null || right == null) throw new CommandException("条件式内の値を解決できませんでした");

            if(op == ">" || op == "<"){
                if(double.TryParse(left.ToString(), out double leftNum) && double.TryParse(right.ToString(), out double rightNum)){
                    return op switch{
                        ">" => leftNum > rightNum,
                        "<" => leftNum < rightNum,
                        _ => throw new CommandException($"サポートされていない演算子です: {op}"),
                    };
                } else {
                    throw new CommandException("条件式内の値を数値に変換できませんでした");
                }
            }else{
                string leftStr = left.ToString() ?? "";
                string rightStr = right.ToString() ?? "";

                return op switch{
                    "==" => leftStr == rightStr,
                    "!=" => leftStr != rightStr,
                    _ => throw new CommandException($"サポートされていない演算子です: {op}"),
                };
            }

        }
    }
}
