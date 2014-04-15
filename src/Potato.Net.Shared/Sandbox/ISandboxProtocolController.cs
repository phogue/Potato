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
    /// Remoting interface for Potato.Core to communicate with remote Plugin.
    /// </summary>
    public interface ISandboxProtocolController : IProtocolShared {
        /// <summary>
        /// Object to bubble events to
        /// </summary>
        ISandboxProtocolCallbackProxy Bubble { get; set; }

        /// <summary>
        /// Loads a protocol assembly, loads a new IProtocol instance with the setup provided.
        /// </summary>
        /// <param name="assemblyFile"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        bool Create(String assemblyFile, IProtocolType type);
    }
}
