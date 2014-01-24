namespace Procon.Net.Shared {
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
