#region Copyright
// Copyright 2015 Geoff Green.
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

namespace Potato.Core.Shared.Models {
    /// <summary>
    /// Stores a command to be sent asynchronously as well as the callback (if supplied)
    /// once the command has completed.
    /// </summary>
    [Serializable]
    public class AsynchronousCommandModel : CoreModel {
        /// <summary>
        /// The command being passed around
        /// </summary>
        public ICommand Command { get; set; }

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
