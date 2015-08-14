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

namespace Potato.Net.Shared {
    /// <summary>
    /// The basic interface of communication between Core and Net
    /// </summary>
    public interface IProtocol : IProtocolShared {
        /// <summary>
        /// Fired when ever a dispatched game event occurs.
        /// </summary>
        event Action<IProtocol, IProtocolEventArgs> ProtocolEvent;

        /// <summary>
        /// Fired when something occurs with the underlying client. This can
        /// be connections, disconnections, logins or raw packets being recieved.
        /// </summary>
        event Action<IProtocol, IClientEventArgs> ClientEvent;
    }
}
