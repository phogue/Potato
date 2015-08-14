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
using System.Threading;
using NUnit.Framework;
using Newtonsoft.Json;
using Potato.Core.Events;
using Potato.Core.Shared;
using Potato.Core.Shared.Events;
using Potato.Core.Shared.Models;
using Potato.Service.Shared;

namespace Potato.Core.Test.Events {
    [TestFixture]
    public class TestEvents {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();

            Defines.LogsDirectory.Refresh();
            if (Defines.LogsDirectory.Exists == true) {
                try {
                    Defines.LogsDirectory.Delete(true);
                }
                catch {
                    Assert.Fail("Logs Directory has outside lock, possibly explorer window open?");
                }
            }
        }

        /// <summary>
        ///     Tests that events are logged correctly.
        /// </summary>
        [Test]
        public void TestEventsLogged() {
            var events = new EventsController();

            events.Log(new GenericEvent() {
                Success = true,
                GenericEventType = GenericEventType.SecurityGroupAdded
            });

            Assert.AreEqual(1, events.LoggedEvents.Count);
        }

        /// <summary>
        ///     Tests that the events are disposed of correctly.
        /// </summary>
        [Test]
        public void TestEventsLoggedDisposed() {
            var events = new EventsController();

            events.Log(new GenericEvent() {
                Success = true,
                GenericEventType = GenericEventType.SecurityGroupAdded
            });

            events.Dispose();

            Assert.IsNull(events.LoggedEvents);
        }

        /// <summary>
        ///     Tests that an event is fired whenever an event is logged.
        /// </summary>
        [Test]
        public void TestEventsLoggedEventFired() {
            var requestWait = new AutoResetEvent(false);
            var events = new EventsController();

            events.EventLogged += (sender, args) => requestWait.Set();

            events.Log(new GenericEvent() {
                Success = true,
                GenericEventType = GenericEventType.SecurityGroupAdded
            });

            // This shouldn't wait at all if the event was actually fired.
            Assert.IsTrue(requestWait.WaitOne(60000));
        }

        /// <summary>
        ///     Tests that events written with the method with no parameters
        /// </summary>
        [Test]
        public void TestEventsNoParametersWrittenToFile() {
            // The current time, to the second.
            var now = DateTime.Now;

            // When the event was logged.
            var then = now.AddHours(-1);

            // Then, rounded to the nearest hour.
            var roundedThenHour = new DateTime(then.Year, then.Month, then.Day, then.Hour, 0, 0);

            var events = new EventsController();
            events.Shared.Variables.Tunnel(CommandBuilder.VariablesSet(CommonVariableNames.WriteLogEventsToFile, true.ToString()).SetOrigin(CommandOrigin.Local));

            events.Log(new GenericEvent() {
                Success = true,
                GenericEventType = GenericEventType.SecurityGroupAdded,
                Scope = new CommandData() {
                    Accounts = new List<AccountModel>() {
                        new AccountModel() {
                            Username = "Phogue"
                        }
                    }
                },
                Stamp = then
            });

            events.WriteEvents();

            var logFileName = events.EventsLogFileName(roundedThenHour);
            
            var logEvent = JsonConvert.DeserializeObject<GenericEvent>(File.ReadAllText(logFileName).Trim(',', '\r', '\n'));

            Assert.IsTrue(logEvent.Success);
            Assert.AreEqual("SecurityGroupAdded", logEvent.Name);
            Assert.AreEqual("Phogue", logEvent.Scope.Accounts.First().Username);
        }

        /// <summary>
        ///     Tests that an event that is one hour old (older than the five minute expire time)
        ///     will be written to a log file.
        /// </summary>
        [Test]
        public void TestEventsSingleWrittenToFile() {
            // The current time, to the second.
            var now = DateTime.Now;

            // When the event was logged.
            var then = now.AddHours(-1);

            // Then, rounded to the nearest hour.
            var roundedThenHour = new DateTime(then.Year, then.Month, then.Day, then.Hour, 0, 0);

            var events = new EventsController();
            events.Shared.Variables.Tunnel(CommandBuilder.VariablesSet(CommonVariableNames.WriteLogEventsToFile, true.ToString()).SetOrigin(CommandOrigin.Local));

            events.Log(new GenericEvent() {
                Success = true,
                GenericEventType = GenericEventType.SecurityGroupAdded,
                Scope = new CommandData() {
                    Accounts = new List<AccountModel>() {
                        new AccountModel() {
                            Username = "Phogue"
                        }
                    }
                },
                Stamp = then
            });

            events.WriteEvents(now);

            var logFileName = events.EventsLogFileName(roundedThenHour);

            var logEvent = JsonConvert.DeserializeObject<GenericEvent>(File.ReadAllText(logFileName).Trim(',', '\r', '\n'));

            Assert.IsTrue(logEvent.Success);
            Assert.AreEqual("SecurityGroupAdded", logEvent.Name);
            Assert.AreEqual("Phogue", logEvent.Scope.Accounts.First().Username);
        }

        /// <summary>
        ///     Tests that an expired event will be written to a log file, but a non-expired
        ///     event will remain in memory.
        /// </summary>
        [Test]
        public void TestEventsUnexpiredWrittenToFile() {
            // The current time, to the second.
            var now = DateTime.Now;

            // When the event was logged.
            var then = now.AddHours(-1);

            // Then, rounded to the nearest hour.
            var roundedThenHour = new DateTime(then.Year, then.Month, then.Day, then.Hour, 0, 0);

            var events = new EventsController();
            events.Shared.Variables.Tunnel(CommandBuilder.VariablesSet(CommonVariableNames.WriteLogEventsToFile, true.ToString()).SetOrigin(CommandOrigin.Local));

            events.Log(new GenericEvent() {
                Success = true,
                GenericEventType = GenericEventType.SecurityGroupAdded,
                Scope = new CommandData() {
                    Accounts = new List<AccountModel>() {
                        new AccountModel() {
                            Username = "Zaeed"
                        }
                    }
                },
                Stamp = now
            });

            events.Log(new GenericEvent() {
                Success = true,
                GenericEventType = GenericEventType.SecurityGroupAdded,
                Scope = new CommandData() {
                    Accounts = new List<AccountModel>() {
                        new AccountModel() {
                            Username = "Phogue"
                        }
                    }
                },
                Stamp = then
            });

            events.WriteEvents(now);

            var logFileName = events.EventsLogFileName(roundedThenHour);



            var logEvent = JsonConvert.DeserializeObject<GenericEvent>(File.ReadAllText(logFileName).Trim(',', '\r', '\n'));

            Assert.IsTrue(logEvent.Success);
            Assert.AreEqual("SecurityGroupAdded", logEvent.Name);
            Assert.AreEqual("Phogue", logEvent.Scope.Accounts.First().Username);
        }
    }
}