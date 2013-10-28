namespace Procon.Core.Test.ExecutableCommands.Objects {
    public class ExecutablePreviewTester : ExecutableBasicTester {
        /// <summary>
        /// Sets the value of the test flag.
        /// </summary>
        [CommandAttribute(CommandType = CommandType.VariablesSet, CommandAttributeType = CommandAttributeType.Preview)]
        public CommandResultArgs SetTestFlagPreview(Command command, int value) {
            CommandResultArgs result = command.Result;

            if (value == 10) {
                result.Status = CommandResultType.None;
            }

            return result;
        }
    }
}
