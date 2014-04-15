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
using Potato.Net.Shared.Models;

namespace Potato.Net.Shared.Actions {
    /// <summary>
    /// An action for the network layer to execute
    /// </summary>
    public interface INetworkAction : INetworkModel {
        /// <summary>
        /// The name of the action to take.
        /// </summary>
        String Name { get; set; }

        /// <summary>
        /// The specific type of action taken with this object.
        /// </summary>
        NetworkActionType ActionType { get; set; }

        /// <summary>
        /// A unique id generated for this particular action
        /// </summary>
        Guid Uid { get; set; }
    }
}
