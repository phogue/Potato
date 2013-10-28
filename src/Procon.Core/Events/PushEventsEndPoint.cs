using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Procon.Core.Utils;
using Procon.Net.Utils.HTTP;

namespace Procon.Core.Events {
    public class PushEventsEndPoint : IDisposable {

        /// <summary>
        /// The identifier of this stream
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// The stream key (temporary password) specified by the stream end point when setting up.
        /// </summary>
        public String StreamKey { get; set; }

        /// <summary>
        /// Simple flag to determine if a push is in progress.
        /// </summary>
        public bool Pushing { get; set; }

        /// <summary>
        /// The url to push data to.
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// The interval in seconds to push events
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// The content type of the data to be pushed. This determines how the events list should be
        /// serialized for this end point. The default is xml serialization.
        /// </summary>
        public String ContentType { get; set; }

        /// <summary>
        /// List of objects to serialize to xml passing through as content as POST.
        /// </summary>
        public List<GenericEventArgs> EventsStream { get; set; }

        /// <summary>
        /// Event fired whenever a push has completed successfully or with an error.
        /// </summary>
        public event EventHandler PushCompleted;

        public PushEventsEndPoint() {
            this.Id = String.Empty;
            this.StreamKey = String.Empty;
            this.EventsStream = new List<GenericEventArgs>();
            this.Pushing = false;
            this.Interval = 1;
            this.ContentType = Mime.ApplicationXml;
            this.Uri = new Uri("http://localhost/");
        }

        public void Append(GenericEventArgs item) {
            if (this.EventsStream != null) {
                lock (this.EventsStream) {
                    item.Disposed += new EventHandler(GenericEventArgs_Disposed);

                    this.EventsStream.Add(item);
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
            GenericEventArgs item = sender as GenericEventArgs;

            if (item != null) {
                item.Disposed -= new EventHandler(GenericEventArgs_Disposed);

                lock (this.EventsStream) {
                    // Remove the item from the outgoing stream if it exists.
                    this.EventsStream.Remove(item);
                }
            }
        }

        /// <summary>
        /// Serializes the events list in whatever format specified for this end point.
        /// </summary>
        /// <param name="contentType">The type to serialize to</param>
        /// <param name="request">A request payload to send to the push end point</param>
        /// <returns>A string of the formatted list of events</returns>
        public static String SerializeEventsRequest(String contentType, PushEventsRequest request) {
            String requestContent = String.Empty;

            switch (contentType) {
                case Mime.ApplicationXml:
                    requestContent = request.ToXElement().ToString();
                    break;
                case Mime.ApplicationJson:
                    requestContent = JsonConvert.SerializeObject(request, new JsonSerializerSettings() {
                        NullValueHandling = NullValueHandling.Ignore,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        
                    });
                    break;
            }

            return requestContent;
        }

        /// <summary>
        /// Pushes the current data to the Uri.
        /// </summary>
        public void Push() {
            // Don't block, we'll pick it up next round.
            if (this.EventsStream != null && this.Pushing == false) {
                this.Pushing = true;

                List<GenericEventArgs> data;

                // Clone the list of events we're pushing out.
                lock (this.EventsStream) {
                    data = new List<GenericEventArgs>(this.EventsStream);
                }

                // Only transfer if we have something new to report.
                if (data.Count > 0) {
                    Request push = new Request(this.Uri.ToString()) {
                        Method = "POST",
                        RequestContent = PushEventsEndPoint.SerializeEventsRequest(this.ContentType, new PushEventsRequest() {
                            Id = this.Id,
                            StreamKey = this.StreamKey,
                            Events = data
                        }),
                        RequestContentType = this.ContentType,
                        AdditionalData = data
                    };

                    push.RequestComplete += new Request.RequestEventDelegate(RequestCompleted);
                    push.RequestError += new Request.RequestEventDelegate(RequestCompleted);

                    push.BeginRequest();
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
        /// <param name="sender"></param>
        private void RequestCompleted(Request sender) {
            sender.RequestComplete -= new Request.RequestEventDelegate(RequestCompleted);
            sender.RequestError -= new Request.RequestEventDelegate(RequestCompleted);

            List<GenericEventArgs> pushedDataList = sender.AdditionalData as List<GenericEventArgs>;

            if (this.EventsStream != null && pushedDataList != null) {
                lock (this.EventsStream) {
                    foreach (GenericEventArgs pushedData in pushedDataList) {
                        this.EventsStream.Remove(pushedData);
                    }
                }
            }

            this.OnPushCompleted();

            this.Pushing = false;
        }

        protected virtual void OnPushCompleted() {
            EventHandler handler = this.PushCompleted;
            if (handler != null) {
                handler(this, EventArgs.Empty);
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
