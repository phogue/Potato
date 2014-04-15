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
using Potato.Net.Shared;

namespace Potato.Core.Test.Mocks.Protocols {
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
