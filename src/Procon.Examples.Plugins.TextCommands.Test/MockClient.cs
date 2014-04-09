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
using Procon.Net;
using Procon.Net.Shared;

namespace Procon.Examples.Plugins.TextCommands.Test {
    class MockClient : Client {
        public override IPacket Send(IPacketWrapper wrapper) {
            throw new NotImplementedException();
        }

        public override void Connect() {
            throw new NotImplementedException();
        }

        public override void Shutdown() {
            throw new NotImplementedException();
        }

        public override void Shutdown(Exception e) {
            throw new NotImplementedException();
        }

        public override void Shutdown(SocketException se) {
            throw new NotImplementedException();
        }

        protected override void ShutdownConnection() {
            throw new NotImplementedException();
        }
    }
}
