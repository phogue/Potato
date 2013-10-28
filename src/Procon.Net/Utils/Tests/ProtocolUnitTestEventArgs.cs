using System;

namespace Procon.Net.Utils.Tests {

    [Serializable]
    public class ProtocolUnitTestEventArgs : EventArgs, IDisposable {

        /// <summary>
        /// Short messages showing why a test may have failed.
        /// </summary>
        public String Message { get; set; }

        /// <summary>
        /// When this event occured.
        /// </summary>
        public DateTime Stamp { get; set; }

        /// <summary>
        /// The test attached to this event.
        /// </summary>
        public ProtocolUnitTest Test { get; set; }

        public ProtocolUnitTestEventArgs() : base() {
            this.Stamp = DateTime.Now;
        }

        public void Dispose() {
            this.Message = null;
            this.Test = null;
        }
    }
}
