using System.Text.RegularExpressions;

namespace Procon.Net.Protocols.PunkBuster.Objects {
    public class PunkBusterEndPlayerList : IPunkBuster {

        /// <summary>
        /// The total number of players at the end of the player list.
        /// </summary>
        public int PlayerCount { get; set; }

        /// <summary>
        /// Deserialize a regular expression match object into the the object.
        /// </summary>
        /// <param name="data"></param>
        public void Deserialize(Match data) {
            this.PlayerCount = int.Parse(data.Groups["PlayerCount"].Value);
        }
    }
}
