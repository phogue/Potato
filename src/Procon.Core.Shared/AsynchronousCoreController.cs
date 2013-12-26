using System;
using System.Threading;
using System.Threading.Tasks;
using Procon.Core.Shared.Models;

namespace Procon.Core.Shared {
    /// <summary>
    /// Dispatches commands asynchronously and later accepts results, also asynchronously.
    /// </summary>
    /// <remarks>
    ///     <para>Pay close attention to the distinction made when we just execute a command and
    /// later capture the executed result as it propogates. This allows any class, wherever it is, to just
    /// pass on commands for execution while also allowing for capture of asynchronous results.</para>
    /// <para>This is used over the AppDomain. We want the plugin controller to dispatch, we then
    /// want the sandbox controller to dispatch in it's own thread running in Procon's full-trust
    /// but we want both the Sandbox controller and the Plugin controller to fire when a result is encountered.</para>
    /// </remarks>
    public abstract class AsynchronousCoreController : CoreController {
        /// <summary>
        /// The state of this controller in model form.
        /// </summary>
        public AsynchronousCommandStateModel AsyncStateModel { get; set; }

        /// <summary>
        /// Initializes the controller with the default values
        /// </summary>
        protected AsynchronousCoreController() : base() {
            this.AsyncStateModel = new AsynchronousCommandStateModel();
        }

        public override ICoreController Execute() {
            // Start the long running command dispatcher.
            Task.Factory.StartNew(ExecuteQueuedCommands, this.AsyncStateModel.TaskQueueWaitCancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            return base.Execute();
        }

        /// <summary>
        /// Long running loop to process commands when signaled that a new command(s) has been
        /// added to the queue.
        /// </summary>
        public virtual void ExecuteQueuedCommands() {
            while (this.AsyncStateModel.TaskQueueWaitCancellationTokenSource.IsCancellationRequested == false) {

                // Wait until we are poked that something has been added to the queue.
                this.AsyncStateModel.TaskQueueWait.WaitOne();

                // Dequeue and process a new command.
                AsynchronousCommandModel asynchronousCommand = null;

                if (this.AsyncStateModel.PendingCommandsQueue.TryDequeue(out asynchronousCommand) == true) {
                    // Add it so we can look for a reference when it comes back.
                    this.AsyncStateModel.ExecutedCommandsPool.TryAdd(asynchronousCommand.Command.CommandGuid, asynchronousCommand);

                    Task.Factory.StartNew(() => {
                        if (asynchronousCommand.IsTunneling == true) {
                            base.Tunnel(asynchronousCommand.Command);
                        }
                        else {
                            base.Bubble(asynchronousCommand.Command);
                        }
                    });
                }
            }

            // Cleanup
            this.AsyncStateModel.TaskQueueWaitCancellationTokenSource.Dispose();
            this.AsyncStateModel.TaskQueueWait.Dispose();

            this.AsyncStateModel.ExecutedCommandsPool.Clear();

            this.AsyncStateModel = null;
        }

        /// <summary>
        /// Appends a tunnelling command to the queue, alerts the command process thread then
        /// returns execution to the caller.
        /// </summary>
        /// <param name="command">The command to enqueue</param>
        /// <param name="completed">The optional callback when a result is available.</param>
        public virtual void BeginTunnel(Command command, Action<CommandResultArgs> completed = null) {
            if (this.AsyncStateModel != null && this.AsyncStateModel.TaskQueueWaitCancellationTokenSource.IsCancellationRequested == false) {
                this.AsyncStateModel.PendingCommandsQueue.Enqueue(new AsynchronousCommandModel() {
                    Command = command,
                    IsTunneling = true,
                    Completed = completed
                });

                this.AsyncStateModel.TaskQueueWait.Set();
            }
        }

        /// <summary>
        /// Appends a bubbling command to the queue, alerts the command process thread then
        /// returns execution to the caller.
        /// </summary>
        /// <param name="command">The command to enqueue</param>
        /// <param name="completed">The optional callback when a result is available.</param>
        public virtual void BeginBubble(Command command, Action<CommandResultArgs> completed = null) {
            if (this.AsyncStateModel != null && this.AsyncStateModel.TaskQueueWaitCancellationTokenSource.IsCancellationRequested == false) {
                this.AsyncStateModel.PendingCommandsQueue.Enqueue(new AsynchronousCommandModel() {
                    Command = command,
                    IsTunneling = false,
                    Completed = completed
                });

                this.AsyncStateModel.TaskQueueWait.Set();
            }
        }

        /// <summary>
        /// Executes the command synchronously on this end, blocking until a result has been returned.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public override CommandResultArgs Tunnel(Command command) {
            CommandResultArgs synchronousResult = command.Result;
            AutoResetEvent resultWait = new AutoResetEvent(false);

            this.BeginTunnel(command, result => {
                synchronousResult = result;

                resultWait.Set();
            });

            resultWait.WaitOne();

            return synchronousResult;
        }

        /// <summary>
        /// Executes the command synchronously on this end, blocking until a result has been returned.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public override CommandResultArgs Bubble(Command command) {
            CommandResultArgs synchronousResult = command.Result;
            AutoResetEvent resultWait = new AutoResetEvent(false);

            this.BeginBubble(command, result => {
                synchronousResult = result;

                resultWait.Set();
            });

            resultWait.WaitOne();

            return synchronousResult;
        }

        // Captures all executed commands, seeing if this async conroller is waiting for a command to have been executed.
        public override CommandResultArgs PropogateExecuted(Command command, bool tunnel = true) {
            AsynchronousCommandModel asynchronousCommandModel = null;

            // If we are waiting for it and we have removed the executed wrapper successfully.
            if (this.AsyncStateModel.ExecutedCommandsPool.TryRemove(command.CommandGuid, out asynchronousCommandModel) == true) {
                Task.Factory.StartNew(() => asynchronousCommandModel.OnResult(command.Result));
            }

            return base.PropogateExecuted(command, tunnel);
        }

        public override void Dispose() {
            base.Dispose();

            // This should end the pendingcommandpool -> commandexecutedpool thread
            this.AsyncStateModel.TaskQueueWaitCancellationTokenSource.Cancel();
            this.AsyncStateModel.TaskQueueWait.Set();
        }
    }
}
