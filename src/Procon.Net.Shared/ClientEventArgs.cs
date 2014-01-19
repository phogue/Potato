using System;

namespace Procon.Net.Shared {
    /// <summary>
    /// An event originating from the networking side of the protocol implementation.
    /// </summary>
    [Serializable]
    public class ClientEventArgs : EventArgs, IClientEventArgs {
        public ClientEventType EventType { get; set; }

        public ConnectionState ConnectionState { get; set; }

        public DateTime Stamp { get; set; }

        public IClientEventData Then { get; set; }

        public IClientEventData Now { get; set; }

        /// <summary>
        /// Initializes the event with the default values.
        /// </summary>
        public ClientEventArgs() {
            this.Stamp = DateTime.Now;

            this.Then = new ClientEventData();
            this.Now = new ClientEventData();
        }
    }
}
