using System;

namespace Procon.Net.Shared {
    /// <summary>
    /// An event originating from the networking side of the protocol implementation.
    /// </summary>
    public interface IClientEventArgs {
        /// <summary>
        /// Stores the type of event (ConnectionStateChanged, PacketSent etc)
        /// </summary>
        ClientEventType EventType { get; set; }

        /// <summary>
        /// The state of the connection (Connected/Disconnected/LoggedIn)
        /// </summary>
        ConnectionState ConnectionState { get; set; }

        /// <summary>
        /// When the event occured. Defaults to the current date/time.
        /// </summary>
        DateTime Stamp { get; set; }

        /// <summary>
        /// Data describing the effected data before the event occured.
        /// </summary>
        IClientEventData Then { get; set; }

        /// <summary>
        /// Data describing how the data looked after the event.
        /// </summary>
        IClientEventData Now { get; set; }
    }
}
