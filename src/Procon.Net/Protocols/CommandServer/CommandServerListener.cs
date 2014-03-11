using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using Procon.Net.Shared;

namespace Procon.Net.Protocols.CommandServer {
    /// <summary>
    /// Should listen to and manage connections at a basic level for a command 
    /// Security and packet logic should be handled elsewhere.
    /// </summary>
    public class CommandServerListener : IDisposable {
        /// <summary>
        /// The port to listen on.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// A list of active clients with open connections
        /// </summary>
        protected ConcurrentDictionary<String, CommandServerClient> Clients { get; set; }

        /// <summary>
        /// The listener 
        /// </summary>
        public TcpListener Listener { get; set; }

        /// <summary>
        /// Lock used during shutdown
        /// </summary>
        protected readonly Object ShutdownLock = new Object();

        /// <summary>
        /// The loaded CommandServer.pfx certificate to encrypt incoming stream
        /// </summary>
        public X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// Fired whenever an incoming request occurs.
        /// </summary>
        public Action<IClient, CommandServerPacket> PacketReceived;

        /// <summary>
        /// An exception occured while starting the listener.
        /// </summary>
        public Action<Exception> BeginException;

        /// <summary>
        /// An exception occured while accepting a connection.
        /// </summary>
        public Action<Exception> ListenerException;

        /// <summary>
        /// Initializes the command server with the default values
        /// </summary>
        public CommandServerListener() {
            this.Clients = new ConcurrentDictionary<String, CommandServerClient>();
        }

        /// <summary>
        /// Creates and starts listening for tcp clients on the specified port.
        /// </summary>
        public bool BeginListener() {
            bool started = false;

            try {
                if (this.Clients == null) {
                    this.Clients = new ConcurrentDictionary<String, CommandServerClient>();
                }

                this.Listener = new TcpListener(IPAddress.Any, this.Port);
                this.Listener.Start();
                
                // Accept the connection.
                this.Listener.BeginAcceptTcpClient(new AsyncCallback(CommandServerListener.AcceptTcpClientCallback), this);

                started = true;
            }
            catch (Exception e) {
                this.Shutdown();
                this.OnBeginException(e);
            }

            return started;
        }

        // Process the client connection. 
        protected static void AcceptTcpClientCallback(IAsyncResult ar) {

            // Get the listener that handles the client request.
            CommandServerListener commandServerListener = (CommandServerListener)ar.AsyncState;

            if (commandServerListener.Listener != null) {
                try {
                    // End the operation and display the received data on the console.
                    CommandServerClient client = new CommandServerClient(commandServerListener.Listener.EndAcceptTcpClient(ar), commandServerListener.Certificate);
                    
                    commandServerListener.Clients.TryAdd(client.RemoteEndPoint.ToString(), client);

                    // Listen for events on our new client
                    client.PacketReceived += commandServerListener.client_PacketReceived;
                    client.ConnectionStateChanged += commandServerListener.client_ConnectionStateChanged;

                    // k, go. Now start reading.
                    client.BeginRead();

                    // Signal the calling thread to continue.
                    commandServerListener.Listener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClientCallback), commandServerListener);
                }
                catch (Exception e) {
                    commandServerListener.Shutdown();
                    commandServerListener.OnListenerException(e);
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
        public void Respond(IClient sender, CommandServerPacket request, CommandServerPacket response) {
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
            if (this.Clients != null) {
                List<CommandServerClient> poked = new List<CommandServerClient>(this.Clients.Values);

                poked.ForEach(client => client.Poke());
            }
        }

        protected void client_PacketReceived(IClient sender, IPacketWrapper packet) {
            // Bubble the packet for processing.
            this.OnPacketReceived(sender, packet as CommandServerPacket);
        }

        /// <summary>
        /// Remove all disconnected clients from our list of clients to shut down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="newState"></param>
        protected void client_ConnectionStateChanged(IClient sender, ConnectionState newState) {
            if (newState == ConnectionState.ConnectionDisconnected) {
                sender.PacketReceived -= this.client_PacketReceived;
                sender.ConnectionStateChanged -= this.client_ConnectionStateChanged;

                CommandServerClient removed = null;
                this.Clients.TryRemove(((CommandServerClient)sender).RemoteEndPoint.ToString(), out removed);
            }
        }

        protected virtual void OnPacketReceived(IClient client, CommandServerPacket request) {
            var handler = PacketReceived;

            if (handler != null) {
                handler(client, request);
            }
        }

        protected virtual void OnListenerException(Exception exception) {
            var handler = ListenerException;

            if (handler != null) {
                handler(exception);
            }
        }

        protected virtual void OnBeginException(Exception exception) {
            var handler = BeginException;

            if (handler != null) {
                handler(exception);
            }
        }

        /// <summary>
        /// Shuts down the current instance
        /// </summary>
        public void Shutdown() {
            lock (this.ShutdownLock) {
                // Stop listening or new connections
                if (this.Listener != null) {
                    this.Listener.Stop();
                    this.Listener = null;
                }

                // Disconnect all existing connections
                if (this.Clients != null) {
                    foreach (var client in this.Clients) {
                        client.Value.Shutdown();
                        client.Value.PacketReceived -= this.client_PacketReceived;
                        client.Value.ConnectionStateChanged -= this.client_ConnectionStateChanged;
                    }

                    this.Clients.Clear();
                    this.Clients = null;
                }
            }
        }

        public void Dispose() {
            // No longer calling delegates
            this.BeginException = null;
            this.PacketReceived = null;
            this.ListenerException = null;

            // Shutdown the listener and all current clients.
            this.Shutdown();

            this.Certificate = null;
        }
    }
}
