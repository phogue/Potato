using System;
using System.Collections.Generic;
using Procon.Core.Shared.Events;

namespace Procon.Core.Events {
    /// <summary>
    /// A single volatile request sent to the push end point.
    /// </summary>
    public class PushEventsRequest {
        /// <summary>
        /// The identifier of this stream
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// The stream key (temporary password) specified by the stream end point when setting up.
        /// </summary>
        public String StreamKey { get; set; }

        /// <summary>
        /// A list of events to serialize and send.
        /// </summary>
        public List<IGenericEvent> Events { get; set; }
    }
}
