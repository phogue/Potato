using System;
using System.Net;
using System.Net.Sockets;

namespace Procon.Net.Test.Mocks {
    public class MockTcpListener {

        /// <summary>
        /// The port to listen on.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The listener 
        /// </summary>
        public TcpListener Listener { get; set; }

        /// <summary>
        /// Fired whenever an incoming request occurs.
        /// </summary>
        public event PacketReceivedHandler PacketReceived;
        public delegate void PacketReceivedHandler(IClient client, MockPacket request);

        /// <summary>
        /// An exception occured.
        /// </summary>
        public event ExceptionHandler Exception;
        public delegate void ExceptionHandler(Exception exception);

        /// <summary>
        /// Creates and starts listening for tcp clients on the specified port.
        /// </summary>
        public void BeginListener() {
            try {
                this.Listener = new TcpListener(IPAddress.Any, this.Port);
                this.Listener.Start();

                // Accept the connection.
                this.Listener.BeginAcceptTcpClient(new AsyncCallback(MockTcpListener.AcceptTcpClientCallback), this);
            }
            catch (Exception e) {
                this.OnException(e);
            }
        }

        // Process the client connection. 
        protected static void AcceptTcpClientCallback(IAsyncResult ar) {

            // Get the listener that handles the client request.
            MockTcpListener daemonListener = (MockTcpListener)ar.AsyncState;

            if (daemonListener.Listener != null) {
                try {
                    // End the operation and display the received data on the console.
                    MockTcpClient client = new MockTcpClient(daemonListener.Listener.EndAcceptTcpClient(ar));

                    // Listen for events on our new client
                    client.PacketReceived += new ClientBase.PacketDispatchHandler(daemonListener.client_PacketReceived);
                    client.ConnectionStateChanged += new ClientBase.ConnectionStateChangedHandler(daemonListener.client_ConnectionStateChanged);

                    // k, go. Now start reading.
                    client.BeginRead();

                    // Signal the calling thread to continue.
                    daemonListener.Listener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClientCallback), daemonListener);
                }
                catch (Exception e) {
                    daemonListener.OnException(e);
                }
            }
        }

        protected void client_PacketReceived(IClient sender, Packet packet) {
            // Bubble the packet for processing.
            this.OnPacketReceived(sender, packet as MockPacket);
        }

        protected void client_ConnectionStateChanged(IClient sender, ConnectionState newState) {
            if (newState == ConnectionState.ConnectionDisconnected) {
                sender.PacketReceived -= new ClientBase.PacketDispatchHandler(this.client_PacketReceived);
                sender.ConnectionStateChanged -= new ClientBase.ConnectionStateChangedHandler(this.client_ConnectionStateChanged);
            }
        }

        protected virtual void OnPacketReceived(IClient client, MockPacket request) {
            PacketReceivedHandler handler = PacketReceived;

            if (handler != null) {
                handler(client, request);
            }
        }

        protected virtual void OnException(Exception exception) {
            ExceptionHandler handler = Exception;

            if (handler != null) {
                handler(exception);
            }
        }
    }
}
