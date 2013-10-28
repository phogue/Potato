using System.Text.RegularExpressions;
using Procon.Net.Protocols.Objects;

namespace Procon.Net.Protocols.Source.Objects {

    public interface ISourceObject {

        NetworkObject Parse(Match match);

    }
}
