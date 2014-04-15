using System.Text.RegularExpressions;
using Potato.Net.Protocols.Objects;

namespace Potato.Net.Protocols.Source.Objects {

    public interface ISourceObject {

        NetworkObject Parse(Match match);

    }
}
