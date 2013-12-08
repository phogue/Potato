using System.Collections.Generic;

namespace Procon.Core {
    /// <summary>
    /// The implementing object accepts command execution
    /// </summary>
    public interface IExecutableBase {
        /// <summary>
        /// All objects to tunnel downwards during execution
        /// </summary>
        List<IExecutableBase> TunnelObjects { get; set; }

        /// <summary>
        /// All objects to bubble upwards during execution
        /// </summary>
        List<IExecutableBase> BubbleObjects { get; set; }

        /// <summary>
        /// Run a preview of a command on the current object, then tunnel or bubble the command.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="tunnel">If the command should then be tunneled or bubbled</param>
        /// <returns>The result of the preview. A handler may have canceled the command.</returns>
        CommandResultArgs PropogatePreview(Command command, bool tunnel = true);

        /// <summary>
        /// Run a command on the current object, then tunnel or bubble the command.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="tunnel">If the command should then be tunneled or bubbled</param>
        /// <returns>The result of the execution.</returns>
        CommandResultArgs PropogateHandler(Command command, bool tunnel = true);

        /// <summary>
        /// Alert the object that a command has been executed with the following results
        /// </summary>
        /// <param name="command"></param>
        /// <param name="tunnel">If the command should then be tunneled or bubbled</param>
        /// <returns>The result of the executed.</returns>
        CommandResultArgs PropogateExecuted(Command command, bool tunnel = true);

        /// <summary>
        /// Execute a command, then tunnel it if the dispatch fails or remains as continuing
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        CommandResultArgs Tunnel(Command command);

        /// <summary>
        /// Execute a command, then bubble it if the dispatch fails or remains as continuing
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        CommandResultArgs Bubble(Command command);
    }
}
