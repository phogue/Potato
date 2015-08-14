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
using System.Collections.Generic;
using Potato.Net.Shared.Actions;

namespace Potato.Net.Shared {
    /// <summary>
    /// Data attached to a client event
    /// </summary>
    public interface IClientEventData {
        /// <summary>
        /// List of exceptions attached to this event, if any.
        /// </summary>
        List<string> Exceptions { get; set; }

        /// <summary>
        /// List of packets attached to this event, if any.
        /// </summary>
        List<IPacket> Packets { get; set; }

        /// <summary>
        /// List of actions attached to this event, if any.
        /// </summary>
        List<INetworkAction> Actions { get; set; } 
    }
}
