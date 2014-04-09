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
using Procon.Core.Shared;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;

namespace Procon.Core.Events {
    /// <summary>
    /// Captures events from a procon instance, checking if they are relevent for output
    /// to the console. If they are, then the data is formatted for output if a message is
    /// missing from the event.
    /// <remarks>
    /// <para>
    /// This will be expanded upon in the future, but for now I just needed a way to see what
    /// was happening within Procon.
    /// </para>
    /// </remarks>
    /// </summary>
    public class EventsConsoleController : CoreController, ISharedReferenceAccess {

        public SharedReferences Shared { get; private set; }

        /// <summary>
        /// Initializes events console with default values.
        /// </summary>
        public EventsConsoleController() : base() {
            this.Shared = new SharedReferences();
        }

        public override ICoreController Execute() {
            this.Shared.Events.EventLogged += new Core.Events.EventsController.EventLoggedHandler(Events_EventLogged);

            return base.Execute();
        }

        protected String FormatGuid(Guid guid) {
            return String.Format("{0}..{1}", new String(guid.ToString().Take(5).ToArray()), new String(guid.ToString().Skip(Math.Max(0, guid.ToString().Count() - 3)).Take(3).ToArray()));
        }

        protected String FormatGuid(String guid) {
            return String.Format("{0}..{1}", new String(guid.Take(5).ToArray()), new String(guid.Skip(Math.Max(0, guid.Count() - 3)).Take(3).ToArray()));
        }

        protected String FormatEvent(IGenericEvent item) {
            String text = null;

            switch (item.Name) {
                case "TextCommandRegistered":
                    TextCommandModel firstTextCommand = item.Now.TextCommands.FirstOrDefault();
                    ConnectionModel firstConnection = item.Scope.Connections.FirstOrDefault();

                    if (firstTextCommand != null && firstConnection != null) {
                        text = String.Format(@"Registed command(s) ""{0}"" to plugin {1} on connection {2}.", String.Join(", ", firstTextCommand.Commands.ToArray()), this.FormatGuid(firstTextCommand.PluginGuid), this.FormatGuid(firstConnection.ConnectionGuid));
                    }

                    break;
                case "ConnectionDisconnected":
                case "ConnectionDisconnecting":
                case "ConnectionConnecting":
                case "ConnectionConnected":
                case "ConnectionListening":
                case "ConnectionReady":
                case "ConnectionLoggedIn":
                    text = this.FormatGuid(item.Scope.Connections.First().ConnectionGuid);
                    break;
                default:
                    if (String.IsNullOrEmpty(item.Message.Trim()) == false) {
                        text = item.Message;
                    }
                    
                    break;
            }

            if (String.IsNullOrEmpty(text) == false) {
                text = String.Format("[{0}] {1}: {2}", DateTime.Now.ToString("HH:mm:ss"), item.Name, text);
            }

            return text;
        }

        protected void Events_EventLogged(object sender, IGenericEvent e) {
            String text = this.FormatEvent(e);

            if (String.IsNullOrEmpty(text) == false) Console.WriteLine(text);
        }

        public override void Dispose() {
            this.Shared.Events.EventLogged -= new Core.Events.EventsController.EventLoggedHandler(Events_EventLogged);

            base.Dispose();
        }
    }
}