#region Copyright
// Copyright 2015 Geoff Green.
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
using System.Linq;
using Potato.Core.Shared;
using Potato.Net.Shared;
using Potato.Net.Shared.Actions;
using Potato.Net.Shared.Actions.Deferred;

namespace Potato.Examples.Plugins.Actions.Test {
    /// <summary>
    /// A mock network layer that reflects packets/actions that are received.
    /// </summary>
    /// <remarks>This code here does not really reflect anything located within Potato.
    /// It's just a class to pretend actions are being handled</remarks>
    public class MockNetworkLayer : CoreController {
        
        /// <summary>
        /// Holds all deferred actions we have recieved.
        /// </summary>
        public WaitingActions Waiting = new WaitingActions();

        public MockNetworkLayer() : base() {
            CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    Name = NetworkActionType.NetworkPlayerKick.ToString(),
                    CommandAttributeType = CommandAttributeType.Handler,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "action",
                            Type = typeof(INetworkAction)
                        }
                    },
                    Handler = NetworkProtocolActionKick
                }
            });
        }

        /// <summary>
        /// Fake receiving packets from the server.
        /// </summary>
        public void MockResponses() {
            Waiting.Mark(new Packet() {
                RequestId = 100,
                Words = new List<string>() {
                    "OK"
                },
                DebugText = "[0-OK]",
                Origin = PacketOrigin.Client,
                Type = PacketType.Response
            });

            Waiting.Mark(new Packet() {
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
        /// This function is similar to that found in Potato.Core.Connections.Connection.NetworkProtocolActionKick
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult NetworkProtocolActionKick(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = new CommandResult() {
                CommandResultType = CommandResultType.Success,
                Success = true
            };

            // You can ignore this. This is a mock of Potato's internal process, but looks nothing like it really

            var kick = parameters["action"].First<INetworkAction>();

            var requests = new List<IPacket>() {
                new Packet() {
                    RequestId = 100,
                    Words = new List<string>() {
                        "admin.kickPlayer",
                        kick.Scope.Players.First().Name
                    },
                    DebugText = string.Format("[0-admin.kickPlayer] [1-{0}]", kick.Scope.Players.First().Name),
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
                    DebugText = string.Format("[0-admin.say] [1-{0}] [2-player] [3-{1}]", kick.Scope.Content.First(), kick.Scope.Players.First().Name),
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request
                }
            };

            Waiting.Wait(new NetworkAction() {
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
