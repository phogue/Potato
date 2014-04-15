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
using System.Net.Sockets;

namespace Potato.Net.Shared {
    /// <summary>
    /// Client to handle network communication.
    /// </summary>
    public interface IClient {
        /// <summary>
        /// All of the credentials and connection details required to connect the protocol
        /// </summary>
        IClientSetup Options { get; }

        /// <summary>
        /// The current connection state.
        /// </summary>
        ConnectionState ConnectionState { get; set; }

        /// <summary>
        /// Fired when a packet is successfully sent to the remote end point.
        /// </summary>
        event Action<IClient, IPacketWrapper> PacketSent;

        /// <summary>
        /// Fired when a packet is successfully deserialized from the server.
        /// </summary>
        event Action<IClient, IPacketWrapper> PacketReceived;

        /// <summary>
        /// Fired when a socket exception (something goes wrong with the connection)
        /// </summary>
        event Action<IClient, SocketException> SocketException;

        /// <summary>
        /// Fired when an exception occurs somewhere in the client (which we should debug eh)
        /// </summary>
        event Action<IClient, Exception> ConnectionFailure;

        /// <summary>
        /// Fired whenever this connection state has changed.
        /// </summary>
        event Action<IClient, ConnectionState> ConnectionStateChanged;

        /// <summary>
        /// Sets up the protocol, initializing the client
        /// </summary>
        void Setup(IClientSetup setup);

        /// <summary>
        /// Pokes the connection, ensuring that the connection is still alive. If
        /// this method determines that the connection is dead then it will call for
        /// a shutdown.
        /// </summary>
        /// <remarks>
        ///     <para>
        /// This method is a final check to make sure communications are proceeding in both directions in
        /// the last five minutes. If nothing has been sent and received in the last five minutes then the connection is assumed
        /// dead and a shutdown is initiated.
        /// </para>
        /// </remarks>
        void Poke();

        /// <summary>
        /// Sends a packet to the server
        /// </summary>
        /// <param name="wrapper"></param>
        /// <returns>The packet sent to the server.</returns>
        IPacket Send(IPacketWrapper wrapper);

        /// <summary>
        /// Attempts a connection to the server using the specified host name and port.
        /// </summary>
        void Connect();

        /// <summary>
        /// Shuts down the connection, closing streams etc.
        /// </summary>
        void Shutdown();
    }
}
