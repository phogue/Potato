using System;
using Newtonsoft.Json;
using Procon.Net.Shared.Protocols;

namespace Procon.Core.Shared.Models {
    [Serializable]
    public class AccountPlayerModel : IDisposable, ICloneable {
        /// <summary>
        /// The name of the game 
        /// </summary>
        public String GameType { get; set; }

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

        public AccountPlayerModel() {
            this.GameType = CommonGameType.None;

            this.Uid = String.Empty;
        }

        public void Dispose() {
            this.GameType = CommonGameType.None;
            this.Uid = null;
            this.Account = null;
        }

        public object Clone() {
            return this.MemberwiseClone();
        }
    }
}
