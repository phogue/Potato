using System;
using System.Collections.Generic;
using Procon.Core.Shared;
using Procon.Core.Shared.Plugins;
using Procon.Net.Shared;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Actions.Deferred;
using Procon.Net.Shared.Models;

namespace Procon.Examples.Plugins.Actions {
    public class Program : PluginController {
        public Program() : base() {
            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        Name = "KickPlayer",
                        CommandAttributeType = CommandAttributeType.Handler
                    },
                    new CommandDispatchHandler(this.KickPlayer)
                },
                {
                    new CommandAttribute() {
                        Name = "DeferredKickPlayer",
                        CommandAttributeType = CommandAttributeType.Handler
                    },
                    new CommandDispatchHandler(this.DeferredKickPlayer)
                }
            });
        }

        /// <summary>
        /// Kick a player and pretty much don't care about the result.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected CommandResultArgs KickPlayer(Command command, Dictionary<String, CommandParameter> parameters) {
            // You would usually pull the player object from this.GameState but we mock together a player here.
            CommandResultArgs result = this.Action(new Kick() {
                Scope = {
                    Players = new List<Player>() {
                        new Player() {
                            Uid = "EA_12345",
                            Name = "Phogue"
                        }
                    },
                    Content = new List<String>() {
                        "This is a reason to kick this person"
                    }
                }
            });

            // That's it. You can see the queued packets (which might have debugtext in them, might not just yet)
            // If you do need to know everything that was sent/recv for a packet you should look at deferred actions.

            command.Result.Now.Content = new List<String>();

            foreach (IPacket packet in result.Now.Packets) {
                command.Result.Now.Content.Add(String.Format("KickPlayer.Result.packet: {0} {1} {2} {3}", packet.Origin, packet.Type, packet.RequestId, packet.DebugText));
            }

            return command.Result;
        }

        /// <summary>
        /// Kick a player, with various callbacks for different outcomes
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected CommandResultArgs DeferredKickPlayer(Command command, Dictionary<String, CommandParameter> parameters) {
            command.Result = new CommandResultArgs {
                Now = {
                    Content = new List<String>()
                }
            };

            this.Action(new DeferredAction<Kick>() {

                // The action to send to the networking layer. 
                // Ideal execution order: [ "Sent", "Each", "Done", "Always" ]
                // Timeout execution order: [ "Sent", "Expired", "Always" ]
                Action = new Kick() {
                    Scope = {
                        Players = new List<Player>() {
                            new Player() {
                                Uid = "EA_12345",
                                Name = "Phogue"
                            }
                        },
                            Content = new List<String>() {
                            "This is a reason to kick this person"
                        }
                    }
                },

                // All delegates assigned below are optional, but if you don't assign one you
                // might as well just send the action and not bother with a deferred action

                // (Optional) Sent is called when execution has returned from the networking layer. The packets have been sent
                // or are queued to be sent, so they may not have gone through the serialization for packets
                // therefore some of these requests may appear "empty". You should wait for responses in Each/Done
                // to see the full packets both sent and received.
                Sent = (action, requests) => {
                    Console.WriteLine("DeferredKickPlayer: {0}", action.Uid);

                    // Add to our return. This callback is synchronous
                    foreach (IPacket packet in requests) {
                        command.Result.Now.Content.Add(String.Format("KickPlayer.Result.packet: {0} {1} {2} {3}", packet.Origin, packet.Type, packet.RequestId, packet.DebugText));
                    }

                    foreach (IPacket packet in requests) {
                        Console.WriteLine("DeferredKickPlayer.Sent.packet: {0} {1} {2} {3}", packet.Origin, packet.Type, packet.RequestId, packet.DebugText);
                    }
                },

                // Each is called when all responses to all sent packets have been received. It is 
                // called prior to Done and is just a helper to match request/response packets together.
                Each = (action, request, response) => {
                    Console.WriteLine("DeferredKickPlayer.Each {0} {1} ({2}) ({3})", action.Uid, request.RequestId, request.DebugText, response.DebugText);
                },

                // (Optional) Done is called when all responses to all sent packets have been received, but is
                // called after "Each"
                Done = (action, requests, responses) => {
                    Console.WriteLine("DeferredKickPlayer.Done {0}", action.Uid);
                },

                // (Optional) Expired is called when packets have been sent, but responses that match the RequestId
                // have not been recieved. We've waited, but now we're giving up.
                Expired = (action, requests, responses) => {
                    Console.WriteLine("DeferredKickPlayer.Expired {0}", action.Uid);
                },

                // (Optional) If the action is done or expired, this method will always be called when the request is finalized.
                Always = action => {
                    Console.WriteLine("DeferredKickPlayer.Always {0}", action.Uid);
                }
            });

            return command.Result;
        }
    }
}
