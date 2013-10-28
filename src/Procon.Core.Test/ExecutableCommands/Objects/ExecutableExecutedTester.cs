namespace Procon.Core.Test.ExecutableCommands.Objects {
    public class ExecutableExecutedTester : ExecutableBasicTester {

        public int ExecutedTestValue { get; set; }

        /// <summary>
        /// Sets the value of the test flag.
        /// </summary>
        [CommandAttribute(CommandType = CommandType.VariablesSet, CommandAttributeType = CommandAttributeType.Executed)]
        public CommandResultArgs SetTestFlagPreview(Command command, int value) {
            CommandResultArgs result = command.Result;

            this.ExecutedTestValue = this.TestNumber * 2;

            return result;
        }
    }
}
