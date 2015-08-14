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
using System.Linq;
using System.Net;
using Potato.Net.Shared;

namespace Potato.Core.Shared.Remote {
    /// <summary>
    /// A request to be propogated with the command
    /// </summary>
    [Serializable]
    public class HttpCommandRequest : ICommandRequest {
        public Dictionary<string, string> Tags { get; set; }
        public List<string> Content { get; set; }
        public List<IPacket> Packets { get; set; }

        /// <summary>
        /// Initalizes the request with default values.
        /// </summary>
        public HttpCommandRequest() {
            Tags = new Dictionary<string, string>();
            Content = new List<string>();
            Packets = new List<IPacket>();
        }

        /// <summary>
        /// Appends a header collection as simple text key-value-pair
        /// </summary>
        /// <returns>this</returns>
        public HttpCommandRequest AppendTags(WebHeaderCollection headers) {
            foreach (var key in headers.AllKeys.Where(key => key != null && Tags.ContainsKey(key) == false)) {
                Tags.Add(key, headers[key]);
            }

            return this;
        }

        /// <summary>
        /// Appends a NameValue collection as simple text key-value-pair
        /// </summary>
        /// <returns>this</returns>
        public HttpCommandRequest AppendTags(NameValueCollection query) {
            foreach (var key in query.AllKeys.Where(key => key != null && Tags.ContainsKey(key) == false)) {
                Tags.Add(key, query[key]);
            }

            return this;
        }
    }
}
