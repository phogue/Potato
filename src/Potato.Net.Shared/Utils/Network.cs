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
using System.Net;
using System.Net.NetworkInformation;

namespace Potato.Net.Shared.Utils {
    /// <summary>
    /// Basic networking helpers 
    /// </summary>
    public static class Network {

        /// <summary>
        /// Resolves a hostname to an ip address
        /// </summary>
        /// <param name="hostName">The hostname or ip address</param>
        /// <returns></returns>
        public static IPAddress ResolveHostName(string hostName) {
            IPAddress address = IPAddress.None;

            if (IPAddress.TryParse(hostName, out address) == false) {
                try {
                    IPHostEntry hostEntry = Dns.GetHostEntry(hostName);

                    address = hostEntry.AddressList.Length > 0 ? hostEntry.AddressList[0] : IPAddress.None;
                }
                catch {
                    address = IPAddress.None;
                }
            }

            return address;
        }

        /// <summary>
        /// Fetches the external ip address of the current system by pinging a hostname.
        /// </summary>
        /// <param name="hostName">The hostname or ip address</param>
        /// <returns></returns>
        public static IPAddress GetExternalIpAddress(String hostName) {
            PingReply replay = null;

            try {
                replay = new Ping().Send(hostName);
            }
            catch {
                replay = null;
            }

            return replay != null && replay.Status == IPStatus.Success ? replay.Address : null;
        }
    }
}
