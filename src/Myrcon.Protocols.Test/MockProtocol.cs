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
using System.Collections.Generic;
using Potato.Net.Shared;
using Potato.Net.Shared.Actions;

namespace Myrcon.Protocols.Test {
    /// <summary>
    /// An empty protocol that should function like aworking protocol (not throw errors at least)
    /// </summary>
    [ProtocolDeclaration(Type = "MockProtocol", Name = "Mock Protocol", Provider = "Myrcon")]
    public class MockProtocol : IProtocol {

        public MockProtocol() : base() {
            this.State = new ProtocolState();
            this.Client = new MockClient();
            this.Options = new ProtocolSetup();
            this.ProtocolType = new ProtocolType() {
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
            this.Options = setup;
            this.Client.Setup(ClientSetup.FromProtocolSetup(setup));

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
