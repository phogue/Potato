using System;
using Procon.Net.Shared.Models;

namespace Myrcon.Protocols.Frostbite.Objects {
    [Serializable]
    public class TeamScore : NetworkModel {

        public int TeamID { get; set; }

        public int Score { get; set; }
    }
}
