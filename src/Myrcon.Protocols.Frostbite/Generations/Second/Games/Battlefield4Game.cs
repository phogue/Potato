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

namespace Myrcon.Protocols.Frostbite.Generations.Second.Games {
    /// <summary>
    /// Protocol implementation for Battlefield 4
    /// </summary>
    [ProtocolDeclaration(Type = CommonProtocolType.DiceBattlefield4, Name = "Battlefield 4", Provider = "Myrcon")]
    public class Battlefield4Game : SecondGame {

        /// <summary>
        /// Game constructor to initalize the server info and any other dispatch methods
        /// </summary>
        public Battlefield4Game() : base() {

            this.ServerInfoParameters = new List<string>() {
                "ServerName",
                "PlayerCount",
                "MaxPlayerCount",
                "GameMode",
                "Map",
                "CurrentRound",
                "TotalRounds",
                "TeamScores",
                "ConnectionState",
                "Ranked",
                "PunkBuster",
                "Passworded",
                "ServerUpTime",
                "RoundTime"
            };

            this.PacketDispatcher.Append(new Dictionary<IPacketDispatch, Action<IPacketWrapper, IPacketWrapper>>() {
                {
                    new PacketDispatch() {
                        Name = "server.onLevelLoaded",
                        Origin = PacketOrigin.Server
                    },
                    new Action<IPacketWrapper, IPacketWrapper>(this.ServerOnLevelLoadedDispatchHandler)
                }
            });
        }
    }
}
