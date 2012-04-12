// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Net.Protocols.Frostbite.BF.BF3 {
    using Procon.Net.Protocols.Frostbite.BF.BF3.Objects;
    using Procon.Net.Attributes;
    using Procon.Net.Protocols.Objects;
    using Procon.Net.Protocols.Frostbite.Objects;

    [Game(GameType = GameType.BF_3)]
    public class BF3Game : BFGame {

        public BF3Game(string hostName, ushort port)
            : base(hostName, port) {

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

        }

        [DispatchPacket(MatchText = "admin.listPlayers")]
        public override void AdminListPlayersResponseDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            BF3PlayerList players = new BF3PlayerList() {
                Subset = new FrostbitePlayerSubset().Parse(request.Words.GetRange(1, request.Words.Count - 1))
            }.Parse(response.Words.GetRange(1, response.Words.Count - 1));

            this.AdminListPlayersFinalize(players);
        }

        [DispatchPacket(MatchText = "mapList.list")]
        public override void MapListListDispatchHandler(FrostbitePacket request, FrostbitePacket response) {
            if (request.Words.Count >= 1) {

                FrostbiteMapList maps = new BF3FrostbiteMapList().Parse(response.Words.GetRange(1, response.Words.Count - 1));

                Map mapInfo = null;
                foreach (Map map in maps) {
                    if ((mapInfo = this.State.MapPool.Find(x => String.Compare(x.Name, map.Name, true) == 0)) != null) {
                        map.FriendlyName = mapInfo.FriendlyName;
                        map.GameMode = mapInfo.GameMode;
                    }
                }
                this.State.MapList = maps;

                this.ThrowGameEvent(
                    GameEventType.MaplistUpdated
                );
            }
        }

        protected override void Action(Chat chat) {
            if (chat.Subset != null) {
                string subset = String.Empty;

                if (chat.Subset.Context == PlayerSubsetContext.All) {
                    subset = "all";
                }
                else if (chat.Subset.Context == PlayerSubsetContext.Player && chat.Subset.Player != null) {
                    if (chat.Subset.Squad != Squad.None) {
                        subset = String.Format("squad {0} {1}", FrostbiteConverter.TeamToTeamId(chat.Subset.Team), FrostbiteConverter.SquadToSquadId(chat.Subset.Squad));
                    }
                    else if (chat.Subset.Team != Team.None) {
                        subset = String.Format("team {0}", FrostbiteConverter.TeamToTeamId(chat.Subset.Team));
                    }
                    else {
                        subset = "all";
                    }
                }
                else if (chat.Subset.Context == PlayerSubsetContext.Team) {
                    subset = String.Format("team {0}", FrostbiteConverter.TeamToTeamId(chat.Subset.Team));
                }
                else if (chat.Subset.Context == PlayerSubsetContext.Squad) {
                    subset = String.Format("squad {0} {1}", FrostbiteConverter.TeamToTeamId(chat.Subset.Team), FrostbiteConverter.SquadToSquadId(chat.Subset.Squad));
                }

                if (chat.ChatActionType == ChatActionType.Say) {
                    this.Send(this.Create("admin.say \"{0}\" {1}", chat.Text, subset));
                }
                else if (chat.ChatActionType == ChatActionType.Yell || chat.ChatActionType == ChatActionType.YellOnly) {
                    this.Send(this.Create("admin.yell \"{0}\" 8000 {1}", chat.Text, subset));
                }
            }
        }

        protected override void SendEventsEnabledPacket() {
            this.Send(this.Create("admin.eventsEnabled true"));
        }

    }
}
