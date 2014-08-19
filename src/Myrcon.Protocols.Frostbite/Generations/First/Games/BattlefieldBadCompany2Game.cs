#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using Potato.Net.Shared;
using Potato.Net.Shared.Protocols;

namespace Myrcon.Protocols.Frostbite.Generations.First.Games {
    [ProtocolDeclaration(Type = CommonProtocolType.DiceBattlefieldBadCompany2, Name = "Battlefield: Bad Company 2", Provider = "Myrcon")]
    public class BattlefieldBadCompany2Game : FirstGame {

        public BattlefieldBadCompany2Game() : base() {
            ServerInfoParameters = new List<String>() {
                "ServerName",       "PlayerCount",   "MaxPlayerCount",   "GameMode",
                "Map",              "CurrentRound",  "TotalRounds",      "TeamScores",
                "ConnectionState",  "Ranked",        "PunkBuster",       "Passworded",
                "ServerUptime",     "RoundTime",     "GameMod",          "Mappack"
            };
        }
    }
}
