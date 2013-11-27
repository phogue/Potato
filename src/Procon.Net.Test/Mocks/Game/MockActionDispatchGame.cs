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

        protected override List<IPacketWrapper> Action(Chat chat) {
            return new List<IPacketWrapper>() {
                this.WrapPacket(new Packet() {
                    Text = "Chat"
                })
            };
        }

        protected override List<IPacketWrapper> Action(Kick kick) {
            return new List<IPacketWrapper>() {
                this.WrapPacket(new Packet() {
                    Text = "Kick"
                })
            };
        }

        protected override List<IPacketWrapper> Action(Ban ban) {
            return new List<IPacketWrapper>() {
                this.WrapPacket(new Packet() {
                    Text = "Ban"
                })
            };
        }

        protected override List<IPacketWrapper> Action(Map map) {
            return new List<IPacketWrapper>() {
                this.WrapPacket(new Packet() {
                    Text = "Map"
                })
            };
        }

        protected override List<IPacketWrapper> Action(Kill kill) {
            return new List<IPacketWrapper>() {
                this.WrapPacket(new Packet() {
                    Text = "Kill"
                })
            };
        }

        protected override List<IPacketWrapper> Action(Move move) {
            return new List<IPacketWrapper>() {
                this.WrapPacket(new Packet() {
                    Text = "Move"
                })
            };
        }

        protected override List<IPacketWrapper> Action(Raw raw) {
            return new List<IPacketWrapper>() {
                this.WrapPacket(new Packet() {
                    Text = "Raw"
                })
            };
        }
    }
}
