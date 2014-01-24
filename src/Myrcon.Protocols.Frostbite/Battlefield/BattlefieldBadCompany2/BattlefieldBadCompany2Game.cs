using System;
using System.Collections.Generic;
using Procon.Net;
using Procon.Net.Shared.Protocols;

namespace Myrcon.Protocols.Frostbite.Battlefield.BattlefieldBadCompany2 {
    [ProtocolDeclaration(Type = CommonGameType.DiceBattlefieldBadCompany2, Name = "Battlefield: Bad Company 2", Provider = "Myrcon")]
    public class BFBC2Game : BattlefieldGame {

        public BFBC2Game() : base() {
            ServerInfoParameters = new List<String>() {
                "ServerName",       "PlayerCount",   "MaxPlayerCount",   "GameMode",
                "Map",              "CurrentRound",  "TotalRounds",      "TeamScores",
                "ConnectionState",  "Ranked",        "PunkBuster",       "Passworded",
                "ServerUptime",     "RoundTime",     "GameMod",          "Mappack"
            };
        }
    }
}
