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
using Newtonsoft.Json;
using Potato.Net.Shared;

namespace Potato.Core.Shared.Models {
    /// <summary>
    /// Details of an open protocol
    /// </summary>
    [Serializable]
    public class ConnectionModel : CoreModel {
        /// <summary>
        /// The unique hash for this connection. This simplifies identifying a connection to a single string that can be compared.
        /// </summary>
        public Guid ConnectionGuid { get; set; }

        /// <summary>
        /// The protocol type of the connection this is describing
        /// </summary>
        public ProtocolType ProtocolType { get; set; }

        /// <summary>
        /// The host name end point of the established connection
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// The host port end point of the established connection
        /// </summary>
        public ushort Port { get; set; }

        /// <summary>
        /// The password used to connect to the end point.
        /// </summary>
        [JsonIgnore]
        public string Password { get; set; }

        /// <summary>
        /// Additional arguments for a connection
        /// </summary>
        public string Arguments { get; set; }
    }
}
