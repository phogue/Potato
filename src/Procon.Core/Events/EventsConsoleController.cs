using System;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Procon.Core.Shared;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;

namespace Procon.Core.Events {

    /// <summary>
    /// Captures events from a procon instance, checking if they are relevent for output
    /// to the console. If they are, then the data is formatted for output if a message is
    /// missing from the event.
    /// 
    /// This will be expanded upon in the future, but for now I just needed a way to see what
    /// was happening within Procon.
    /// 
    /// todo:
    ///     - Implement a check in the variables for a "output level".
    ///         - 0: none
    ///         - 1: notice
    ///         - 2: warning
    ///         - 3: error
    ///         - 4: debug
    ///     - Assign events an output level, only output them if the output level is sufficient.
    ///     - Format this much, much better. Standardize it even, so the connection guid is always 80 chars in or something.
    ///       This is to be used for debugging mostly, so output level 0 = nothing output, but then it should be human readable
    ///       at a glance to quickly debug.
    /// </summary>
    public class EventsConsoleController : CoreController, ISharedReferenceAccess {

        [XmlIgnore, JsonIgnore]
        public SharedReferences Shared { get; private set; }

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

        protected String FormatEvent(GenericEventArgs item) {
            String text = null;

            switch (item.Name) {
                case "TextCommandRegistered":
                    TextCommandModel firstTextCommand = item.Now.TextCommands.FirstOrDefault();
                    ConnectionModel firstConnection = item.Scope.Connections.FirstOrDefault();

                    if (firstTextCommand != null && firstConnection != null) {
                        text = String.Format(@"Registed command(s) ""{0}"" to plugin {1} on connection {2}.", String.Join(", ", firstTextCommand.Commands.ToArray()), this.FormatGuid(firstTextCommand.PluginUid), this.FormatGuid(firstConnection.ConnectionGuid));
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

        protected void Events_EventLogged(object sender, GenericEventArgs e) {
            String text = this.FormatEvent(e);

            if (String.IsNullOrEmpty(text) == false) Console.WriteLine(text);
        }

        public override void Dispose() {
            this.Shared.Events.EventLogged -= new Core.Events.EventsController.EventLoggedHandler(Events_EventLogged);

            base.Dispose();
        }
    }
}