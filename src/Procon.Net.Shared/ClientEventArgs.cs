using System;

namespace Procon.Net.Shared {

    [Serializable]
    public class ClientEventArgs : EventArgs {

        /// <summary>
        /// Stores the type of event (ConnectionStateChanged, PacketSent etc)
        /// </summary>
        public ClientEventType EventType { get; set; }

        /// <summary>
        /// The state of the connection (Connected/Disconnected/LoggedIn)
        /// </summary>
        public ConnectionState ConnectionState { get; set; }

        /// <summary>
        /// When the event occured. Defaults to the current date/time.
        /// </summary>
        public DateTime Stamp { get; set; }

        /// <summary>
        /// Data describing the effected data before the event occured.
        /// </summary>
        public ClientEventData Then { get; set; }

        /// <summary>
        /// Data describing how the data looked after the event.
        /// </summary>
        public ClientEventData Now { get; set; }

        public ClientEventArgs() {
            this.Stamp = DateTime.Now;

            this.Then = new ClientEventData();
            this.Now = new ClientEventData();
        }
    }
}
