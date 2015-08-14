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
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Potato.Net.Shared;
using Potato.Net.Shared.Utils;

namespace Potato.Net.Protocols.CommandServer {
    /// <summary>
    /// This entire serializer is only ever meant to handle small requests. It's supposed
    /// to be used to issue command requests and get responses.
    /// </summary>
    /// <remarks><para>If we ever need additional functionality or handle larger request/response sizes I'll probably look into a third party implementation.</para></remarks>
    public class CommandServerPacketSerializer : IPacketSerializer {

        public uint PacketHeaderSize { get; set; }

        /// <summary>
        /// Initializes the packet header size requirment for this packet type
        /// </summary>
        public CommandServerPacketSerializer() : base() {
            PacketHeaderSize = 14;
        }

        /// <summary>
        /// Parses the get query string into a NameValueCollection
        /// </summary>
        /// <param name="packet">The packe to check for a query string</param>
        /// <returns>The name value collection</returns>
        protected static NameValueCollection ParseGet(CommandServerPacket packet) {
            return HttpUtility.ParseQueryString(packet.Request.Query);
        }

        /// <summary>
        /// Parses the post data into a NameValueCollection
        /// </summary>
        /// <param name="packet">The packe to check for a query string</param>
        /// <returns>The name value collection</returns>
        protected static NameValueCollection ParsePost(CommandServerPacket packet) {
            var query = new NameValueCollection();

            if (packet.Content != null) {
                query = HttpUtility.ParseQueryString(packet.Content);
            }
            
            return query;
        }

        /// <summary>
        /// Combines various name value collectons into a single name value collecton
        /// </summary>
        /// todo move to extension method?
        protected static NameValueCollection CombineNameValueCollections(params NameValueCollection[] collections) {
            var query = new NameValueCollection();

            foreach (var collection in collections) {
                foreach (var key in collection.AllKeys) {
                    query[key] = collection[key];
                }
            }

            return query;
        }

        /// <summary>
        /// Parses the contents of a HttpRequestHeader.Authorization header value
        /// </summary>
        /// <param name="raw">The contents of the key</param>
        /// <returns>The authorization credentials or an empty array if none can be found</returns>
        protected static string[] ParseAuthorizationHeader(string raw) {
            var credentials = new string[0];

            if (string.IsNullOrEmpty(raw) == false) {
                var parts = raw.Split(' ');

                if (parts.Length == 2 && parts.First().Trim().Equals("Basic", StringComparison.InvariantCultureIgnoreCase)) {
                    var parsed = Encoding.ASCII.GetString(Convert.FromBase64String(parts.Last().Trim())).Split(':');

                    if (parsed.Length == 2) {
                        credentials = parsed;
                    }
                }
            }

            return credentials;
        }

        /// <summary>
        /// Parses packet data into a CommandServerPacket
        /// </summary>
        /// <param name="packet">The packet to populate</param>
        /// <param name="packetData">The raw data to deseralize</param>
        protected static void Parse(CommandServerPacket packet, byte[] packetData) {
            var packetStringData = Regex.Split(Encoding.UTF8.GetString(packetData), @"\r\n\r\n");

            packet.Header = packetStringData.FirstOrDefault();

            // Fetch the rest of the content data for later.
            packet.Content = packetStringData.LastOrDefault();

            if (packet.Header != null) {
                var headers = packet.Header.Split(new [] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);

                if (headers.Length > 0) {
                    var status = headers.First().Wordify();
                    
                    var headerValues = packet.Header.Split(new [] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToDictionary(
                        line => (line.Split(new[] { ":" }, 2, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "").Trim(),
                        line => (line.Split(new[] { ":" }, 2, StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? "").Trim()
                    );

                    if (status.Count == 3 && headerValues.ContainsKey("Host") == true) {
                        packet.Request = new Uri("http://" + headerValues["Host"] + status[1]);
                        packet.Method = status[0];
                        packet.ProtocolVersion = new Version(status[2].Replace("HTTP/", ""));

                        foreach (var header in headerValues) {
                            try {
                                packet.Headers.Set(header.Key, header.Value);
                            }
                            catch {
                                packet.Headers.Set(header.Key, "");
                            }
                        }

                        if (packet.Headers[HttpRequestHeader.Authorization] != null) {
                            packet.Authorization = ParseAuthorizationHeader(packet.Headers[HttpRequestHeader.Authorization]);
                        }

                        if (string.IsNullOrEmpty(packet.Content) && packet.Headers[HttpRequestHeader.ContentLength] == null) {
                            packet.Headers.Set(HttpRequestHeader.ContentLength, "");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Compresses a byte stream using Gzip
        /// </summary>
        protected static byte[] GzipCompress(byte[] data) {
            byte[] compressedData;

            using (var memoryStream = new MemoryStream()) {
                using (var compressionStream = new GZipStream(memoryStream, CompressionMode.Compress)) {
                    compressionStream.Write(data, 0, data.Length);
                }

                compressedData = memoryStream.ToArray();
            }

            return compressedData;
        }

        /// <summary>
        /// Compresses a byte stream using Deflate
        /// </summary>
        protected static byte[] DeflateCompress(byte[] data) {
            byte[] compressedData;

            using (var memoryStream = new MemoryStream()) {
                using (var compressionStream = new DeflateStream(memoryStream, CompressionMode.Compress)) {
                    compressionStream.Write(data, 0, data.Length);
                }

                compressedData = memoryStream.ToArray();
            }

            return compressedData;
        }

        /// <summary>
        /// Serializes the content to a byte array, optionally compressing it if the client has sent
        /// through that it accepts the encoding.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        protected static byte[] SerializeContent(CommandServerPacket packet) {
            var data = Encoding.UTF8.GetBytes(packet.Content);

            if (packet.Headers[HttpResponseHeader.ContentEncoding] != null) {
                var contentEncoding = packet.Headers[HttpResponseHeader.ContentEncoding].ToLowerInvariant();

                if (contentEncoding.Contains("gzip") == true) {
                    data = GzipCompress(data);
                }
                else if (contentEncoding.Contains("deflate") == true) {
                    data = DeflateCompress(data);
                }
            }

            return data;
        }

        /// <summary>
        /// Serializes all of the header data to a byte array for transfer.
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        protected static byte[] SerializeHeader(CommandServerPacket packet, byte[] content) {
            var builder = new StringBuilder();

            // Ensure a couple of headers are through..
            packet.Headers[HttpResponseHeader.Connection] = "close";
            packet.Headers[HttpResponseHeader.ContentLength] = content.Length.ToString(CultureInfo.InvariantCulture);

            builder.AppendFormat("HTTP/{0}.{1} {2} {3}\r\n", packet.ProtocolVersion.Major, packet.ProtocolVersion.Minor, (int)packet.StatusCode, packet.StatusCode);
            builder.Append(packet.Headers);
            // Already adds the double new lines.

            return Encoding.UTF8.GetBytes(builder.ToString());
        }

        public byte[] Serialize(IPacketWrapper wrapper) {
            var commandServerWrapper = wrapper as CommandServerPacket;
            byte[] serialized = null;

            if (commandServerWrapper != null) {
                var content = SerializeContent(commandServerWrapper);
                var header = SerializeHeader(commandServerWrapper, content);
                serialized = new byte[header.Length + content.Length];

                Array.Copy(header, serialized, header.Length);
                Array.Copy(content, 0, serialized, header.Length, content.Length);
            }

            return serialized;
        }

        public IPacketWrapper Deserialize(byte[] packetData) {
            var packet = new CommandServerPacket();

            Parse(packet, packetData);
            packet.Query = CombineNameValueCollections(ParseGet(packet), ParsePost(packet));

            return packet;
        }

        public long ReadPacketSize(byte[] packetData) {
            // Attempt an initial parse to get the headers.
            var packet = new CommandServerPacket();
            Parse(packet, packetData);

            long length = 0;
            
            // If the headers contain a content-length variable, use that.
            if (packet.Headers[HttpRequestHeader.ContentLength] != null) {
                // todo Potential issues with content encoding?

                if (long.TryParse(packet.Headers[HttpRequestHeader.ContentLength], out length) == false) {
                    length = packetData.Length;
                }
                else {
                    length += packet.Header.Length;

                    // The header/content separator.
                    length += 4;
                }
            }

            return length;
        }
    }
}
