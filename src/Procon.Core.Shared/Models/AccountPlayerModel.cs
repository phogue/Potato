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
using Newtonsoft.Json;
using Procon.Net.Shared.Protocols;

namespace Procon.Core.Shared.Models {
    /// <summary>
    /// A player attached to a security account
    /// </summary>
    [Serializable]
    public class AccountPlayerModel : IDisposable, ICloneable {
        /// <summary>
        /// The name of the game 
        /// </summary>
        public String ProtocolType { get; set; }

        /// <summary>
        /// Unique identifer value for the player.  In Frostbite based
        /// games this will be the players name.  In Call Of Duty games this
        /// will be the players GUID number.
        /// </summary>
        public String Uid { get; set; }

        /// <summary>
        /// Backreference to the owner of this assignment.
        /// </summary>
        [JsonIgnore]
        public AccountModel Account { get; set; }

        /// <summary>
        /// Initializes the account player with default values.
        /// </summary>
        public AccountPlayerModel() {
            this.ProtocolType = CommonProtocolType.None;

            this.Uid = String.Empty;
        }

        public void Dispose() {
            this.ProtocolType = CommonProtocolType.None;
            this.Uid = null;
            this.Account = null;
        }

        public object Clone() {
            return this.MemberwiseClone();
        }
    }
}
