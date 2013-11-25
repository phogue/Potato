using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Procon.Net.Test.Mocks {
    public class MockTcpListener {

        /// <summary>
        /// The port to listen on.
        /// </summary>
        public ushort Port { get; set; }

        /// <summary>
        /// The listener 
        /// </summary>
        public TcpListener Listener { get; set; }

        /// <summary>
        /// List of active clients
        /// </summary>
        public List<MockTcpClient> Clients { get; set; }

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

        public MockTcpListener() {
            this.Clients = new List<MockTcpClient>();
        }

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
            MockTcpListener listener = (MockTcpListener)ar.AsyncState;

            if (listener.Listener != null) {
                try {
                    // End the operation and display the received data on the console.
                    MockTcpClient client = new MockTcpClient(listener.Listener.EndAcceptTcpClient(ar));

                    // Listen for events on our new client
                    client.PacketReceived += new ClientBase.PacketDispatchHandler(listener.client_PacketReceived);
                    client.ConnectionStateChanged += new ClientBase.ConnectionStateChangedHandler(listener.client_ConnectionStateChanged);

                    listener.Clients.Add(client);

                    // k, go. Now start reading.
                    client.BeginRead();

                    // Signal the calling thread to continue.
                    listener.Listener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClientCallback), listener);
                }
                catch (Exception e) {
                    listener.OnException(e);
                }
            }
        }

        protected void client_PacketReceived(IClient sender, IPacketWrapper packet) {
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

        public void Shutdown() {
            if (this.Listener != null) {
                this.Listener.Stop();
                this.Listener = null;

                foreach (MockTcpClient client in this.Clients) {
                    client.Shutdown();
                    client.PacketReceived -= new ClientBase.PacketDispatchHandler(this.client_PacketReceived);
                    client.ConnectionStateChanged -= new ClientBase.ConnectionStateChangedHandler(this.client_ConnectionStateChanged);
                }

                this.Clients.Clear();
                this.Clients = null;
            }
        }
    }
}
