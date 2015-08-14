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
using System.Collections.Specialized;
using System.Net;
using Potato.Net.Shared;

namespace Potato.Net.Protocols.CommandServer {
    /// <summary>
    /// A packet (essentially http) for communication via the command server
    /// </summary>
    [Serializable]
    public class CommandServerPacket : IPacketWrapper {

        /// <summary>
        /// The requested Uri
        /// </summary>
        public Uri Request { get; set; }

        /// <summary>
        /// The status code, used when responding to the request.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// The method used during the request/response
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The protocol version being used (1.0.0.0 or 1.1.0.0)
        /// </summary>
        public Version ProtocolVersion { get; set; }

        /// <summary>
        /// All of the headers for this http request/response.
        /// </summary>
        public WebHeaderCollection Headers { get; set; }
        
        /// <summary>
        /// The post/get data attached to the request.
        /// </summary>
        public NameValueCollection Query { get; set; }

        /// <summary>
        /// The raw header data recieved or to be sent.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// The raw data from a POST request or the data to be output as a response.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Basic strings supplied for http authorization
        /// </summary>
        public string[] Authorization { get; set; }

        /// <summary>
        /// The base packet in this request
        /// </summary>
        public IPacket Packet { get; set; }

        /// <summary>
        /// Initializes default values.
        /// </summary>
        public CommandServerPacket() {
            Headers = new WebHeaderCollection();
            Query = new NameValueCollection();
            Content = string.Empty;
            Authorization = new string[0];

            Packet = new Packet();
        }
    }
}
