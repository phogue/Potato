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

namespace Procon.Service.Shared {
    /// <summary>
    /// A simple messaging system for bilateral communication with Procon Core
    /// </summary>
    [Serializable]
    public sealed class ServiceMessage : IDisposable {
        /// <summary>
        /// A simple name for this message.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// A list of arguments for this message
        /// </summary>
        public IDictionary<String, String> Arguments { get; set; }

        /// <summary>
        /// When this message was created.
        /// </summary>
        public DateTime Stamp { get; set; }

        /// <summary>
        /// Initializes the service message with the default values.
        /// </summary>
        public ServiceMessage() {
            this.Arguments = new Dictionary<String, String>();
            this.Stamp = DateTime.Now;
        }

        public void Dispose() {
            this.Name = null;

            if (this.Arguments != null) this.Arguments.Clear();
            this.Arguments = null;
        }
    }
}
