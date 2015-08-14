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
using System.Threading;

namespace Potato.Core.Shared {
    /// <summary>
    /// Handles throttling the stream of events to tick every 1 second, reducing
    /// the requirement to cross the AppDomain several hundred times a second.
    /// </summary>
    /// <typeparam name="T">The type of item being streamed in</typeparam>
    public class ThrottledStream<T> : IThrottledStream<T> {
        public TimeSpan Interval { get; set; }
        public bool Running { get; set; }
        public Action<List<T>> FlushTo { get; set; }

        /// <summary>
        /// List of items waiting to be flushed
        /// </summary>
        public List<T> Items { get; set; }

        /// <summary>
        /// Lock used when accessing the items list.
        /// </summary>
        protected readonly object ItemsLock = new object();

        /// <summary>
        /// The timer controlling the interval ticking.
        /// </summary>
        public Timer IntervalTick { get; set; }

        /// <summary>
        /// Sets up the default values (1 second interval etc)
        /// </summary>
        public ThrottledStream() {
            Interval = new TimeSpan(0, 0, 0, 1);
            Items = new List<T>();
        }

        /// <summary>
        /// Flushes any waitign items to the callback method
        /// </summary>
        public void Flush() {
            lock (ItemsLock) {
                if (Running == true && Items.Count > 0) {
                    if (FlushTo != null) {
                        FlushTo(new List<T>(Items));
                    }
                }

                Items.Clear();
            }
        }

        public IThrottledStream<T> Call(T item) {
            if (Running == true) {
                lock (ItemsLock) {
                    Items.Add(item);
                }
            }

            return this;
        }

        public IThrottledStream<T> Start() {
            Running = true;

            IntervalTick = new Timer(state => Flush(), null, Interval, Interval);

            return this;
        }

        public IThrottledStream<T> Stop() {
            Running = false;

            FlushTo = null;

            if (IntervalTick != null) {
                IntervalTick.Dispose();
                IntervalTick = null;
            }

            return this;
        }
    }
}
