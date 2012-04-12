using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Procon.Net.Protocols.Source.Objects {
    using Procon.Net.Protocols.Objects;
    public class SourceChat : Chat, ISourceObject {

        public ISourceObject Parse(Match match) {
            this.Author = new Player() {
                Name = match.Groups["name"].Value,
                GUID = match.Groups["uniqueid"].Value
            };

            if (String.Compare(match.Groups["context"].Value, "say", true) == 0) {
                this.Subset = new PlayerSubset() {
                    Context = PlayerSubsetContext.All
                };
            }
            else {
                this.Subset = new PlayerSubset() {
                    Context = PlayerSubsetContext.Team
                };
            }

            this.Text = match.Groups["text"].Value.Replace("\r", "");

            this.Origin = ChatOrigin.Player;

            return this;
        }

        public SourceChat ParseConsoleSay(List<string> words) {
            this.Origin = ChatOrigin.Reflected;
            this.Author = new Player() { Name = "Procon" };

            this.Text = String.Join(" ", words.Skip(1).ToArray());

            return this;
        }
    }
}
