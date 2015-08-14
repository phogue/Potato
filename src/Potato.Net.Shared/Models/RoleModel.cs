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

namespace Potato.Net.Shared.Models {
    /// <summary>
    /// A role, describing a player
    /// </summary>
    [Serializable]
    public sealed class RoleModel : NetworkModel {
        /// <summary>
        /// The name of the role the player has taken
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Initializes the role with empty values.
        /// </summary>
        public RoleModel() : base() {
            Name = string.Empty;
        }
    }
}
