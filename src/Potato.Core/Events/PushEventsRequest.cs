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
using Potato.Core.Shared.Events;

namespace Potato.Core.Events {
    /// <summary>
    /// A single volatile request sent to the push end point.
    /// </summary>
    public class PushEventsRequest {
        /// <summary>
        /// The identifier of this stream
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The stream key (temporary password) specified by the stream end point when setting up.
        /// </summary>
        public string StreamKey { get; set; }

        /// <summary>
        /// A list of events to serialize and send.
        /// </summary>
        public List<IGenericEvent> Events { get; set; }
    }
}
