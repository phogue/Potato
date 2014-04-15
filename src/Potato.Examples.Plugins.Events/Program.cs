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
using System.Linq;
using Potato.Core.Shared;
using Potato.Core.Shared.Events;
using Potato.Core.Shared.Plugins;
using Potato.Net.Shared;
using Potato.Net.Shared.Models;

namespace Potato.Examples.Plugins.Events {
    /// <summary>
    /// This plugin just shows how to accept & process events from Potato.
    /// </summary>
    public class Program : PluginController {

        // Critical: You need to create a new project, not reuse this project.
        //           The critical part is the assembly GUID, which must be unique per plugin
        //           but then remain constant over your releases.
        //           Potato uses the GUID to pipe through events/commands.

        public override void ClientEvent(IClientEventArgs e) {
            // Unless you specifically want to override some of the plugins default
            // functionality, you should always call the base method.
            base.ClientEvent(e);

            // See Potato.Net.ClientEventType enum for descriptions of each possible event.
            if (e.EventType == ClientEventType.ClientPacketReceived) {
                IPacket packet = e.Now.Packets.First();

                Console.WriteLine("Program.ClientEvent.ClientPacketReceived: {0} {1} {2} {3}", packet.Origin, packet.Type, packet.RequestId, packet.DebugText);
            }
            // else more!
        }

        public override void GameEvent(IProtocolEventArgs e) {
            // Unless you specifically want to override some of the plugins default
            // functionality, you should always call the base method.
            base.GameEvent(e);

            // See Potato.Net.GameEventType enum for descriptions of each possible event.
            if (e.ProtocolEventType == ProtocolEventType.ProtocolChat) {
                ChatModel chat = e.Now.Chats.First();

                String text = chat.Now.Content.First();
                PlayerModel talker = chat.Now.Players.First();

                Console.WriteLine("Program.GameEvent.GameChat: {0} said \"{1}\"", talker.Name, text);
            }
            // else more!
        }

        public override void GenericEvent(GenericEvent e) {
            // Unless you specifically want to override some of the plugins default
            // functionality, you should always call the base method.
            base.GenericEvent(e);

            // Your plugin won't get a majority of the possible events since there isn't
            // actually a method for Potato to push these events through (on purpose)

            // Shortest history: It was replaced with the command preview/execute/executed
            // so plugins could intercept the actual command that would fire the event.
            // Now we just use it for internal plugin events.

            // See Potato.Net.GameEventType enum for descriptions of each possible event.
            if (e.GenericEventType == GenericEventType.PluginsEnabled) {
                
                // Consider this event here to be the start of your plugin.  It's live, will 
                // get commands and events from now on.

                Console.WriteLine("Program.GenericEvent.PluginsPluginEnabled: k, go!");

                // Log a custom event with Potato whenever this plugin is enabled. You
                // can log these whenever you want something recorded/transported over the push event
                this.Bubble(CommandBuilder.EventsLog(new GenericEvent() {
                    Name = "This is a custom event that will be logged when the plugin is enabled."
                }));
            }
        }
    }
}
