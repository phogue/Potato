namespace Procon.Core.Test.ExecutableCommands.Objects {
    public class ExecutableEnumTester : ExecutableBase {

        public ExecutableFlagsEnum TestExecutableFlagsEnum { get; set; }

        public ExecutableEnum TestExecutableEnum { get; set; }

        /// <summary>
        /// Tests that a flags enumerator will be passed through
        /// </summary>
        [CommandAttribute(CommandType = CommandType.VariablesSet)]
        public CommandResultArgs SetTestFlagsEnum(Command command, ExecutableFlagsEnum value) {
            CommandResultArgs result = command.Result;

            this.TestExecutableFlagsEnum = value;

            return result;
        }

        /// <summary>
        /// Tests that a enumerator will be passed through
        /// </summary>
        [CommandAttribute(CommandType = CommandType.VariablesSet)]
        public CommandResultArgs SetTestEnum(Command command, ExecutableEnum value) {
            CommandResultArgs result = command.Result;

            this.TestExecutableEnum = value;

            return result;
        }
    }
}
