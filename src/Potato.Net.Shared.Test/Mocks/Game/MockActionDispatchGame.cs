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

using System.Collections.Generic;
using Potato.Net.Shared.Actions;

namespace Potato.Net.Shared.Test.Mocks.Game {

    /// <summary>
    /// Used to simply ensure that actions are dispatched/converted etc. correctly.
    /// </summary>
    public class MockActionDispatchGame : MockGame {
        protected override List<IPacketWrapper> DispatchAction(INetworkAction action) {
            List<IPacketWrapper> wrappers = base.DispatchAction(action);

            switch (action.ActionType) {
                case NetworkActionType.NetworkTextSay:
                    wrappers.Add(
                        this.WrapPacket(new Packet() {
                            Text = "Chat"
                        })
                    );
                    break;
                case NetworkActionType.NetworkPlayerKick:
                    wrappers.Add(
                        this.WrapPacket(new Packet() {
                            Text = "Kick"
                        })
                    );
                    break;
                case NetworkActionType.NetworkPlayerBan:
                    wrappers.Add(
                        this.WrapPacket(new Packet() {
                            Text = "Ban"
                        })
                    );
                    break;
                case NetworkActionType.NetworkMapAppend:
                    wrappers.Add(
                        this.WrapPacket(new Packet() {
                            Text = "Map"
                        })
                    );
                    break;
                case NetworkActionType.NetworkPlayerKill:
                    wrappers.Add(
                        this.WrapPacket(new Packet() {
                            Text = "Kill"
                        })
                    );
                    break;
                case NetworkActionType.NetworkPlayerMove:
                    wrappers.Add(
                        this.WrapPacket(new Packet() {
                            Text = "Move"
                        })
                    );
                    break;
                case NetworkActionType.NetworkPacketSend:
                    wrappers.Add(
                        this.WrapPacket(new Packet() {
                            Text = "Raw"
                        })
                    );
                    break;
            }

            return wrappers;
        }
    }
}
