using System;
using System.Net.Sockets;
using Procon.Net.Shared;

namespace Procon.Core.Test.Remote.TestCommandServerController.Mocks {
    public class MockCommandServerClient : IClient {

        public Action PokeCallback { get; set; }
        public Action<IPacketWrapper> SentCallback { get; set; }
        public Action ConnectCallback { get; set; }
        public Action ShutdownCallback { get; set; }

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
            if (this.PokeCallback != null) this.PokeCallback();
        }

        public IPacket Send(IPacketWrapper wrapper) {
            if (this.SentCallback != null) this.SentCallback(wrapper);

            return new Packet();
        }

        public void Connect() {
            if (this.ConnectCallback != null) this.ConnectCallback();
        }

        public void Shutdown() {
            if (this.ShutdownCallback != null) this.ShutdownCallback();
        }
    }
}
