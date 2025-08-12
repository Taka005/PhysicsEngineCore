using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsEngineCore{
    class CommandRunner(Engine engine){
        private readonly Engine engine = engine;

        private readonly Dictionary<string, object> globalVariables = [];

        public void Execute(string command) {
            string[] parts = command.Split(" ");
            string commandName = parts[0];

            if(commandName == "/fill") {

            } else {
                throw new Exception($"不明なコマンド: {commandName}");
            }
        }

        private void HandleFillCommand(string[] commandParts) {
        
        }
    }
}
