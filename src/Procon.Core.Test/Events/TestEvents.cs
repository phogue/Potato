#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using NUnit.Framework;
using Procon.Core.Events;
using Procon.Core.Shared;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;
using Procon.Service.Shared;

#endregion

namespace Procon.Core.Test.Events {
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
            DateTime now = DateTime.Now;

            // When the event was logged.
            DateTime then = now.AddHours(-1);

            // Then, rounded to the nearest hour.
            var roundedThenHour = new DateTime(then.Year, then.Month, then.Day, then.Hour, 0, 0);

            var events = new EventsController();

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

            String logFileName = events.EventsLogFileName(roundedThenHour);

            XElement logEventElement = XElement.Load(logFileName).Element("Event");

            Assert.AreEqual("true", logEventElement.Element("Success").Value);
            Assert.AreEqual("SecurityGroupAdded", logEventElement.Element("Name").Value);
            Assert.AreEqual(then.ToString("s", CultureInfo.InvariantCulture), DateTime.Parse(logEventElement.Element("Stamp").Value).ToString("s", CultureInfo.InvariantCulture));

            Assert.AreEqual("Phogue", logEventElement.Descendants("AccountModel").First().Element("Username").Value);
        }

        /// <summary>
        ///     Tests that an event that is one hour old (older than the five minute expire time)
        ///     will be written to a log file.
        /// </summary>
        [Test]
        public void TestEventsSingleWrittenToFile() {
            // The current time, to the second.
            DateTime now = DateTime.Now;

            // When the event was logged.
            DateTime then = now.AddHours(-1);

            // Then, rounded to the nearest hour.
            var roundedThenHour = new DateTime(then.Year, then.Month, then.Day, then.Hour, 0, 0);

            var events = new EventsController();

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

            String logFileName = events.EventsLogFileName(roundedThenHour);

            XElement logEventElement = XElement.Load(logFileName).Element("Event");

            Assert.AreEqual("true", logEventElement.Element("Success").Value);
            Assert.AreEqual("SecurityGroupAdded", logEventElement.Element("Name").Value);
            Assert.AreEqual(then.ToString("s", CultureInfo.InvariantCulture), DateTime.Parse(logEventElement.Element("Stamp").Value).ToString("s", CultureInfo.InvariantCulture));

            Assert.AreEqual("Phogue", logEventElement.Descendants("AccountModel").First().Element("Username").Value);
        }

        /// <summary>
        ///     Tests that an expired event will be written to a log file, but a non-expired
        ///     event will remain in memory.
        /// </summary>
        [Test]
        public void TestEventsUnexpiredWrittenToFile() {
            // The current time, to the second.
            DateTime now = DateTime.Now;

            // When the event was logged.
            DateTime then = now.AddHours(-1);

            // Then, rounded to the nearest hour.
            var roundedThenHour = new DateTime(then.Year, then.Month, then.Day, then.Hour, 0, 0);

            var events = new EventsController();

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

            String logFileName = events.EventsLogFileName(roundedThenHour);

            XElement logEventsElement = XElement.Load(logFileName);
            Assert.AreEqual(1, logEventsElement.Elements("Event").Count());

            XElement logEventElement = logEventsElement.Element("Event");

            Assert.AreEqual("true", logEventElement.Element("Success").Value);
            Assert.AreEqual("SecurityGroupAdded", logEventElement.Element("Name").Value);
            Assert.AreEqual(then.ToString("s", CultureInfo.InvariantCulture), DateTime.Parse(logEventElement.Element("Stamp").Value).ToString("s", CultureInfo.InvariantCulture));

            Assert.AreEqual("Phogue", logEventElement.Descendants("AccountModel").First().Element("Username").Value);
        }
    }
}