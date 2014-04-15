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

namespace Potato.Core.Shared.Events {
    /// <summary>
    /// Defines a consistent structure that events should be logged or serialized with.
    /// </summary>
    /// <remarks>
    ///     <pre>This is done in a very non-clever way deliberately. Think of it as an Interface that should never change, but instead be added to.</pre>
    /// </remarks>
    public interface IGenericEvent : ICommandResult, IDisposable {
        /// <summary>
        /// The event ID for this execution. These event ids are volatile, only used to track
        /// during the current execution.
        /// </summary>
        ulong Id { get; set; }

        /// <summary>
        /// The command to be executed, will be converted to a string in Name
        /// </summary>
        GenericEventType GenericEventType { get; set; }
    }
}
