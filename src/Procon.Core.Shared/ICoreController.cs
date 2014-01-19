using System.Collections.Generic;

namespace Procon.Core.Shared {
    /// <summary>
    /// The implementing object accepts command execution
    /// </summary>
    public interface ICoreController {
        /// <summary>
        /// All objects to tunnel downwards during execution
        /// </summary>
        List<ICoreController> TunnelObjects { get; set; }

        /// <summary>
        /// All objects to bubble upwards during execution
        /// </summary>
        List<ICoreController> BubbleObjects { get; set; }

        /// <summary>
        /// Run a preview of a command on the current object, then tunnel or bubble the command.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="direction">If the command should then be tunneled or bubbled</param>
        /// <returns>The result of the preview. A handler may have canceled the command.</returns>
        ICommandResult PropogatePreview(Command command, CommandDirection direction);

        /// <summary>
        /// Run a command on the current object, then tunnel or bubble the command.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="direction">If the command should then be tunneled or bubbled</param>
        /// <returns>The result of the execution.</returns>
        ICommandResult PropogateHandler(Command command, CommandDirection direction);

        /// <summary>
        /// Alert the object that a command has been executed with the following results
        /// </summary>
        /// <param name="command"></param>
        /// <param name="direction">If the command should then be tunneled or bubbled</param>
        /// <returns>The result of the executed.</returns>
        ICommandResult PropogateExecuted(Command command, CommandDirection direction);

        /// <summary>
        /// Execute a command, then tunnel it if the dispatch fails or remains as continuing
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        ICommandResult Tunnel(Command command);

        /// <summary>
        /// Execute a command, then bubble it if the dispatch fails or remains as continuing
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        ICommandResult Bubble(Command command);
    }
}
