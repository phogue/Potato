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

namespace Potato.Net.Shared {
    /// <summary>
    /// A protocol type wrapper describing what the protocol should connect to and who
    /// wrote the implementation
    /// </summary>
    [Serializable]
    public class ProtocolType : IProtocolType {
        /// <summary>
        /// The name of the author or organization that provides this protocol implementation
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// The short key for this game type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The friendly name of the game.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Initalizes the protocol type with empty values.
        /// </summary>
        public ProtocolType() {
            Provider = string.Empty;
            Type = string.Empty;
            Name = string.Empty;
        }

        /// <summary>
        /// Initializes the protocol type from another type
        /// </summary>
        public ProtocolType(IProtocolType from) {
            Provider = from.Provider;
            Type = from.Type;
            Name = from.Name;
        }
    }
}
