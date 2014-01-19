using System;
using Procon.Net.Shared.Protocols;

namespace Procon.Core.Shared.Models {
    /// <summary>
    /// Holds parameters required to authenticate a command
    /// </summary>
    [Serializable]
    public class CommandAuthenticationModel {
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
            this.GameType = CommonGameType.None;
            this.Uid = null;
            this.PasswordPlainText = null;
        }
    }
}
