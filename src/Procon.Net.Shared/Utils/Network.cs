using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Procon.Net.Shared.Utils {
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
