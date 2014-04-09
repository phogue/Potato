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

namespace Procon.Net.Shared {
    /// <summary>
    /// An event originating from the networking side of the protocol implementation.
    /// </summary>
    public interface IClientEventArgs {
        /// <summary>
        /// Stores the type of event (ConnectionStateChanged, PacketSent etc)
        /// </summary>
        ClientEventType EventType { get; set; }

        /// <summary>
        /// The state of the connection (Connected/Disconnected/LoggedIn)
        /// </summary>
        ConnectionState ConnectionState { get; set; }

        /// <summary>
        /// When the event occured. Defaults to the current date/time.
        /// </summary>
        DateTime Stamp { get; set; }

        /// <summary>
        /// Data describing the effected data before the event occured.
        /// </summary>
        IClientEventData Then { get; set; }

        /// <summary>
        /// Data describing how the data looked after the event.
        /// </summary>
        IClientEventData Now { get; set; }
    }
}
