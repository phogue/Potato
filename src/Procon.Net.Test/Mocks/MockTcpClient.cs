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
using Procon.Net.Shared;

namespace Procon.Net.Test.Mocks {
    public class MockTcpClient : TcpClient {

        public MockTcpClient() : base() {

            this.PacketSerializer = new MockPacketSerializer();
        }

        public MockTcpClient(System.Net.Sockets.TcpClient client) : base() {
            this.Client = client;
            this.Stream = client.GetStream();
            this.ConnectionState = ConnectionState.ConnectionLoggedIn;

            this.PacketSerializer = new MockPacketSerializer();
        }
    }
}
