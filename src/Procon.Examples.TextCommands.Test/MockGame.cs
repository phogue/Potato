using System;
using System.Collections.Generic;
using Procon.Net;
using Procon.Net.Actions;
using Procon.Net.Models;

namespace Procon.Examples.TextCommands.Test {
    public class MockGame : Game {
        public MockGame(string hostName, ushort port) : base(hostName, port) {
            
            this.State.Players.AddRange(new List<Player>() {
                new Player() {
                    Name = "Phogue",
                    Uid = "EA_1",
                    Score = 50
                },
                new Player() {
                    Name = "Zaeed",
                    Uid = "EA_2",
                    Score = 0
                }
            });
        }

        protected override void Login(string password) {
            throw new NotImplementedException();
        }

        protected override List<IPacketWrapper> Action(Chat chat) {
            throw new NotImplementedException();
        }

        protected override List<IPacketWrapper> Action(Kick kick) {
            throw new NotImplementedException();
        }

        protected override List<IPacketWrapper> Action(Ban ban) {
            throw new NotImplementedException();
        }

        protected override List<IPacketWrapper> Action(Map map) {
            throw new NotImplementedException();
        }

        protected override List<IPacketWrapper> Action(Kill kill) {
            throw new NotImplementedException();
        }

        protected override List<IPacketWrapper> Action(Move move) {
            throw new NotImplementedException();
        }

        protected override IClient CreateClient(string hostName, ushort port) {
            return new MockClient(hostName, port);
        }

        protected override IPacketWrapper CreatePacket(string format, params object[] args) {
            throw new NotImplementedException();
        }

        protected override IPacketWrapper WrapPacket(IPacket packet) {
            throw new NotImplementedException();
        }
    }
}
