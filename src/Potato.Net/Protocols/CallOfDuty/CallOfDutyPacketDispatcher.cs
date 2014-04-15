using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Potato.Net.Protocols.CallOfDuty {
    public class CallOfDutyPacketDispatcher : PacketDispatcher {

        private static readonly Dictionary<Regex, string> PacketTypes = new Dictionary<Regex, string>() {
            {
                new Regex(@"^map: (?<map>.*?)num[ ]+score[ ]+ping[ ]+Uid[ ]+name[ ]+team[ ]+lastmsg[ ]+address[ ]+qport[ ]+rate", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline), "teamstatus"
            },
            {
                new Regex(@"^Server info settings:", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline), "serverinfo"
            }
        };

        public override void Dispatch(Packet packet) {
            Match match = null;
            foreach (KeyValuePair<Regex, string> packetType in CallOfDutyPacketDispatcher.PacketTypes) {
                if ((match = packetType.Key.Match(((CallOfDutyPacket)packet).Message)).Success == true) {
                    this.Dispatch(new PacketDispatch() {
                        Name = packetType.Value
                    }, null, packet);
                }
            }
        }
    }
}
