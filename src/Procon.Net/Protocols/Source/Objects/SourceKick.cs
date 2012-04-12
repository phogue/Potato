using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Procon.Net.Protocols.Source.Objects {
    using Procon.Net.Protocols.Objects;
    public class SourceKick : Kick, ISourceObject {

        public ISourceObject Parse(Match match) {

            this.Target = new Player() {
                Name = match.Groups["name"].Value,
                GUID = match.Groups["uniqueid"].Value
            };

            this.Reason = match.Groups["message"].Value;

            return this;
        }
    }
}
