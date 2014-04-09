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

namespace Procon.Core.Shared {
    /// <summary>
    /// I'd be tempted to refactor GenericEventArgs so I can seal this class. It's
    /// used as a backbone for xml serialization so any inherited classes could
    /// cause the xml serializer to encounter a type it didn't expect.
    /// </summary>
    public interface ICommandResult {
        /// <summary>
        /// Text based version of CommandResultType, but allows for custom result types from
        /// custom commands within plugins.
        /// </summary>
        String Name { get; set; }

        /// <summary>
        /// A general text message used to describe the event in more detail, if required.
        /// </summary>
        String Message { get; set; }

        /// <summary>
        /// When the event occured. Defaults to the current date/time.
        /// </summary>
        DateTime Stamp { get; set; }

        /// <summary>
        /// The limiting scope of the event (the connection, player etc. that this event is limited to)
        /// </summary>
        ICommandData Scope { get; set; }

        /// <summary>
        /// Data that this used to be, like an account being moved from one group to another
        /// this would be the original group.
        /// </summary>
        ICommandData Then { get; set; }

        /// <summary>
        /// Data as it is seen "now"
        /// </summary>
        ICommandData Now { get; set; }

        /// <summary>
        /// Simple flag determining the success of the command being executed.
        /// </summary>
        Boolean Success { get; set; }

        /// <summary>
        /// A more detailed status describing the command execution.
        /// </summary>
        CommandResultType CommandResultType { get; set; }

        /// <summary>
        /// How the output of the command should be handled if it is a remote request.
        /// Defaults to application/xml where the entire result is output
        /// </summary>
        String ContentType { get; set; }

        /// <summary>
        /// Called when the object is being disposed.
        /// </summary>
        event EventHandler Disposed;
    }
}
