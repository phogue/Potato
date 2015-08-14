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
using System.Linq;
using Newtonsoft.Json;
using Potato.Core.Shared;
using Potato.Core.Shared.Events;
using Potato.Core.Shared.Models;
using Potato.Net.Shared;
using Potato.Service.Shared;

namespace Potato.Core.Events {
    /// <summary>
    /// Logs events, keeping them in memory until a specific time occurs that will write all
    /// events older than a time period to disk.
    /// </summary>
    public class EventsController : CoreController, ISharedReferenceAccess {
        /// <summary>
        /// List of events for history
        /// </summary>
        public List<IGenericEvent> LoggedEvents { get; protected set; }

        /// <summary>
        /// Lock used when fetching a new event Id. I hate that this was originally copied from Potato.Net without looking =\
        /// </summary>
        protected readonly object AcquireEventIdLock = new object();

        /// <summary>
        /// Aquires an event id
        /// </summary>
        protected ulong AcquireEventId {
            get {
                lock (AcquireEventIdLock) {
                    return ++_mEventId;
                }
            }
        }
        private ulong _mEventId;

        /// <summary>
        /// Fired when an event has been logged.
        /// </summary>
        public event EventLoggedHandler EventLogged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event that has been logged</param>
        public delegate void EventLoggedHandler(object sender, IGenericEvent e);

        public SharedReferences Shared { get; private set; }

        protected List<string> DefaultEventsLogIgnoredNames = new List<string>() {
            ProtocolEventType.ProtocolPlayerlistUpdated.ToString(),
            ProtocolEventType.ProtocolSettingsUpdated.ToString()
        };

        /// <summary>
        /// Initializes default attributes and sets up command dispatching
        /// </summary>
        public EventsController() : base() {
            Shared = new SharedReferences();
            LoggedEvents = new List<IGenericEvent>();

            CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    CommandType = CommandType.EventsFetchAfterEventId,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "eventId",
                            Type = typeof(ulong)
                        }
                    },
                    Handler = EventsFetchAfterEventId
                },
                new CommandDispatch() {
                    CommandType = CommandType.EventsLog,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "event",
                            Type = typeof(IGenericEvent)
                        }
                    },
                    Handler = EventsLog
                }
            });
        }

        protected virtual void OnEventLogged(IGenericEvent e) {
            var handler = EventLogged;

            if (handler != null) {
                handler(this, e);
            }
        }

        public override void Dispose() {
            WriteEventsList(LoggedEvents);
            LoggedEvents.Clear();
            LoggedEvents = null;

            EventLogged = null;

            base.Dispose();
        }

        /// <summary>
        /// Log an item to the events list
        /// </summary>
        /// <param name="item"></param>
        public void Log(IGenericEvent item) {
            // Can be null after disposal.
            if (LoggedEvents != null) {
                item.Id = AcquireEventId;

                lock (LoggedEvents) {
                    LoggedEvents.Add(item);
                }

                OnEventLogged(item);
            }
        }

        /// <summary>
        /// Builds the full path to the events log file on a given stamp.
        /// </summary>
        /// <param name="stamp">The date time to build the directory from. Will ignore the minutes and seconds.</param>
        /// <returns>The path to the file to log events</returns>
        public string EventsLogFileName(DateTime stamp) {
            var directory = Path.Combine(Defines.LogsDirectory.FullName, stamp.ToString("yyyy-MM-dd"));

            if (Directory.Exists(directory) == false) {
                Directory.CreateDirectory(directory);
            }

            return Path.Combine(directory, string.Format("events_{0}_to_{1}.json", stamp.ToString("HH_00_00"), stamp.AddHours(1.0D).ToString("HH_00_00")));
        }

        /// <summary>
        /// Writes the selected events to a file.
        /// </summary>
        /// <param name="events">The events to write.</param>
        protected bool WriteEventsList(List<IGenericEvent> events) {
            // Assume everything was successful
            var saved = true;

            if (Shared.Variables.Get(CommonVariableNames.WriteLogEventsToFile, false) == true) {
                foreach (var eventHourlyGroup in events.GroupBy(e => new DateTime(e.Stamp.Year, e.Stamp.Month, e.Stamp.Day, e.Stamp.Hour, 0, 0))) {
                    var logFileName = EventsLogFileName(eventHourlyGroup.Key);

                    try {
                        using (TextWriter writer = new StreamWriter(logFileName, true)) {
                            var serializer = new JsonSerializer {
                                NullValueHandling = NullValueHandling.Ignore,
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            };

                            foreach (var @event in eventHourlyGroup) {
                                serializer.Serialize(writer, @event);
                                writer.WriteLine(",");
                            }
                        }
                    }
                    catch {
                        saved = false;
                    }
                }
            }

            return saved;
        }

        /// <summary>
        /// Write all events older than a specified amount of seconds to a file,
        /// freeing up memory.
        /// </summary>
        public void WriteEvents() {
            WriteEvents(DateTime.Now);
        }

        /// <summary>
        /// Write all events older than a specified amount of seconds to a file,
        /// freeing up memory.
        /// </summary>
        /// <param name="now">The current time to use for all calculations in this method</param>
        public void WriteEvents(DateTime now) {

            // Events can be null after disposal.
            if (LoggedEvents != null) {

                List<IGenericEvent> flushEvents = null;

                var before = now - TimeSpan.FromSeconds(Shared.Variables.Get(CommonVariableNames.MaximumEventsTimeSeconds, 30));

                lock (LoggedEvents) {
                    // All events are appended to the Events list, so we
                    // remove all events until we find one that isn't old enough.
                    // Provided the event is not ignored (don't write ignored events)
                    flushEvents = LoggedEvents.Where(e => e.Stamp < before).Where(e => Shared.Variables.Get(CommonVariableNames.EventsLogIgnoredNames, DefaultEventsLogIgnoredNames).Contains(e.Name) == false).ToList();
                }

                // Don't hold up other threads attempting to log an event.
                WriteEventsList(flushEvents);

                // Now remove all old events. This differs from the events we wrote to disk, as we may
                // have ignored some of the events but we still need to get rid of old events.
                lock (LoggedEvents) {
                    var flushed = LoggedEvents.Where(e => e.Stamp < before).ToList();
                    flushed.ForEach(e => {
                        e.Dispose();
                        LoggedEvents.Remove(e);
                    });
                }
            }
        }

        /// <summary>
        /// Fetches all events after a passed id, as well as after a certain date.
        /// </summary>
        public ICommandResult EventsFetchAfterEventId(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var eventId = parameters["eventId"].First<ulong>();

            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                List<IGenericEvent> events = null;

                lock (LoggedEvents) {
                    events = LoggedEvents.Where(e => e.Stamp > DateTime.Now - TimeSpan.FromSeconds(Shared.Variables.Get(CommonVariableNames.MaximumEventsTimeSeconds, 300)))
                                              .Where(e => e.Id > eventId)
                                              .OrderBy(e => e.Id)
                                              .ToList();
                }

                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success,
                    Message = string.Format(@"Fetched {0} event(s)", events.Count),
                    Now = {
                        Events = events
                    }
                };
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Logs a new event
        /// </summary>
        public ICommandResult EventsLog(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var @event = parameters["event"].First<IGenericEvent>();

            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                Log(@event);

                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success,
                    Now = {
                        Events = new List<IGenericEvent>() {
                            @event
                        }
                    }
                };
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }
    }
}
