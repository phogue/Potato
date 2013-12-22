using System;
using System.Collections.Generic;
using Procon.Core.Shared.Events;

namespace Procon.Core.Events {
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
        public List<GenericEventArgs> Events { get; set; }
    }
}
