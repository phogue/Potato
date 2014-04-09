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

namespace Procon.Core.Shared {
    /// <summary>
    /// Handles throttling and grouping items as they come in, then calling
    /// a method once at a interval to dump a list of events captured in the
    /// last second.
    /// </summary>
    /// <remarks>Useful for reducing cpu time when crossing the AppDomain, as well as returning
    /// control back to Procon core by executing the callback on a seperate thread.</remarks>
    public interface IThrottledStream<T> {
        /// <summary>
        /// The interval to flush events
        /// </summary>
        TimeSpan Interval { get; set; }

        /// <summary>
        /// True if the stream is currently processing
        /// </summary>
        bool Running { get; }

        /// <summary>
        /// The method to periodically flush the contents of the collected list to
        /// </summary>
        Action<List<T>> FlushTo { get; set; }

        /// <summary>
        /// Call this method with the specific item, which will be appended to the list and 
        /// passed through next time.
        /// </summary>
        /// <param name="item">The item </param>
        IThrottledStream<T> Call(T item);

        /// <summary>
        /// Start processing the stream, collecting items.
        /// </summary>
        IThrottledStream<T> Start();

        /// <summary>
        /// Stop processing the stream. Essentially disposes the object.
        /// </summary>
        IThrottledStream<T> Stop();
    }
}
