using System.Text.RegularExpressions;

namespace Procon.Net.Protocols.Source.Objects {
    using Procon.Net.Protocols.Objects;
    public class SourceKick : ISourceObject {

        public NetworkObject Parse(Match match) {
            Kick kick = new Kick {
                Target = new Player() {
                    Name = match.Groups["name"].Value,
                    Uid = match.Groups["uniqueid"].Value
                },
                Reason = match.Groups["message"].Value
            };

            return kick;
        }
    }
}
