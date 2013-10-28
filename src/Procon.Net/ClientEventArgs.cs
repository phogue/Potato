using System;

namespace Procon.Net {

    [Serializable]
    public class ClientEventArgs : EventArgs {

        [NonSerialized]
        private Exception _connectionError;

        /// <summary>
        /// When the event occured. Defaults to the current date/time.
        /// </summary>
        public DateTime Stamp { get; set; }

        /// <summary>
        /// Stores the type of event (ConnectionStateChanged, PacketSent etc)
        /// </summary>
        public ClientEventType EventType { get; set; }

        /// <summary>
        /// The state of the connection (Connected/Disconnected/LoggedIn)
        /// </summary>
        public ConnectionState ConnectionState { get; set; }

        /// <summary>
        /// The last error recorded on the connection
        /// </summary>
        public Exception ConnectionError {
            get { return this._connectionError; }
            set { this._connectionError = value; }
        }

        /// <summary>
        /// The packet that was sent/recv - EventType == PacketSent || PacketReceived
        /// </summary>
        public Packet Packet { get; set; }

        public ClientEventArgs() {
            this.Stamp = DateTime.Now;
        }
    }
}
