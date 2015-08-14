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
using Potato.Net.Shared;
using Potato.Net.Shared.Actions;

namespace Myrcon.Protocols.Test {
    [ProtocolDeclaration(Type = "MyrconTestProtocol8", Name = "Test Protocol 8", Provider = "Myrcon")]
    public class TestProtocol : IProtocol {
        public IClient Client { get; private set; }
        public IProtocolState State { get; private set; }
        public IProtocolSetup Options { get; private set; }
        public IProtocolType ProtocolType { get; private set; }
        public string ProtocolsConfigDirectory { get; set; }
        public event Action<IProtocol, IProtocolEventArgs> ProtocolEvent;
        public event Action<IProtocol, IClientEventArgs> ClientEvent;
        public IProtocolSetupResult Setup(IProtocolSetup setup) {
            throw new NotImplementedException();
        }

        public List<IPacket> Action(INetworkAction action) {
            throw new NotImplementedException();
        }

        public IPacket Send(IPacketWrapper packet) {
            throw new NotImplementedException();
        }

        public void AttemptConnection() {
            throw new NotImplementedException();
        }

        public void Shutdown() {
            throw new NotImplementedException();
        }

        public void Synchronize() {
            throw new NotImplementedException();
        }
    }
}
