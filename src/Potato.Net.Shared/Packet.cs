#region Copyright
// Copyright 2015 Geoff Green.
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
using System.Net;

namespace Potato.Net.Shared {
    /// <summary>
    /// Basic information sent/recv from the connected client.
    /// </summary>
    [Serializable]
    public sealed class Packet : IPacket {

        /// <summary>
        /// When this packet was created
        /// </summary>
        public DateTime Stamp { get; set; }

        /// <summary>
        /// The origin of the packet. This is useful when the server sends back responses to packets, we
        /// can say the packet originiated from the client and this is the response.
        /// </summary>
        public PacketOrigin Origin { get; set; }

        /// <summary>
        /// If this is a response or not to a previous packet.
        /// </summary>
        public PacketType Type { get; set; }

        /// <summary>
        /// The sequence id for this command/event
        /// </summary>
        public int? RequestId { get; set; }

        /// <summary>
        /// The raw bytes used to deserialize this packet.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Textual representation of this packet
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Textual representation of the packet
        /// </summary>
        public string DebugText { get; set; }

        /// <summary>
        /// The wordified version of the text string version of the packet.
        /// </summary>
        public List<string> Words { get; set; }

        /// <summary>
        /// The remote end point for the packet.
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; set; }

        /// <summary>
        /// Initializes the packet with the default values.
        /// </summary>
        public Packet() {
            RequestId = null;
            Stamp = DateTime.Now;
            Words = new List<string>();
        }
    }
}
