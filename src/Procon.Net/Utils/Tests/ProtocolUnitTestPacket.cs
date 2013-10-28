using System;

namespace Procon.Net.Utils.Tests {

    [Serializable]
    public class ProtocolUnitTestPacket : ProtocolUnitTestObject {

        /// <summary>
        /// Flag to specify if this packet has been matched already.
        /// </summary>
        public bool Found { get; set; }

        public override string ToString() {
            return this.Regex == null ? this.Text : String.Format("/{0}/", this.Regex);
        }
    }
}
