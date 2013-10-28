using System;
using System.Collections.Generic;

namespace Procon.Net.Protocols.Frostbite.BF.BFBC2 {
    using Procon.Net.Attributes;

    [GameType(Type = CommonGameType.BF_BC2, Name = "Battlefield: Bad Company 2", Provider = "Myrcon")]
    public class BFBC2Game : BFGame {

        public BFBC2Game(string hostName, ushort port) : base(hostName, port) {
            ServerInfoParameters = new List<String>() {
                "ServerName",       "PlayerCount",   "MaxPlayerCount",   "GameMode",
                "Map",              "CurrentRound",  "TotalRounds",      "TeamScores",
                "ConnectionState",  "Ranked",        "PunkBuster",       "Passworded",
                "ServerUptime",     "RoundTime",     "GameMod",          "Mappack"
            };
        }


    }
}
