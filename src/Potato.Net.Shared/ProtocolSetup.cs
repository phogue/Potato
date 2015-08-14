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

namespace Potato.Net.Shared {
    /// <summary>
    /// Default setup variables used when creating a new protocol
    /// </summary>
    [Serializable]
    public class ProtocolSetup : IProtocolSetup {
        public string Hostname { get; set; }
        public ushort Port { get; set; }
        public string Password { get; set; }
        public IDictionary<string, string> Arguments { get; set; }

        public string ConfigDirectory { get; set; }

        /// <summary>
        /// Initializes the setup with default values.
        /// </summary>
        public ProtocolSetup() {
            Arguments = new Dictionary<string, string>();
        }

        public string ArgumentsString() {
            var list = new List<string>();

            foreach (var variable in Arguments) {
                list.Add(string.Format("--{0}", variable.Key));
                list.Add(string.Format(@"""{0}""", variable.Value));
            }

            return string.Join(" ", list);
        }
    }
}
