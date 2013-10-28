using System.Collections.Generic;
using Procon.Core.Variables;
using Procon.Core.Events;

namespace Procon.Core.Test.ExecutableCommands.Objects {
    public class ExecutableOverrideTester : ExecutableBase {
        public int TestNumber { get; set; }

        /// <summary>
        /// Sets the value of the test flag.
        /// </summary>
        [CommandAttribute(CommandType = CommandType.VariablesSet)]
        public CommandResultArgs SetTestFlag(Command command, int value) {
            this.TestNumber = value;

            return new CommandResultArgs() {
                Success = true,
                Status = CommandResultType.Success,
                Message = "Set Number",
                Now = new CommandData() {
                    Variables = new List<Variable>() {
                        new Variable() {
                            Name = "TestNumber",
                            Value = this.TestNumber
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Sets the value of the test flag.
        /// </summary>
        [CommandAttribute(CommandType = CommandType.VariablesSet)]
        public CommandResultArgs SetTestFlag(Command command, float value) {
            return this.SetTestFlag(command, (int)value);
        }
    }
}
