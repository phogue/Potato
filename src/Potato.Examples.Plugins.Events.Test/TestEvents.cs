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
using System.Linq;
using NUnit.Framework;
using Potato.Core.Connections.Plugins;
using Potato.Core.Events;
using Potato.Core.Shared;
using Potato.Net.Shared;
using Potato.Net.Shared.Models;

namespace Potato.Examples.Plugins.Events.Test {
    /// <summary>
    /// This is even lazier than the other examples. There is zero testing as
    /// events are fired and forgotten by Potato.
    /// </summary>
    /// <remarks>This just allows you convientient way of testing plugins via debugging</remarks>
    [TestFixture]
    public class TestEvents {
        /// <summary>
        /// Test for you to debug.
        /// Set a breakpoint within Potato.Examples.Events.ClientEvent.ClientPacketReceived
        /// </summary>
        [Test]
        public void TestClientEventClientPacketReceived() {
            // Create a new plugin controller to load up the test plugin
            var plugins = (CorePluginController)new CorePluginController().Execute();

            // Enable the single plugin that was loaded, otherwise it won't recieve any tunneled
            // commands or events.
            plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            plugins.PluginFactory.ClientEvent(new List<IClientEventArgs>() {
                new ClientEventArgs() {
                    EventType = ClientEventType.ClientPacketReceived,
                    ConnectionState = ConnectionState.ConnectionLoggedIn,
                    Now = {
                        Packets = new List<IPacket>() {
                            new Packet() {
                                RequestId = 1,
                                Origin = PacketOrigin.Client,
                                Type = PacketType.Response,
                                Text = "hello world!",
                                DebugText = "[0-hello] [1-world!]",
                                Words = new List<string>() {
                                    "hello",
                                    "world!"
                                }
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Test for you to debug.
        /// Set a breakpoint within Potato.Examples.Events.GameEvent.GameChat
        /// </summary>
        [Test]
        public void TestGameEventGameChat() {
            // Create a new plugin controller to load up the test plugin
            var plugins = (CorePluginController)new CorePluginController().Execute();

            // Enable the single plugin that was loaded, otherwise it won't recieve any tunneled
            // commands or events.
            plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            plugins.PluginFactory.ProtocolEvent(new List<IProtocolEventArgs>() {
                new ProtocolEventArgs() {
                    ProtocolEventType = ProtocolEventType.ProtocolChat,
                    Now = {
                        Chats = new List<ChatModel>() {
                            new ChatModel() {
                                Origin = NetworkOrigin.Player,
                                Scope = {
                                    Groups = new List<GroupModel>() {
                                        new GroupModel() {
                                            Type = GroupModel.Team,
                                            Uid = "1"
                                        }
                                    }
                                },
                                Now = {
                                    Content = new List<string>() {
                                        "Hello!"
                                    },
                                    Players = new List<PlayerModel>() {
                                        new PlayerModel() {
                                            Uid = "EA_1234",
                                            Name = "Phogue"
                                            // other details..
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Test for you to debug.
        /// Set a breakpoint within Potato.Examples.Events.GenericEvent.PluginsPluginEnabled
        /// </summary>
        [Test]
        public void TestGenericEventPluginsPluginEnabled() {
            // Create a new plugin controller to load up the test plugin
            var plugins = (CorePluginController)new CorePluginController().Execute();

            // Enable the single plugin that was loaded, otherwise it won't recieve any tunneled
            // commands or events.
            plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            // That's it. Enabling the plugin will have this event fired.
        }

        /// <summary>
        /// Test for you to debug.
        /// Set a breakpoint within Potato.Examples.Events.GenericEvent.PluginsPluginEnabled
        /// </summary>
        [Test]
        public void TestCustomEventLoggedFromPlugin() {
            // Create an events controller to bubble up commands from the plugins controller
            var events = (EventsController)new EventsController().Execute();

            // Create a new plugin controller to load up the test plugin
            var plugins = (CorePluginController)new CorePluginController().Execute();

            plugins.BubbleObjects = new List<ICoreController>() {
                events
            };

            // Enable the single plugin that was loaded, otherwise it won't recieve any tunneled
            // commands or events.
            plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            // Now check that our custom event was logged to th events controller
            Assert.IsNotEmpty(events.LoggedEvents);
            Assert.AreEqual("This is a custom event that will be logged when the plugin is enabled.", events.LoggedEvents.First().Name);
        }
    }
}
