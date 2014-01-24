using System;
using System.Net.Sockets;
using Procon.Net.Shared;

namespace Procon.Core.Test.Mocks.Protocols {
    public class MockClient : IClient {
        public string Hostname { get; private set; }
        public ushort Port { get; private set; }
        public IClientSetup Options { get; private set; }
        public ConnectionState ConnectionState { get; set; }
        public event Action<IClient, IPacketWrapper> PacketSent;
        public event Action<IClient, IPacketWrapper> PacketReceived;
        public event Action<IClient, SocketException> SocketException;
        public event Action<IClient, Exception> ConnectionFailure;
        public event Action<IClient, ConnectionState> ConnectionStateChanged;
        public void Setup(IClientSetup setup) {
            
        }

        public void Poke() {
            
        }

        public IPacket Send(IPacketWrapper wrapper) {
            return null;
        }

        public void Connect() {
            
        }

        public void Shutdown() {
            
        }
    }
}
