using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace Procon.Core.Shared.Models {
    /// <summary>
    /// Stores all information required by the asynchronous controller to pool pending/executed commands and manage multiple tasks.
    /// </summary>
    public class AsynchronousCommandStateModel : CoreModel {

        /// <summary>
        /// A pool of commands yet to be executed by the command execution thread
        /// </summary>
        public ConcurrentQueue<AsynchronousCommandModel> PendingCommandsQueue { get; set; }

        /// <summary>
        /// Command that has (or is just about to be) dispatched for execution.
        /// </summary>
        public ConcurrentDictionary<Guid, AsynchronousCommandModel> ExecutedCommandsPool { get; set; }

        /// <summary>
        /// A cancellation token to stop all execution of commands.
        /// </summary>
        public CancellationTokenSource TaskQueueWaitCancellationTokenSource { get; set; }

        /// <summary>
        /// The signlar to set whenever a command has been added to the pool or the cancellation token has been poked.
        /// </summary>
        public AutoResetEvent TaskQueueWait { get; set; }

        /// <summary>
        /// The number of milliseconds a task should wait before cancelling.
        /// </summary>
        public int TaskTimeout { get; set; }

        /// <summary>
        /// Initalizes with the default values.
        /// </summary>
        public AsynchronousCommandStateModel() : base() {
            this.PendingCommandsQueue = new ConcurrentQueue<AsynchronousCommandModel>();
            this.ExecutedCommandsPool = new ConcurrentDictionary<Guid, AsynchronousCommandModel>();
            this.TaskQueueWaitCancellationTokenSource = new CancellationTokenSource();
            this.TaskQueueWait = new AutoResetEvent(false);
            this.TaskTimeout = 1000;
        }
        
        /// <summary>
        /// Helper to determine if a command is known/propogating/waiting to this controller.
        /// </summary>
        /// <param name="commandGuid">The guid to lookup</param>
        /// <returns>True if the controller currently knows about the command in some context, false otherwise.</returns>
        public bool IsKnown(Guid commandGuid) {
            return this.PendingCommandsQueue.Any(command => command.Command.CommandGuid == commandGuid) || this.ExecutedCommandsPool.ContainsKey(commandGuid);
        }
    }
}
