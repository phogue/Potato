using System.Text.RegularExpressions;
using Procon.Net.Protocols.Objects;

namespace Procon.Net.Protocols.CallOfDuty.Objects {

    public interface ICallOfDutyObject {

        NetworkObject Parse(Match match);

    }
}
