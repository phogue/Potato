using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Procon.Net.Protocols.Daemon {
    // Should listen to and manage connections at a basic level for a daemon
    // Security and packet logic should be handled elsewhere.
    public class DaemonListener : IDisposable {

        /// <summary>
        /// The port to listen on.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// A list of active clients with open connections
        /// </summary>
        protected List<DaemonClient> Clients { get; set; }

        /// <summary>
        /// The listener 
        /// </summary>
        public TcpListener Listener { get; set; }

        /// <summary>
        /// Lock used when altering the Clients collection
        /// </summary>
        protected readonly Object ClientsLock = new Object();

        /// <summary>
        /// Lock used during disposal
        /// </summary>
        protected readonly Object DisposeLock = new Object();

        /// <summary>
        /// Fired whenever an incoming request occurs.
        /// </summary>
        public event PacketReceivedHandler PacketReceived;
        public delegate void PacketReceivedHandler(Client<DaemonPacket> client, DaemonPacket request);

        /// <summary>
        /// An exception occured.
        /// </summary>
        public event ExceptionHandler Exception;
        public delegate void ExceptionHandler(Exception exception);

        public DaemonListener() {
            this.Clients = new List<DaemonClient>();
        }

        /// <summary>
        /// Creates and starts listening for tcp clients on the specified port.
        /// </summary>
        public void BeginListener() {
            try {
                this.Listener = new TcpListener(IPAddress.Any, this.Port);
                this.Listener.Start();

                // Accept the connection.
                this.Listener.BeginAcceptTcpClient(new AsyncCallback(DaemonListener.AcceptTcpClientCallback), this);
            }
            catch (Exception e) {
                this.OnException(e);
                this.Dispose();
            }
        }

        // Process the client connection. 
        protected static void AcceptTcpClientCallback(IAsyncResult ar) {

            // Get the listener that handles the client request.
            DaemonListener daemonListener = (DaemonListener)ar.AsyncState;

            if (daemonListener.Listener != null) {
                try {
                    // End the operation and display the received data on the console.
                    DaemonClient client = new DaemonClient(daemonListener.Listener.EndAcceptTcpClient(ar));

                    // Make sure we have a reference to our client.
                    lock (daemonListener.ClientsLock) {
                        daemonListener.Clients.Add(client);
                    }

                    // Listen for events on our new client
                    client.PacketReceived += new Client<DaemonPacket>.PacketDispatchHandler(daemonListener.client_PacketReceived);
                    client.ConnectionStateChanged += new Client<DaemonPacket>.ConnectionStateChangedHandler(daemonListener.client_ConnectionStateChanged);

                    // k, go. Now start reading.
                    client.BeginRead();

                    // Signal the calling thread to continue.
                    daemonListener.Listener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClientCallback), daemonListener);
                }
                catch (Exception e) {
                    daemonListener.OnException(e);
                    daemonListener.Dispose();
                }
            }
        }

        /// <summary>
        /// Respondes to a packet, making sure the response matches the request.
        /// We've purposfully allowed direct packets to be sent in case we needed
        /// to deliberately bypass some of these.
        /// </summary>
        /// <param name="sender">The client that received the response.</param>
        /// <param name="request">The original packet received by the listener.</param>
        /// <param name="response">The response to send to the server.</param>
        public void Respond(Client<DaemonPacket> sender, DaemonPacket request, DaemonPacket response) {
            response.Method = request.Method;
            response.ProtocolVersion = request.ProtocolVersion;

            if (request.Headers[HttpRequestHeader.AcceptEncoding] != null) {
                String acceptEncoding = request.Headers[HttpRequestHeader.AcceptEncoding].ToLowerInvariant();

                if (acceptEncoding.Contains("gzip") == true) {
                    response.Headers[HttpRequestHeader.ContentEncoding] = "gzip";
                }
                else if (acceptEncoding.Contains("deflate") == true) {
                    response.Headers[HttpRequestHeader.ContentEncoding] = "deflate";
                }
            }

            sender.Send(response);
        }

        /// <summary>
        /// Copy the list of clients, then run through poking them to ensure they are still alive.
        /// </summary>
        public void Poke() {
            List<DaemonClient> poked;

            // Note we modify the Clients list in events fired from the client
            // so we take a copy to poke in case this results in a dead lock.
            lock (this.ClientsLock) {
                poked = new List<DaemonClient>(this.Clients);
            }

            poked.ForEach(client => client.Poke());
        }

        protected void client_PacketReceived(Client<DaemonPacket> sender, DaemonPacket packet) {
            // Bubble the packet for processing.
            this.OnPacketReceived(sender, packet);
        }

        /// <summary>
        /// Remove all disconnected clients from our list of clients to shut down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="newState"></param>
        protected void client_ConnectionStateChanged(Client<DaemonPacket> sender, ConnectionState newState) {
            if (newState == ConnectionState.ConnectionDisconnected) {
                lock (this.ClientsLock) {
                    sender.PacketReceived -= new Client<DaemonPacket>.PacketDispatchHandler(this.client_PacketReceived);
                    sender.ConnectionStateChanged -= new Client<DaemonPacket>.ConnectionStateChangedHandler(this.client_ConnectionStateChanged);

                    this.Clients.Remove(sender as DaemonClient);
                }
            }
        }

        protected virtual void OnPacketReceived(Client<DaemonPacket> client, DaemonPacket request) {
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

        public void Dispose() {
            lock (this.DisposeLock) {
                if (this.Listener != null) {
                    this.Listener.Stop();
                    this.Listener = null;

                    lock (this.ClientsLock) {
                        foreach (DaemonClient client in this.Clients) {
                            client.Shutdown();
                            client.PacketReceived -= new Client<DaemonPacket>.PacketDispatchHandler(this.client_PacketReceived);
                            client.ConnectionStateChanged -= new Client<DaemonPacket>.ConnectionStateChangedHandler(this.client_ConnectionStateChanged);
                        }

                        this.Clients.Clear();
                        this.Clients = null;
                    }
                }
            }
        }
    }
}
