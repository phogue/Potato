using System.Net;

namespace Procon.Net.Shared.Utils {
    public class Network {

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
    }
}
