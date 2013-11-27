using System.Collections.Generic;
using Procon.Net.Actions;

namespace Procon.Net.Test.Mocks.Game {

    /// <summary>
    /// Used to simply ensure that actions are dispatched/converted etc. correctly.
    /// </summary>
    public class MockActionDispatchGame : MockGame {
        public MockActionDispatchGame(string hostName, ushort port)
            : base(hostName, port) {
        }

        protected override List<IPacket> Action(Chat chat) {
            return new List<IPacket>() {
                new Packet() {
                    Text = "Chat"
                }
            };
        }

        protected override List<IPacket> Action(Kick kick) {
            return new List<IPacket>() {
                new Packet() {
                    Text = "Kick"
                }
            };
        }

        protected override List<IPacket> Action(Ban ban) {
            return new List<IPacket>() {
                new Packet() {
                    Text = "Ban"
                }
            };
        }

        protected override List<IPacket> Action(Map map) {
            return new List<IPacket>() {
                new Packet() {
                    Text = "Map"
                }
            };
        }

        protected override List<IPacket> Action(Kill kill) {
            return new List<IPacket>() {
                new Packet() {
                    Text = "Kill"
                }
            };
        }

        protected override List<IPacket> Action(Move move) {
            return new List<IPacket>() {
                new Packet() {
                    Text = "Move"
                }
            };
        }

        protected override List<IPacket> Action(Raw raw) {
            return new List<IPacket>() {
                new Packet() {
                    Text = "Raw"
                }
            };
        }
    }
}
