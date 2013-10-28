using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Procon.Net.Utils.Tests {

    [Serializable]
    public class ProtocolUnitTestCommand : IDisposable {

        /// <summary>
        /// The initial single command to send to the server.
        /// </summary>
        public ProtocolUnitTestPacket Send { get; set; }

        /// <summary>
        /// List of responses to check against the server with.
        /// </summary>
        [XmlArray(ElementName = "Responses"), XmlArrayItem(ElementName = "Response"), OptionalField]
        public List<ProtocolUnitTestPacket> Responses;

        /// <summary>
        /// List of events to expect from the server within the timeout of the test.
        /// </summary>
        [XmlArray(ElementName = "Requests"), XmlArrayItem(ElementName = "Request"), OptionalField]
        public List<ProtocolUnitTestPacket> Requests;

        public ProtocolUnitTestCommand() {
            this.Responses = new List<ProtocolUnitTestPacket>();
            this.Requests = new List<ProtocolUnitTestPacket>();
        }

        public void Dispose() {
            this.Send.Dispose();
            this.Responses.ForEach(e => e.Dispose());
            this.Requests.ForEach(e => e.Dispose());
        }
    }
}
