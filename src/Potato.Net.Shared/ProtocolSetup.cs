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

namespace Potato.Net.Shared {
    /// <summary>
    /// Default setup variables used when creating a new protocol
    /// </summary>
    [Serializable]
    public class ProtocolSetup : IProtocolSetup {
        public String Hostname { get; set; }
        public ushort Port { get; set; }
        public String Password { get; set; }
        public IDictionary<String, String> Arguments { get; set; }

        public string ConfigDirectory { get; set; }

        /// <summary>
        /// Initializes the setup with default values.
        /// </summary>
        public ProtocolSetup() {
            this.Arguments = new Dictionary<String, String>();
        }

        public String ArgumentsString() {
            var list = new List<String>();

            foreach (var variable in this.Arguments) {
                list.Add(String.Format("--{0}", variable.Key));
                list.Add(String.Format(@"""{0}""", variable.Value));
            }

            return String.Join(" ", list);
        }
    }
}
