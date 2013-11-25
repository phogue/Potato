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

namespace Procon.Net.Protocols.Daemon {
    using Procon.Net.Utils;

    /// <summary>
    /// This entire serializer is only ever meant to handle small requests. It's supposed
    /// to be used to issue command requests and get responses.
    /// </summary>
    /// <remarks><para>If we ever need additional functionality or handle larger request/response sizes I'll probably look into a third party implementation.</para></remarks>
    public class DaemonPacketSerializer : IPacketSerializer {

        public uint PacketHeaderSize { get; set; }

        public DaemonPacketSerializer() : base() {
            this.PacketHeaderSize = 14;
        }

        protected static NameValueCollection ParseGet(DaemonPacket packet) {
            return HttpUtility.ParseQueryString(packet.Request.Query);
        }

        protected static NameValueCollection ParsePost(DaemonPacket packet) {
            NameValueCollection query = new NameValueCollection();

            if (packet.Content != null) {
                query = HttpUtility.ParseQueryString(packet.Content);
            }
            
            return query;
        }

        protected static NameValueCollection CombineNameValueCollections(params NameValueCollection[] collections) {
            NameValueCollection query = new NameValueCollection();

            foreach (NameValueCollection collection in collections) {
                foreach (String key in collection.AllKeys) {
                    query[key] = collection[key];
                }
            }

            return query;
        }

        protected static void Parse(DaemonPacket packet, byte[] packetData) {
            String[] packetStringData = Regex.Split(Encoding.UTF8.GetString(packetData), @"\r\n\r\n");

            packet.Header = packetStringData.FirstOrDefault();

            // Fetch the rest of the content data for later.
            packet.Content = packetStringData.LastOrDefault();

            if (packet.Header != null) {
                string[] headers = packet.Header.Split(new [] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);

                if (headers.Length > 0) {
                    List<String> status = headers.First().Wordify();

                    var headerValues = packet.Header.Split(new [] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToDictionary(
                        line => line.Split(new [] { ": " }, 2, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(),
                        line => line.Split(new [] { ": " }, 2, StringSplitOptions.RemoveEmptyEntries).LastOrDefault()
                    );

                    if (status.Count == 3 && headerValues.ContainsKey("Host") == true) {
                        packet.Request = new Uri("http://" + headerValues["Host"] + status[1]);
                        packet.Method = status[0];
                        packet.ProtocolVersion = new Version(status[2].Replace("HTTP/", ""));

                        foreach (var header in headerValues) {
                            packet.Headers[header.Key] = header.Value;
                        }
                    }
                }
            }
        }

        protected static byte[] GzipCompress(byte[] data) {
            byte[] compressedData;

            using (MemoryStream memoryStream = new MemoryStream()) {
                using (GZipStream compressionStream = new GZipStream(memoryStream, CompressionMode.Compress)) {
                    compressionStream.Write(data);
                }

                compressedData = memoryStream.ToArray();
            }

            return compressedData;
        }

        protected static byte[] DeflateCompress(byte[] data) {
            byte[] compressedData;

            using (MemoryStream memoryStream = new MemoryStream()) {
                using (DeflateStream compressionStream = new DeflateStream(memoryStream, CompressionMode.Compress)) {
                    compressionStream.Write(data);
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
        protected byte[] SerializeContent(DaemonPacket packet) {
            byte[] data = Encoding.UTF8.GetBytes(packet.Content);

            if (packet.Headers[HttpRequestHeader.ContentEncoding] != null) {
                String contentEncoding = packet.Headers[HttpRequestHeader.ContentEncoding].ToLowerInvariant();

                if (contentEncoding.Contains("gzip") == true) {
                    data = DaemonPacketSerializer.GzipCompress(data);
                }
                else if (contentEncoding.Contains("deflate") == true) {
                    data = DaemonPacketSerializer.DeflateCompress(data);
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
        protected byte[] SerializeHeader(DaemonPacket packet, byte[] content) {
            StringBuilder builder = new StringBuilder();

            // Ensure a couple of headers are through..
            packet.Headers[HttpRequestHeader.Connection] = "close";
            packet.Headers[HttpRequestHeader.ContentLength] = content.Length.ToString(CultureInfo.InvariantCulture);

            builder.AppendFormat("HTTP/{0}.{1} {2} {3}\r\n", packet.ProtocolVersion.Major, packet.ProtocolVersion.Minor, (int)packet.StatusCode, packet.StatusCode);
            builder.Append(packet.Headers);
            // Already adds the double new lines.

            return Encoding.UTF8.GetBytes(builder.ToString());
        }

        public byte[] Serialize(IPacketWrapper wrapper) {
            DaemonPacket daemonPacket = wrapper as DaemonPacket;
            byte[] serialized = null;

            if (daemonPacket != null) {
                byte[] content = this.SerializeContent(daemonPacket);
                byte[] header = this.SerializeHeader(daemonPacket, content);
                serialized = new byte[header.Length + content.Length];

                Array.Copy(header, serialized, header.Length);
                Array.Copy(content, 0, serialized, header.Length, content.Length);
            }

            return serialized;
        }

        public IPacketWrapper Deserialize(byte[] packetData) {
            DaemonPacket packet = new DaemonPacket();

            DaemonPacketSerializer.Parse(packet, packetData);
            packet.Query = DaemonPacketSerializer.CombineNameValueCollections(DaemonPacketSerializer.ParseGet(packet), DaemonPacketSerializer.ParsePost(packet));

            return packet;
        }

        public long ReadPacketSize(byte[] packetData) {
            // Attempt an initial parse to get the headers.
            DaemonPacket packet = new DaemonPacket();
            DaemonPacketSerializer.Parse(packet, packetData);

            long length = packetData.Length;

            // If the headers contain a content-length variable, use that.
            if (packet.Headers[HttpRequestHeader.ContentLength] != null) {
                // todo Potential issues with content encoding?

                if (long.TryParse(packet.Headers[HttpRequestHeader.ContentLength], out length) == false) {
                    length = packetData.Length;
                }
                else {
                    length += packet.Header.Length;
                }

                // The header/content separator.
                length += 4;
            }

            return length;
        }
    }
}
