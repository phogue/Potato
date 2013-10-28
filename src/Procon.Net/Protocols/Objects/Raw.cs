using System;

namespace Procon.Net.Protocols.Objects {
    public sealed class Raw : NetworkAction {

        /// <summary>
        /// The text part of a packet
        /// </summary>
        public String PacketText { get; set; }

        public Raw() : base() {
            this.PacketText = String.Empty;
        }

        public Raw(String format, params object[] args) : base() {
            this.PacketText = String.Format(format, args);
        }
    }
}
