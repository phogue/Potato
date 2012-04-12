using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Procon.Net.Protocols.Source.Objects {
    using Procon.Net.Protocols.Objects;
    public class SourceDisconnection : ISourceObject {

        public Player Player { get; set; }

        public ISourceObject Parse(Match match) {
            this.Player = new Player() {
                Name = match.Groups["name"].Value,
                GUID = match.Groups["uniqueid"].Value
            };

            return this;
        }
    }
}
