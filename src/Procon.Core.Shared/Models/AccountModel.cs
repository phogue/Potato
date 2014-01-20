using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Procon.Core.Shared.Models {
    /// <summary>
    /// A single account attached to a group
    /// </summary>
    [Serializable]
    public class AccountModel : CoreModel, IDisposable {
        /// <summary>
        /// Username for this account. Must be unique to the security controller.
        /// </summary>
        public String Username { get; set; }

        /// <summary>
        /// The IETF Language code that this user preferres to use.
        /// </summary>
        public String PreferredLanguageCode { get; set; }

        /// <summary>
        /// The hashed password, salted with PasswordSalt
        /// </summary>
        /// <remarks>
        ///     <para>We ignore the password hash, so the hash is never sent across the network or logged.</para>
        /// </remarks>
        [JsonIgnore]
        public String PasswordHash { get; set; }

        /// <summary>
        /// All of the assigned players to this account.
        /// </summary>
        public List<AccountPlayerModel> Players { get; set; }

        /// <summary>
        /// Backreference to the group that owns this account.
        /// </summary>
        [JsonIgnore]
        public GroupModel Group { get; set; }

        /// <summary>
        /// Initializes the account with default values.
        /// </summary>
        public AccountModel()
            : base() {
            this.Username = String.Empty;
            this.PasswordHash = String.Empty;
            this.PreferredLanguageCode = String.Empty;
            this.Players = new List<AccountPlayerModel>();
        }

        public  void Dispose() {
            foreach (AccountPlayerModel accountPlayer in this.Players) {
                accountPlayer.Dispose();
            }

            this.Players.Clear();
            this.Players = null;

            this.Username = null;
            this.PreferredLanguageCode = null;
            this.PasswordHash = null;
            this.Group = null;
        }
    }
}
