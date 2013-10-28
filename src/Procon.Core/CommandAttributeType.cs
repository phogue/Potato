namespace Procon.Core {
    public enum CommandAttributeType {
        /// <summary>
        /// We want to preview a command. This allows for commands to be cancelled
        /// by returning any result status other than Continue.
        /// </summary>
        Preview,
        /// <summary>
        /// The actual executable method which will use the command and return a result.
        /// </summary>
        /// <remarks>This is the default used in the CommandAttribute class.</remarks>
        Handler,
        /// <summary>
        /// Executed shows what command has been previewed and passed, then executed
        /// as well as the result of the execution. A way to see what has been modified
        /// elsewhere.
        /// </summary>
        Executed
    }
}
