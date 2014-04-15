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
using Potato.Net.Shared.Protocols;

namespace Potato.Core.Shared.Models {
    /// <summary>
    /// Holds parameters required to authenticate a command
    /// </summary>
    [Serializable]
    public class CommandAuthenticationModel : CoreModel {
        /// <summary>
        /// The username of the initiator
        /// </summary>
        public String Username { get; set; }

        /// <summary>
        /// The password of the user executing the command. Used to authenticate
        /// remote requests.
        /// </summary>
        /// <remarks>Will change to much more secure password authentication</remarks>
        public String PasswordPlainText { get; set; }

        /// <summary>
        /// The id of the supplied access token.
        /// </summary>
        public Guid TokenId { get; set; }

        /// <summary>
        /// The token, as a replacement for the password.
        /// </summary>
        public String Token { get; set; }

        /// <summary>
        /// The game type of the initiators player Uid
        /// </summary>
        public String GameType { get; set; }

        /// <summary>
        /// The uid of the player initiating the command
        /// </summary>
        public String Uid { get; set; }

        /// <summary>
        /// Initializes the authentication model with the default values.
        /// </summary>
        public CommandAuthenticationModel() {
            this.Username = null;
            this.GameType = CommonProtocolType.None;
            this.Uid = null;
            this.PasswordPlainText = null;
        }
    }
}
