using System;
using Procon.Net.Models;

namespace Procon.Net.Protocols.Myrcon.Frostbite.Objects {
    [Serializable]
    public class TeamScore : NetworkObject {

        public int TeamID { get; set; }

        public int Score { get; set; }
    }
}
