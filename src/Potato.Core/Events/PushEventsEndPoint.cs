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
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Potato.Core.Shared.Events;
using Potato.Net.Shared.Utils.HTTP;

namespace Potato.Core.Events {
    /// <summary>
    /// An end point to push grouped events to via http/https
    /// </summary>
    public class PushEventsEndPoint : IPushEventsEndPoint {
        public String Id { get; set; }

        public String StreamKey { get; set; }

        public bool Pushing { get; set; }

        public Uri Uri { get; set; }

        public int Interval { get; set; }

        public String ContentType { get; set; }

        public List<String> InclusiveNames { get; set; }

        public List<IGenericEvent> EventsStream { get; set; }

        /// <summary>
        /// Event fired whenever a push has completed successfully or with an error.
        /// </summary>
        public event Action<IPushEventsEndPoint> PushCompleted;

        /// <summary>
        /// Initializes the end point with the default values.
        /// </summary>
        public PushEventsEndPoint() {
            this.Id = String.Empty;
            this.StreamKey = String.Empty;
            this.EventsStream = new List<IGenericEvent>();
            this.Pushing = false;
            this.Interval = 1;
            this.ContentType = Mime.ApplicationJson;
            this.Uri = new Uri("http://localhost/");
            this.InclusiveNames = new List<String>();
        }

        /// <summary>
        /// Appends an event onto the end of the objects to stream next sync
        /// </summary>
        /// <param name="item">The event to append</param>
        public void Append(IGenericEvent item) {
            if (this.EventsStream != null) {
                lock (this.EventsStream) {
                    if (this.InclusiveNames.Contains(item.Name) == true) {
                        item.Disposed += GenericEventArgs_Disposed;

                        this.EventsStream.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// Fired whenever a generic event is disposed, usually after it has been removed from
        /// the main events controller. This occurs when the event times out. We remove it from
        /// our outgoing stream as well to avoid a memory leak.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenericEventArgs_Disposed(object sender, EventArgs e) {
            GenericEvent item = sender as GenericEvent;

            if (item != null) {
                item.Disposed -= new EventHandler(GenericEventArgs_Disposed);

                lock (this.EventsStream) {
                    // Remove the item from the outgoing stream if it exists.
                    this.EventsStream.Remove(item);
                }
            }
        }

        /// <summary>
        /// Serializes the events list in whatever format specified for this end point, writing it to a text writer.
        /// </summary>
        /// <param name="writer">The writer to output the serialized data to</param>
        /// <param name="contentType">The type to serialize to</param>
        /// <param name="request">A request payload to send to the push end point</param>
        public static void WriteSerializedEventsRequest(TextWriter writer, String contentType, PushEventsRequest request) {
            switch (contentType) {
                case Mime.ApplicationJson:
                    JsonSerializer serializer = new JsonSerializer {
                        NullValueHandling = NullValueHandling.Ignore,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    };

                    serializer.Serialize(writer, request);

                    break;
            }
        }
        
        /// <summary>
        /// Pushes the current data to the Uri.
        /// </summary>
        public void Push() {
            // Don't block, we'll pick it up next round.
            if (this.EventsStream != null && this.Pushing == false) {
                this.Pushing = true;

                List<IGenericEvent> data;

                // Clone the list of events we're pushing out.
                lock (this.EventsStream) {
                    data = new List<IGenericEvent>(this.EventsStream);
                }

                // Only transfer if we have something new to report.
                if (data.Count > 0) {
                    WebRequest request = WebRequest.Create(this.Uri.ToString());
                    request.Method = WebRequestMethods.Http.Post;
                    request.ContentType = this.ContentType;
                    request.Proxy = null;

                    request.BeginGetRequestStream(streamAsyncResult => {
                        try {
                            using (TextWriter writer = new StreamWriter(request.EndGetRequestStream(streamAsyncResult))) {
                                PushEventsEndPoint.WriteSerializedEventsRequest(writer, this.ContentType, new PushEventsRequest() {
                                    Id = this.Id,
                                    StreamKey = this.StreamKey,
                                    Events = data
                                });
                            }

                            request.BeginGetResponse(responseAsyncResult => {
                                try {
                                    WebResponse response = request.EndGetResponse(responseAsyncResult);

                                    this.RequestCompleted(data);

                                    response.Close();
                                }
                                catch {
                                    // General error, remove our sent data dropping the events. Couldn't communicate with end point.
                                    this.RequestCompleted(data);
                                }
                            }, null);
                        }
                        catch {
                            // General error, remove our sent data dropping the events. Couldn't communicate with end point.
                            this.RequestCompleted(data);
                        }
                    }, null);
                }
                else {
                    this.Pushing = false;
                }
            }
        }

        /// <summary>
        /// Fired once the command has completed or errored. We always remove the stream regardless
        /// of it being an error, treating it like a udp stream. The server gets the data or it gets
        /// left behind. We're just pushing updated data.
        /// </summary>
        private void RequestCompleted(IEnumerable<IGenericEvent> pushedDataList) {
            if (this.EventsStream != null && pushedDataList != null) {
                lock (this.EventsStream) {
                    foreach (GenericEvent pushedData in pushedDataList) {
                        pushedData.Disposed -= new EventHandler(GenericEventArgs_Disposed);

                        this.EventsStream.Remove(pushedData);
                    }
                }
            }

            this.OnPushCompleted();

            this.Pushing = false;
        }

        protected virtual void OnPushCompleted() {
            var handler = this.PushCompleted;
            if (handler != null) {
                handler(this);
            }
        }

        public void Dispose() {
            this.Uri = null;

            lock (this.EventsStream) {
                this.EventsStream.Clear();
                this.EventsStream = null;
            }

            this.PushCompleted = null;
        }
    }
}
