#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Potato.Net.Utils.Tests {

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
            Responses = new List<ProtocolUnitTestPacket>();
            Requests = new List<ProtocolUnitTestPacket>();
        }

        public void Dispose() {
            Send.Dispose();
            Responses.ForEach(e => e.Dispose());
            Requests.ForEach(e => e.Dispose());
        }
    }
}
