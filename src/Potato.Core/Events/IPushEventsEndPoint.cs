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
    /// An end point to push grouped events to via http/https
    /// </summary>
    public interface IPushEventsEndPoint : IDisposable {
        /// <summary>
        /// The identifier of this stream
        /// </summary>
        String Id { get; set; }

        /// <summary>
        /// The stream key (temporary password) specified by the stream end point when setting up.
        /// </summary>
        String StreamKey { get; set; }

        /// <summary>
        /// Simple flag to determine if a push is in progress.
        /// </summary>
        bool Pushing { get; set; }

        /// <summary>
        /// The url to push data to.
        /// </summary>
        Uri Uri { get; set; }

        /// <summary>
        /// The interval in seconds to push events
        /// </summary>
        int Interval { get; set; }

        /// <summary>
        /// The content type of the data to be pushed. This determines how the events list should be
        /// serialized for this end point. The default is xml serialization.
        /// </summary>
        String ContentType { get; set; }

        /// <summary>
        /// A list of names the event must match to be appended to this stream. Anything not in this list
        /// won't be pushed to the end point.
        /// </summary>
        List<String> InclusiveNames { get; set; }

        /// <summary>
        /// List of objects to serialize to xml passing through as content as POST.
        /// </summary>
        List<IGenericEvent> EventsStream { get; set; }

        /// <summary>
        /// Event fired whenever a push has completed successfully or with an error.
        /// </summary>
        event Action<IPushEventsEndPoint> PushCompleted;

        /// <summary>
        /// Appends an event onto the end of the objects to stream next sync
        /// </summary>
        /// <param name="item">The event to append</param>
        void Append(IGenericEvent item);

        /// <summary>
        /// Pushes the current data to the Uri.
        /// </summary>
        void Push();
    }
}
