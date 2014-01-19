using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Core.Shared {
    /// <summary>
    /// I'd be tempted to refactor GenericEventArgs so I can seal this class. It's
    /// used as a backbone for xml serialization so any inherited classes could
    /// cause the xml serializer to encounter a type it didn't expect.
    /// </summary>
    public interface ICommandResult {
        /// <summary>
        /// A general text message used to describe the event in more detail, if required.
        /// </summary>
        String Message { get; set; }

        /// <summary>
        /// When the event occured. Defaults to the current date/time.
        /// </summary>
        DateTime Stamp { get; set; }

        /// <summary>
        /// The limiting scope of the event (the connection, player etc. that this event is limited to)
        /// </summary>
        CommandData Scope { get; set; }

        /// <summary>
        /// Data that this used to be, like an account being moved from one group to another
        /// this would be the original group.
        /// </summary>
        CommandData Then { get; set; }

        /// <summary>
        /// Data as it is seen "now"
        /// </summary>
        CommandData Now { get; set; }

        /// <summary>
        /// Simple flag determining the success of the command being executed.
        /// </summary>
        Boolean Success { get; set; }

        /// <summary>
        /// A more detailed status describing the command execution.
        /// </summary>
        CommandResultType Status { get; set; }

        /// <summary>
        /// How the output of the command should be handled if it is a remote request.
        /// Defaults to application/xml where the entire result is output
        /// </summary>
        String ContentType { get; set; }
    }
}
