using System;
using System.Text.RegularExpressions;

namespace Procon.Net.Protocols.PunkBuster.Packets {
    public class PunkBusterPlayer : IPunkBuster {

        /// <summary>
        /// Slot Id of the player.
        /// </summary>
        public uint SlotId { get; set; }

        /// <summary>
        /// The players IP
        /// </summary>
        public String Ip { get; set; }

        /// <summary>
        /// The players name
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The players PunkBuster Guid
        /// </summary>
        public String Guid { get; set; }

        /// <summary>
        /// Deserialize a regular expression match object into the the object.
        /// </summary>
        /// <param name="data"></param>
        public void Deserialize(Match data) {
            this.SlotId = uint.Parse(data.Groups["SlotId"].Value);
            this.Ip = data.Groups["Ip"].Value;
            this.Name = data.Groups["Name"].Value;
            this.Guid = data.Groups["Guid"].Value;
        }
    }
}
