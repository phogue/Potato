#region Copyright
// Copyright 2015 Geoff Green.
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
using System.Collections.Generic;
using Potato.Net;
using Potato.Net.Shared;
using Potato.Net.Shared.Actions;

namespace Potato.Core.Test.Mocks.Protocols {
    [ProtocolDeclaration(Type = "MockProtocol", Name = "Mock Protocol 3", Provider = "Myrcon")]
    public class MockProtocol : IProtocol {

        public MockProtocol() : base() {
            State = new ProtocolState();
            Client = new MockClient();
            Options = new ProtocolSetup();
            ProtocolType = new ProtocolType() {
                Name = "Mock Protocol 3",
                Type = "MockProtocol",
                Provider = "Myrcon"
            };
        }

        public IClient Client { get; private set; }
        public IProtocolState State { get; private set; }
        public IProtocolSetup Options { get; private set; }
        public string Password { get; set; }
        public string Additional { get; set; }
        public IProtocolType ProtocolType { get; private set; }
        public string ProtocolsConfigDirectory { get; set; }
        public event Action<IProtocol, IProtocolEventArgs> ProtocolEvent;
        public event Action<IProtocol, IClientEventArgs> ClientEvent;
        public IProtocolSetupResult Setup(IProtocolSetup setup) {
            Options = setup;
            Client.Setup(ClientSetup.FromProtocolSetup(setup));

            return new ProtocolSetupResult();
        }

        public List<IPacket> Action(INetworkAction action) {
            return null;
        }

        public IPacket Send(IPacketWrapper packet) {
            return null;
        }

        public void AttemptConnection() {
            
        }

        public void Shutdown() {
            
        }

        public void Synchronize() {
            
        }
    }
}
