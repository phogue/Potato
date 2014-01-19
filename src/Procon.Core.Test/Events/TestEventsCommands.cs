#region

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Events;
using Procon.Core.Shared;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;

#endregion

namespace Procon.Core.Test.Events {
    [TestFixture]
    public class TestEventsCommands {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        ///     Tests that an event will be returned if it is after a specific ID but
        ///     not if it has expired.
        /// </summary>
        [Test]
        public void TestEventsAfterEventIdExcludingExpired() {
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
                }
            });

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
                Stamp = DateTime.Now.AddHours(-1)
            });

            ICommandResult result = events.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.EventsFetchAfterEventId,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    0
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual(1, result.Now.Events.Count);
            Assert.AreEqual("Phogue", result.Now.Events.First().Scope.Accounts.First().Username);
        }

        /// <summary>
        ///     Tests that fetching events after an id without the permission
        ///     to do so will result in an insufficient error being returned.
        /// </summary>
        [Test]
        public void TestEventsAfterEventIdInsufficientPermission() {
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
                }
            });

            ICommandResult result = events.Tunnel(new Command() {
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Remote,
                CommandType = CommandType.EventsFetchAfterEventId,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    0
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        ///     Tests that an event will be returned if it is after a specific ID.
        /// </summary>
        [Test]
        public void TestEventsAfterEventIdSuccess() {
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
                }
            });

            ICommandResult result = events.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.EventsFetchAfterEventId,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    0
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual(1, result.Now.Events.Count);
            Assert.AreEqual("Phogue", result.Now.Events.First().Scope.Accounts.First().Username);
        }
    }
}