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
    /// A simple predicate to determine if the packet matches criteria to dispatch
    /// </summary>
    public interface IPacketDispatch {
        /// <summary>
        /// The name of the packet we should check against
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Where the packet originated from (initiated from client, recieved response or initiated by server, expecting response)
        /// </summary>
        PacketOrigin Origin { get; set; }
    }
}
