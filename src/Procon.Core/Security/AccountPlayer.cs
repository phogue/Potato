using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Procon.Core.Security {
    using Procon.Net.Protocols;

    [Serializable]
    public class AccountPlayer : IDisposable, ICloneable {

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
        [XmlIgnore, JsonIgnore]
        public Account Account { get; set; }

        public AccountPlayer() {
            this.GameType = Net.Protocols.CommonGameType.None;

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
