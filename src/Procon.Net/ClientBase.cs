using System;
using System.Net.Sockets;

namespace Procon.Net {
    public abstract class ClientBase {

        public delegate void PacketDispatchHandler(IClient sender, Packet packet);

        public delegate void SocketExceptionHandler(IClient sender, SocketException se);

        public delegate void FailureHandler(IClient sender, Exception exception);

        public delegate void ConnectionStateChangedHandler(IClient sender, ConnectionState newState);
    }
}
