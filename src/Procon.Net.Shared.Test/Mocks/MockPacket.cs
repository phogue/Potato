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
using Procon.Net.Shared.Utils;

namespace Procon.Net.Shared.Test.Mocks {
    public class MockPacket : IPacketWrapper {
        /// <summary>
        /// The underlying simple packet class 
        /// </summary>
        public IPacket Packet { get; set; }

        /// <summary>
        /// Basic text to pass bac kand forth
        /// </summary>
        public String Text {
            get { return this.Packet.Text; }
            set {
                this.Packet.Text = value;
                this.Packet.Words = value.Wordify();
            }
        }

        public MockPacket() : base() {
            this.Packet = new Packet();
        }
    }
}
