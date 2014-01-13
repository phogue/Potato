using System;
using System.Net.Sockets;
using Procon.Net;
using Procon.Net.Shared;

namespace Procon.Examples.Plugins.TextCommands.Test {
    class MockClient : Client {
        public MockClient(string hostname, ushort port) : base(hostname, port) {
        }

        public override IPacket Send(IPacketWrapper wrapper) {
            throw new NotImplementedException();
        }

        public override void Connect() {
            throw new NotImplementedException();
        }

        public override void Shutdown() {
            throw new NotImplementedException();
        }

        public override void Shutdown(Exception e) {
            throw new NotImplementedException();
        }

        public override void Shutdown(SocketException se) {
            throw new NotImplementedException();
        }

        protected override void ShutdownConnection() {
            throw new NotImplementedException();
        }
    }
}
