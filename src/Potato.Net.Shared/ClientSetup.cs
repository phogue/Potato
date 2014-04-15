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
namespace Potato.Net.Shared {
    /// <summary>
    /// Setup variables used when creating a new client
    /// </summary>
    public class ClientSetup : IClientSetup {
        public string Hostname { get; set; }
        public ushort Port { get; set; }

        /// <summary>
        /// Creates a ClientSetup object from a protocol setup interface
        /// </summary>
        public static IClientSetup FromProtocolSetup(IProtocolSetup setup) {
            return new ClientSetup() {
                Hostname = setup.Hostname,
                Port = setup.Port
            };
        }
    }
}
