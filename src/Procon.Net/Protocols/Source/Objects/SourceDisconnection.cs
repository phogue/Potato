using System.Text.RegularExpressions;

namespace Procon.Net.Protocols.Source.Objects {
    using Procon.Net.Protocols.Objects;
    public class SourceDisconnection : NetworkObject, ISourceObject {

        public Player Player { get; set; }

        public NetworkObject Parse(Match match) {
            this.Player = new Player() {
                Name = match.Groups["name"].Value,
                Uid = match.Groups["uniqueid"].Value
            };

            return this;
        }
    }
}
