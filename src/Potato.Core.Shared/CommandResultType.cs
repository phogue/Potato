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

namespace Potato.Core.Shared {
    /// <summary>
    /// A status type as a result of the command.
    /// </summary>
    [Serializable]
    public enum CommandResultType {
        /// <summary>
        /// Command never got run?
        /// </summary>
        None,
        /// <summary>
        /// Nothing was found or no errors, cancels, etc have occured. The command should continue to execute.
        /// </summary>
        Continue,
        /// <summary>
        /// The command should be canceled with no further previews/executions.
        /// </summary>
        Cancel,
        /// <summary>
        /// The command has failed
        /// </summary>
        Failed,
        /// <summary>
        /// The command is successful
        /// </summary>
        Success,
        /// <summary>
        /// An invalid parameter was passed into the command.
        /// </summary>
        InvalidParameter,
        /// <summary>
        /// A required parameter was invalid because a value already exists
        /// </summary>
        AlreadyExists,
        /// <summary>
        /// A required parameter was invalid because the object does not exists
        /// </summary>
        DoesNotExists,
        /// <summary>
        /// The command was executed but the user had insufficient permissions to follow through with the command.
        /// </summary>
        InsufficientPermissions,
        /// <summary>
        /// The user has a minimum amount to execute a command, but lacks the authority to execute it on the target.
        /// </summary>
        InsufficientAuthority,
        /// <summary>
        /// A limit has been exceeded when attempting to execute the command.
        /// </summary>
        LimitExceeded,
        /// <summary>
        /// The command is essentially canceled, but is continuing asynchronously.
        /// </summary>
        ContinuedAsynchronously,
        /// <summary>
        /// The command has times out while executing asynchronously
        /// </summary>
        TimeoutAsynchronously,
    }
}
