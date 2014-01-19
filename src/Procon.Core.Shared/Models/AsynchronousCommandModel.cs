using System;

namespace Procon.Core.Shared.Models {
    /// <summary>
    /// Stores a command to be sent asynchronously as well as the callback (if supplied)
    /// once the command has completed.
    /// </summary>
    [Serializable]
    public class AsynchronousCommandModel : CoreModel {

        /// <summary>
        /// The command being passed around
        /// </summary>
        public Command Command { get; set; }

        /// <summary>
        /// If the command is moving up or down the chain of objects
        /// </summary>
        public bool IsTunneling { get; set; }

        /// <summary>
        /// Callback when a result has been returned for a command.
        /// </summary>
        public Action<ICommandResult> Completed { get; set; }

        /// <summary>
        /// Pass in the result of the command, which will handle calling the completed
        /// delegate. Just a cleaner shorthand.
        /// </summary>
        /// <param name="result">The result of the executed command.</param>
        public void OnResult(ICommandResult result) {
            var handler = Completed;

            if (handler != null) {
                handler(result);
            }
        }
    }
}
