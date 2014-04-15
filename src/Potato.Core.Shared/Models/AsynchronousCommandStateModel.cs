#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace Potato.Core.Shared.Models {
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
        /// Initalizes with the default values.
        /// </summary>
        public AsynchronousCommandStateModel() : base() {
            this.PendingCommandsQueue = new ConcurrentQueue<AsynchronousCommandModel>();
            this.ExecutedCommandsPool = new ConcurrentDictionary<Guid, AsynchronousCommandModel>();
            this.TaskQueueWaitCancellationTokenSource = new CancellationTokenSource();
            this.TaskQueueWait = new AutoResetEvent(false);
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
