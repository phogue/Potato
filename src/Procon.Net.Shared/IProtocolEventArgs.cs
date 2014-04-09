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
using Procon.Net.Shared.Models;

namespace Procon.Net.Shared {
    /// <summary>
    /// Even though a majority of the data inherits from ProtocolObject, we still have
    /// these as seperate fields for serialization.
    /// </summary>
    public interface IProtocolEventArgs {
        /// <summary>
        /// Stores the type of event (PlayerJoin, PlayerLeave etc)
        /// </summary>
        ProtocolEventType ProtocolEventType { get; set; }

        /// <summary>
        /// Stores everything about the game that we know like
        /// the current playerlist, all the server info etc.
        /// </summary>
        IProtocolState ProtocolState { get; set; }

        /// <summary>
        /// The game type itself (BlackOps, BFBC2)
        /// </summary>
        IProtocolType ProtocolType { get; set; }

        /// <summary>
        /// Data describing the effected data before the event occured.
        /// </summary>
        IProtocolEventData Then { get; set; }

        /// <summary>
        /// Data describing how the data looked after the event.
        /// </summary>
        IProtocolEventData Now { get; set; }

        /// <summary>
        /// When this event occured.
        /// </summary>
        DateTime Stamp { get; set; }
    }
}
