using System;
using Procon.Net.Data;

namespace Procon.Net.Protocols.Frostbite.Objects {
    [Serializable]
    public class TeamScore : NetworkObject {

        public int TeamID { get; set; }

        public int Score { get; set; }
    }
}
