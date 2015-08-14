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
using Potato.Core.Shared;
using Potato.Core.Shared.Models;
using Potato.Net.Shared;

namespace Potato.Core.Connections {
    /// <summary>
    /// Handles connections, plugins and text commands for a single game server.
    /// </summary>
    public interface IConnectionController : ICoreController {
        /// <summary>
        /// Data about the protocol connection
        /// </summary>
        ConnectionModel ConnectionModel { get; set; }

        /// <summary>
        /// Fired when a protocol event is recieved from the protocol appdomain.
        /// </summary>
        event Action<IProtocolEventArgs> ProtocolEvent;

        /// <summary>
        /// Fired when a client event is recieved from the protocol appdomain.
        /// </summary>
        event Action<IClientEventArgs> ClientEvent;

        /// <summary>
        /// Proxy to the active protocol state
        /// </summary>
        IProtocolState ProtocolState { get; }
    }
}
