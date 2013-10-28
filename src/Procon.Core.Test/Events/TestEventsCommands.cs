using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Events;
using Procon.Core.Security;

namespace Procon.Core.Test.Events {
    [TestClass]
    public class TestEventsCommands {
        /// <summary>
        /// Tests that an event will be returned if it is after a specific ID.
        /// </summary>
        [TestMethod]
        public void TestEventsAfterEventIdSuccess() {
            EventsController events = new EventsController();

            events.Log(new GenericEventArgs() {
                Success = true,
                GenericEventType = GenericEventType.SecurityGroupAdded,
                Scope = new CommandData() {
                    Accounts = new List<Account>() {
                        new Account() {
                            Username = "Phogue"
                        }
                    }
                }
            });

            CommandResultArgs result = events.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.EventsFetchAfterEventId,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    0
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual<CommandResultType>(CommandResultType.Success, result.Status);
            Assert.AreEqual(1, result.Now.Events.Count);
            Assert.AreEqual("Phogue", result.Now.Events.First().Scope.Accounts.First().Username);
        }

        /// <summary>
        /// Tests that an event will be returned if it is after a specific ID but
        /// not if it has expired.
        /// </summary>
        [TestMethod]
        public void TestEventsAfterEventIdExcludingExpired() {
            EventsController events = new EventsController();

            events.Log(new GenericEventArgs() {
                Success = true,
                GenericEventType = GenericEventType.SecurityGroupAdded,
                Scope = new CommandData() {
                    Accounts = new List<Account>() {
                        new Account() {
                            Username = "Phogue"
                        }
                    }
                }
            });

            events.Log(new GenericEventArgs() {
                Success = true,
                GenericEventType = GenericEventType.SecurityGroupAdded,
                Scope = new CommandData() {
                    Accounts = new List<Account>() {
                        new Account() {
                            Username = "Zaeed"
                        }
                    }
                },
                Stamp = DateTime.Now.AddHours(-1)
            });

            CommandResultArgs result = events.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.EventsFetchAfterEventId,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    0
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual<CommandResultType>(CommandResultType.Success, result.Status);
            Assert.AreEqual(1, result.Now.Events.Count);
            Assert.AreEqual("Phogue", result.Now.Events.First().Scope.Accounts.First().Username);
        }

        /// <summary>
        /// Tests that fetching events after an id without the permission
        /// to do so will result in an insufficient error being returned.
        /// </summary>
        [TestMethod]
        public void TestEventsAfterEventIdInsufficientPermission() {
            EventsController events = new EventsController();

            events.Log(new GenericEventArgs() {
                Success = true,
                GenericEventType = GenericEventType.SecurityGroupAdded,
                Scope = new CommandData() {
                    Accounts = new List<Account>() {
                        new Account() {
                            Username = "Phogue"
                        }
                    }
                }
            });

            CommandResultArgs result = events.Execute(new Command() {
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                CommandType = CommandType.EventsFetchAfterEventId,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    0
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual<CommandResultType>(CommandResultType.InsufficientPermissions, result.Status);
        }
    }
}
