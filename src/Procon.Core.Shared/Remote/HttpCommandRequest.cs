using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using Procon.Net.Shared;

namespace Procon.Core.Shared.Remote {
    /// <summary>
    /// A request to be propogated with the command
    /// </summary>
    [Serializable]
    public class HttpCommandRequest : ICommandRequest {
        public Dictionary<String, String> Tags { get; set; }
        public List<String> Content { get; set; }
        public List<IPacket> Packets { get; set; }

        /// <summary>
        /// Initalizes the request with default values.
        /// </summary>
        public HttpCommandRequest() {
            this.Tags = new Dictionary<String, String>();
            this.Content = new List<String>();
            this.Packets = new List<IPacket>();
        }

        /// <summary>
        /// Appends a header collection as simple text key-value-pair
        /// </summary>
        /// <returns>this</returns>
        public HttpCommandRequest AppendTags(WebHeaderCollection headers) {
            foreach (var key in headers.AllKeys.Where(key => key != null && this.Tags.ContainsKey(key) == false)) {
                this.Tags.Add(key, headers[key]);
            }

            return this;
        }

        /// <summary>
        /// Appends a NameValue collection as simple text key-value-pair
        /// </summary>
        /// <returns>this</returns>
        public HttpCommandRequest AppendTags(NameValueCollection query) {
            foreach (var key in query.AllKeys.Where(key => key != null && this.Tags.ContainsKey(key) == false)) {
                this.Tags.Add(key, query[key]);
            }

            return this;
        }
    }
}
