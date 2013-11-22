using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Procon.Net.Protocols.Source.Objects {
    using Procon.Net.Protocols.Objects;
    public class SourceKick : ISourceObject {

        public NetworkObject Parse(Match match) {
            return new Kick {
                Now = {
                    Players = new List<Player>() {
                        new Player() {
                            Name = match.Groups["name"].Value,
                            Uid = match.Groups["uniqueid"].Value
                        }
                    },
                    Content = new List<String>() {
                        match.Groups["message"].Value
                    }
                }
            };
        }
    }
}
