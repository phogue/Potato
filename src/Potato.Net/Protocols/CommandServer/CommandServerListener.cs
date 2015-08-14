#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using Potato.Net.Shared;

namespace Potato.Net.Protocols.CommandServer {
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
        protected ConcurrentDictionary<string, CommandServerClient> Clients { get; set; }

        /// <summary>
        /// The listener 
        /// </summary>
        public TcpListener Listener { get; set; }

        /// <summary>
        /// Lock used during shutdown
        /// </summary>
        protected readonly object ShutdownLock = new object();

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
            Clients = new ConcurrentDictionary<string, CommandServerClient>();
        }

        /// <summary>
        /// Creates and starts listening for tcp clients on the specified port.
        /// </summary>
        public bool BeginListener() {
            var started = false;

            try {
                if (Clients == null) {
                    Clients = new ConcurrentDictionary<string, CommandServerClient>();
                }

                Listener = new TcpListener(IPAddress.Any, Port);
                Listener.Start();
                
                // Accept the connection.
                Listener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClientCallback), this);

                started = true;
            }
            catch (Exception e) {
                Shutdown();
                OnBeginException(e);
            }

            return started;
        }

        /// <summary>
        /// Process the client connection. 
        /// </summary>
        private static void AcceptTcpClientCallback(IAsyncResult ar) {

            // Get the listener that handles the client request.
            var commandServerListener = (CommandServerListener)ar.AsyncState;

            if (commandServerListener.Listener != null) {
                try {
                    // End the operation and display the received data on the console.
                    var client = new CommandServerClient(commandServerListener.Listener.EndAcceptTcpClient(ar), commandServerListener.Certificate);
                    
                    commandServerListener.Clients.TryAdd(client.RemoteEndPoint.ToString(), client);

                    // Listen for events on our new client
                    client.PacketReceived += commandServerListener.ClientPacketReceived;
                    client.ConnectionStateChanged += commandServerListener.ClientConnectionStateChanged;

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
        public static void Respond(IClient sender, CommandServerPacket request, CommandServerPacket response) {
            response.Method = request.Method;
            response.ProtocolVersion = request.ProtocolVersion;

            if (request.Headers[HttpRequestHeader.AcceptEncoding] != null) {
                var acceptEncoding = request.Headers[HttpRequestHeader.AcceptEncoding].ToLowerInvariant();

                if (acceptEncoding.Contains("gzip") == true) {
                    response.Headers[HttpResponseHeader.ContentEncoding] = "gzip";
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
            if (Clients != null) {
                var poked = new List<CommandServerClient>(Clients.Values);

                poked.ForEach(client => client.Poke());
            }
        }

        private void ClientPacketReceived(IClient sender, IPacketWrapper packet) {
            // Bubble the packet for processing.
            OnPacketReceived(sender, packet as CommandServerPacket);
        }

        /// <summary>
        /// Remove all disconnected clients from our list of clients to shut down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="newState"></param>
        protected void ClientConnectionStateChanged(IClient sender, ConnectionState newState) {
            if (newState == ConnectionState.ConnectionDisconnected) {
                sender.PacketReceived -= ClientPacketReceived;
                sender.ConnectionStateChanged -= ClientConnectionStateChanged;

                CommandServerClient removed = null;
                Clients.TryRemove(((CommandServerClient)sender).RemoteEndPoint.ToString(), out removed);
            }
        }

        private void OnPacketReceived(IClient client, CommandServerPacket request) {
            var handler = PacketReceived;

            if (handler != null) {
                handler(client, request);
            }
        }

        private void OnListenerException(Exception exception) {
            var handler = ListenerException;

            if (handler != null) {
                handler(exception);
            }
        }

        private void OnBeginException(Exception exception) {
            var handler = BeginException;

            if (handler != null) {
                handler(exception);
            }
        }

        /// <summary>
        /// Shuts down the current instance
        /// </summary>
        public void Shutdown() {
            lock (ShutdownLock) {
                // Stop listening or new connections
                if (Listener != null) {
                    Listener.Stop();
                    Listener = null;
                }

                // Disconnect all existing connections
                if (Clients != null) {
                    foreach (var client in Clients) {
                        client.Value.Shutdown();
                        client.Value.PacketReceived -= ClientPacketReceived;
                        client.Value.ConnectionStateChanged -= ClientConnectionStateChanged;
                    }

                    Clients.Clear();
                    Clients = null;
                }
            }
        }

        public void Dispose() {
            // No longer calling delegates
            BeginException = null;
            PacketReceived = null;
            ListenerException = null;

            // Shutdown the listener and all current clients.
            Shutdown();

            Certificate = null;
        }
    }
}
