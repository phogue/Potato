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

namespace Potato.Net.Shared.Sandbox {
    /// <summary>
    /// Callback for to be initialized in the host domain, set and passed into
    /// the sandboxed domain allowing for events to be pushed through.
    /// </summary>
    public sealed class SandboxProtocolCallbackProxy : MarshalByRefObject, ISandboxProtocolCallbackProxy {
        /// <summary>
        /// Called when ever a dispatched game event occurs.
        /// </summary>
        public Action<IProtocolEventArgs> ProtocolEvent { get; set; }

        /// <summary>
        /// Called when something occurs with the underlying client. This can
        /// be connections, disconnections, logins or raw packets being recieved.
        /// </summary>
        public Action<IClientEventArgs> ClientEvent { get; set; }

        public override object InitializeLifetimeService() {
            return null;
        }

        public void FireProtocolEvent(IProtocolEventArgs args) {
            if (this.ProtocolEvent != null) {
                this.ProtocolEvent(args);
            }
        }

        public void FireClientEvent(IClientEventArgs args) {
            if (this.ClientEvent != null) {
                this.ClientEvent(args);
            }
        }
    }
}
