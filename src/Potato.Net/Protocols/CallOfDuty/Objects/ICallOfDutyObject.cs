using System.Text.RegularExpressions;
using Potato.Net.Protocols.Objects;

namespace Potato.Net.Protocols.CallOfDuty.Objects {

    public interface ICallOfDutyObject {

        NetworkObject Parse(Match match);

    }
}
