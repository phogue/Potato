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
using Potato.Net.Shared.Actions;
using Potato.Net.Shared.Models;

namespace Potato.Examples.Plugins.TextCommands.Test {
    public class MockGame : Protocol {
        public MockGame() : base() {
            
            this.Client = new MockClient();

            this.State.Players.TryAdd("EA_1", new PlayerModel() {
                Name = "Phogue",
                Uid = "EA_1",
                Score = 50
            });

            this.State.Players.TryAdd("EA_2", new PlayerModel() {
                Name = "Zaeed",
                Uid = "EA_2",
                Score = 0
            });
        }

        protected override void Login(string password) {
            throw new NotImplementedException();
        }

        protected override List<IPacketWrapper> DispatchAction(INetworkAction action) {
            throw new NotImplementedException();
        }

        protected override List<IPacketWrapper> ActionChat(INetworkAction action) {
            throw new NotImplementedException();
        }

        protected override List<IPacketWrapper> ActionKill(INetworkAction action) {
            throw new NotImplementedException();
        }

        protected override List<IPacketWrapper> ActionKick(INetworkAction action) {
            throw new NotImplementedException();
        }

        protected override List<IPacketWrapper> ActionBan(INetworkAction action) {
            throw new NotImplementedException();
        }

        protected override List<IPacketWrapper> ActionMove(INetworkAction action) {
            throw new NotImplementedException();
        }

        protected override List<IPacketWrapper> ActionMap(INetworkAction action) {
            throw new NotImplementedException();
        }

        protected override IPacketWrapper CreatePacket(string format, params object[] args) {
            throw new NotImplementedException();
        }

        protected override IPacketWrapper WrapPacket(IPacket packet) {
            throw new NotImplementedException();
        }
    }
}
