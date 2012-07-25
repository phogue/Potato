using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Text;
using System.Reflection;

namespace Procon.Core.Interfaces.Connections.TextCommands {
    using Procon.Core.Interfaces.Layer.Objects;
    using Procon.Core.Interfaces.Security;

    public class RemoteTextCommandController : TextCommandController {

        /// <summary>
        /// Sends a request to the layer to execute a text command.
        /// </summary>
        public override void ParseTextCommand(CommandInitiator initiator, String text) {
            this.Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.TextCommandsParse,
                EventName.None,
                text
            );
        }

        /// <summary>
        /// Sends a request to the layer to preview a command.
        /// </summary>
        public override void PreviewTextCommand(CommandInitiator initiator, String text) {
            this.Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.TextCommandsPreview,
                EventName.None,
                text
            );
        }
    }
}
