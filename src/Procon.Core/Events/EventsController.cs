using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Procon.Net.Utils;

namespace Procon.Core.Events {
    using Procon.Core.Variables;
    using Procon.Core.Utils;

    /// <summary>
    /// Logs events, keeping them in memory until a specific time occurs that will write all
    /// events older than a time period to disk.
    /// </summary>
    public class EventsController : Executable {

        /// <summary>
        /// List of events for history
        /// </summary>
        public List<GenericEventArgs> LoggedEvents { get; protected set; }

        /// <summary>
        /// Lock used when fetching a new event Id. I hate that this was originally copied from Procon.Net without looking =\
        /// </summary>
        protected readonly Object AcquireEventIdLock = new object();

        /// <summary>
        /// Aquires an event id
        /// </summary>
        protected ulong AcquireEventId {
            get {
                lock (this.AcquireEventIdLock) {
                    return ++this._mEventId;
                }
            }
        }
        private ulong _mEventId;

        /// <summary>
        /// Fired when an event has been logged.
        /// </summary>
        public event EventLoggedHandler EventLogged;
        public delegate void EventLoggedHandler(Object sender, GenericEventArgs e);

        public EventsController() : base() {
            this.LoggedEvents = new List<GenericEventArgs>();

            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        CommandType = CommandType.EventsFetchAfterEventId,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "eventId",
                                Type = typeof(ulong)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.FetchEventsSince)
                }
            });
        }

        protected virtual void OnEventLogged(GenericEventArgs e) {
            EventLoggedHandler handler = this.EventLogged;

            if (handler != null) {
                handler(this, e);
            }
        }

        public override void Dispose() {
            this.WriteEventsList(this.LoggedEvents);
            this.LoggedEvents.Clear();
            this.LoggedEvents = null;

            this.EventLogged = null;

            base.Dispose();
        }

        /// <summary>
        /// Log an item to the events list
        /// </summary>
        /// <param name="item"></param>
        public void Log(GenericEventArgs item) {
            // Can be null after disposal.
            if (this.LoggedEvents != null) {
                item.Id = this.AcquireEventId;

                lock (this.LoggedEvents) {
                    this.LoggedEvents.Add(item);
                }

                this.OnEventLogged(item);
            }
        }

        /// <summary>
        /// Builds the full path to the events log file on a given stamp.
        /// </summary>
        /// <param name="stamp">The date time to build the directory from. Will ignore the minutes and seconds.</param>
        /// <returns>The path to the file to log events</returns>
        public String EventsLogFileName(DateTime stamp) {
            String directory = Path.Combine(Defines.LogsDirectory, stamp.ToString("yyyy-MM-dd"));

            if (Directory.Exists(directory) == false) {
                Directory.CreateDirectory(directory);
            }

            return Path.Combine(directory, String.Format("events_{0}_to_{1}.xml", stamp.ToString("HH_00_00"), stamp.AddHours(1.0D).ToString("HH_00_00")));
        }

        /// <summary>
        /// Writes the selected events to a file.
        /// </summary>
        /// <param name="events">The events to write.</param>
        protected void WriteEventsList(List<GenericEventArgs> events) {
            if (this.Variables.Get(CommonVariableNames.WriteLogEventsToFile, true) == true) {
                foreach (var eventHourlyGroup in events.GroupBy(e => new DateTime(e.Stamp.Year, e.Stamp.Month, e.Stamp.Day, e.Stamp.Hour, 0, 0))) {
                    String logFileName = this.EventsLogFileName(eventHourlyGroup.Key);

                    if (File.Exists(logFileName) == false) {
                        using (XmlTextWriter writer = new XmlTextWriter(logFileName, Encoding.UTF8)) {
                            writer.WriteStartElement("Events");
                            writer.WriteEndElement();
                            writer.Close();
                        }
                    }

                    XElement xml = XElement.Load(logFileName);

                    foreach (XElement xmlLogEvent in eventHourlyGroup.Select(logEvent => logEvent.ToXElement())) {
                        xmlLogEvent.RemoveAttributes();

                        xml.Add(xmlLogEvent);
                    }

                    xml.Save(logFileName);
                }
            }
        }

        /// <summary>
        /// Write all events older than a specified amount of seconds to a file,
        /// freeing up memory.
        /// </summary>
        public void WriteEvents() {
            this.WriteEvents(DateTime.Now);
        }

        /// <summary>
        /// Write all events older than a specified amount of seconds to a file,
        /// freeing up memory.
        /// </summary>
        /// <param name="now">The current time to use for all calculations in this method</param>
        public void WriteEvents(DateTime now) {

            // Events can be null after disposal.
            if (this.LoggedEvents != null) {

                List<GenericEventArgs> flushEvents = null;

                lock (this.LoggedEvents) {
                    DateTime before = now - TimeSpan.FromSeconds(this.Variables.Get(CommonVariableNames.MaximumEventsTimeSeconds, 30));

                    // All events are appended to the Events list, so we
                    // remove all events until we find one that isn't old enough.
                    flushEvents = this.LoggedEvents.Where(e => e.Stamp < before).ToList();
                }

                // Don't hold up other threads attempting to log an event.
                this.WriteEventsList(flushEvents);

                // Now remove all of the events we just wrote to disk.
                lock (this.LoggedEvents) {
                    foreach (GenericEventArgs removeEvent in flushEvents) {
                        this.LoggedEvents.Remove(removeEvent);

                        // Dispose each event we just wrote to disk and removed from the Events list.
                        removeEvent.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Fetches all events after a passed id, as well as after a certain date.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CommandResultArgs FetchEventsSince(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            ulong eventId = parameters["eventId"].First<ulong>();

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                List<GenericEventArgs> events = null;

                lock (this.LoggedEvents) {
                    events = this.LoggedEvents.Where(e => e.Stamp > DateTime.Now - TimeSpan.FromSeconds(this.Variables.Get(CommonVariableNames.MaximumEventsTimeSeconds, 300)))
                                              .Where(e => e.Id > eventId)
                                              .OrderBy(e => e.Id)
                                              .ToList();
                }

                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Message = String.Format(@"Fetched {0} event(s)", events.Count),
                    Now = {
                        Events = events
                    }
                };
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }
    }
}
