// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Procon.Core.Interfaces.Connections.Plugins {
    using Procon.Net;
    using Procon.Net.Protocols.Objects;
    using Procon.Core.Interfaces.Security.Objects;
    using Procon.Core.Interfaces.Connections.Plugins.Variables;

    public class Test : PluginAPI {

        //Ignore this:
        protected override void WriteConfig(System.Xml.Linq.XElement xNamespace) { }
        public override void Dispose() { }

        public Test()
            : base() {

            this.Author = "Phogue";
            this.Website = "http://phogue.net";
            this.Description = "herro";

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) {
                // Console.WriteLine("asm: {0}", asm.FullName);
            }
        }

        public override void PluginEvent(PluginEventArgs e) {
            base.PluginEvent(e);

            Console.WriteLine("PluginEvent: {0}", e.EventType );

            if (e.EventType == PluginEventType.CallbacksRegistered) {
                //this.RegisterGameEvent("ConnectionStateChanged");
            }
            else if (e.EventType == PluginEventType.ConfigLoaded) {
                this.Variables.Add(
                    new BooleanVariable() {
                        Group = "First Group",
                        Name = "Is Set?",
                        Value = true,
                        Visible = true
                    }
                );

                this.Variables.Add(
                    new ListVariable() {
                        Group = "Second Group",
                        Name = "List of places",
                        Value = new List<string>() {
                        "First",
                        "Second",
                        "Third"
                    },
                        Visible = true
                    }
                );
            }

        }

        public override void ClientEvent(ClientEventArgs e) {
            base.ClientEvent(e);

            if (e.EventType == ClientEventType.ConnectionStateChange) {

                Console.WriteLine("PLUGIN STATE: {0}", e.ConnectionState);
            }
        }

        public override void GameEvent(GameEventArgs e) {
            base.GameEvent(e);

            if (e.EventType == GameEventType.PlayerJoin) {
                Console.WriteLine("{0} just joined, playing {1}", (e.Player as Player), e.GameType);

                Console.WriteLine("There are now {0} players in the server", e.GameState.PlayerList.Count);
            }
        }

        [Command(CustomName = "MyCustomConfigSetting")]
        protected void MyCustomConfigSetting(CommandInitiator initiator, string a_string, int a_number) {
            Console.WriteLine("Got our custom config setting a_string = \"{0}\", a_number = {1}", a_string, a_number);
        }
    }
}
