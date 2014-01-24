using System.Collections.Generic;
using Procon.Net.Shared;
using Procon.Net.Shared.Actions;

namespace Procon.Net.Test.Mocks.Game {

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
