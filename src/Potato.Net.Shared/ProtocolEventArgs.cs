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

namespace Potato.Net.Shared {
    /// <summary>
    /// Even though a majority of the data inherits from ProtocolObject, we still have
    /// these as seperate fields for serialization.
    /// </summary>
    [Serializable]
    public class ProtocolEventArgs : EventArgs, IProtocolEventArgs {
        public ProtocolEventType ProtocolEventType { get; set; }

        public IProtocolType ProtocolType { get; set; }

        public IProtocolEventData Then { get; set; }

        public IProtocolEventData Now { get; set; }

        public IProtocolStateDifference StateDifference { get; set; }

        public DateTime Stamp { get; set; }

        /// <summary>
        /// Initializes the protocol event with the default values.
        /// </summary>
        public ProtocolEventArgs() {
            Stamp = DateTime.Now;
            ProtocolType = new ProtocolType();
            Then = new ProtocolEventData();
            Now = new ProtocolEventData();
            StateDifference = new ProtocolStateDifference();
        }
    }
}
