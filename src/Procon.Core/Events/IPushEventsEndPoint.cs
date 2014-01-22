using System;
using System.Collections.Generic;
using Procon.Core.Shared.Events;

namespace Procon.Core.Events {
    /// <summary>
    /// An end point to push grouped events to via http/https
    /// </summary>
    public interface IPushEventsEndPoint {
        /// <summary>
        /// The identifier of this stream
        /// </summary>
        String Id { get; set; }

        /// <summary>
        /// The stream key (temporary password) specified by the stream end point when setting up.
        /// </summary>
        String StreamKey { get; set; }

        /// <summary>
        /// Simple flag to determine if a push is in progress.
        /// </summary>
        bool Pushing { get; set; }

        /// <summary>
        /// The url to push data to.
        /// </summary>
        Uri Uri { get; set; }

        /// <summary>
        /// The interval in seconds to push events
        /// </summary>
        int Interval { get; set; }

        /// <summary>
        /// The content type of the data to be pushed. This determines how the events list should be
        /// serialized for this end point. The default is xml serialization.
        /// </summary>
        String ContentType { get; set; }

        /// <summary>
        /// List of objects to serialize to xml passing through as content as POST.
        /// </summary>
        List<IGenericEvent> EventsStream { get; set; }

        /// <summary>
        /// Event fired whenever a push has completed successfully or with an error.
        /// </summary>
        event Action<IPushEventsEndPoint> PushCompleted;
    }
}
