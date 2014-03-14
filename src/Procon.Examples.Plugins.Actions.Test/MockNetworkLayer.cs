using System;
using System.Collections.Generic;
using System.Linq;
using Procon.Core.Shared;
using Procon.Net.Shared;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Actions.Deferred;

namespace Procon.Examples.Plugins.Actions.Test {
    /// <summary>
    /// A mock network layer that reflects packets/actions that are received.
    /// </summary>
    /// <remarks>This code here does not really reflect anything located within Procon.
    /// It's just a class to pretend actions are being handled</remarks>
    public class MockNetworkLayer : CoreController {
        
        /// <summary>
        /// Holds all deferred actions we have recieved.
        /// </summary>
        public WaitingActions Waiting = new WaitingActions();

        public MockNetworkLayer() : base() {
            this.CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    Name = NetworkActionType.NetworkPlayerKick.ToString(),
                    CommandAttributeType = CommandAttributeType.Handler,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "action",
                            Type = typeof(INetworkAction)
                        }
                    },
                    Handler = this.NetworkProtocolActionKick
                }
            });
        }

        /// <summary>
        /// Fake receiving packets from the server.
        /// </summary>
        public void MockResponses() {
            this.Waiting.Mark(new Packet() {
                RequestId = 100,
                Words = new List<string>() {
                    "OK"
                },
                DebugText = "[0-OK]",
                Origin = PacketOrigin.Client,
                Type = PacketType.Response
            });

            this.Waiting.Mark(new Packet() {
                RequestId = 101,
                Words = new List<string>() {
                    "OK"
                },
                DebugText = "[0-OK]",
                Origin = PacketOrigin.Client,
                Type = PacketType.Response
            });
        }

        /// <summary>
        /// This function is similar to that found in Procon.Core.Connections.Connection.NetworkProtocolActionKick
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult NetworkProtocolActionKick(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = new CommandResult() {
                CommandResultType = CommandResultType.Success,
                Success = true
            };

            // You can ignore this. This is a mock of Procon's internal process, but looks nothing like it really

            INetworkAction kick = parameters["action"].First<INetworkAction>();

            List<IPacket> requests = new List<IPacket>() {
                new Packet() {
                    RequestId = 100,
                    Words = new List<string>() {
                        "admin.kickPlayer",
                        kick.Scope.Players.First().Name
                    },
                    DebugText = String.Format("[0-admin.kickPlayer] [1-{0}]", kick.Scope.Players.First().Name),
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request
                },
                new Packet() {
                    RequestId = 101,
                    Words = new List<string>() {
                        "admin.say",
                        kick.Scope.Content.First(),
                        "player",
                        kick.Scope.Players.First().Name,
                    },
                    DebugText = String.Format("[0-admin.say] [1-{0}] [2-player] [3-{1}]", kick.Scope.Content.First(), kick.Scope.Players.First().Name),
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request
                }
            };

            this.Waiting.Wait(new NetworkAction() {
                ActionType = NetworkActionType.NetworkPlayerKick,
                Scope = kick.Scope,
                Now = kick.Now,
                Then = kick.Then
            }, requests);

            result.Now.Packets = requests;

            return result;
        }

    }
}
