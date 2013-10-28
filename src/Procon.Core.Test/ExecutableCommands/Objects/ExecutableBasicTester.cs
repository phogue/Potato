using System.Collections.Generic;
using Procon.Core.Variables;
using Procon.Core.Events;

namespace Procon.Core.Test.ExecutableCommands.Objects {
    public class ExecutableBasicTester : ExecutableBase {
        private int _testNumber;
        public int TestNumber {
            get { return _testNumber; }
            set {
                _testNumber = value;
                this.OnPropertyChanged(this, "TestNumber");
            }
        }

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
        /// Gets the value as an integer for the single test flag.
        /// </summary>
        [CommandAttribute(CommandType = CommandType.VariablesGet)]
        public CommandResultArgs GetTestFlag(Command command) {
            return new CommandResultArgs() {
                Success = true,
                Status = CommandResultType.Success,
                Message = "Get Number",
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
        /// Sets the value of the test flag, but with a custom name.
        /// </summary>
        [CommandAttribute(Name = "CustomSet")]
        public CommandResultArgs CustomSetTestFlag(Command command, int value) {
            return this.SetTestFlag(command, value);
        }
    }
}
