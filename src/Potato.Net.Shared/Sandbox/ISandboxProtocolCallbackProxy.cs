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
    public interface ISandboxProtocolCallbackProxy {
        /// <summary>
        /// Fires a protocol event back across the appdomain
        /// </summary>
        void FireProtocolEvent(IProtocolEventArgs args);

        /// <summary>
        /// Fires a client event back across the appdomain
        /// </summary>
        void FireClientEvent(IClientEventArgs args);
    }
}
