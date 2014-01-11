using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Remote;
using Procon.Core.Shared;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;

namespace Procon.Core.Test.Remote.TestCommandServerController {
    [TestFixture]
    public class TestConfigure {
        [SetUp]
        protected void SetUp() {
            SharedReferences.Setup();
        }

        /// <summary>
        /// Tests the listener can be setup if the port variable is set then the enabled variable.
        /// </summary>
        [Test]
        public void TestVariableEnabledPortThenEnabled() {
            var commandServer = new CommandServerController();

            commandServer.Shared.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonVariableNames.CommandServerPort,
                    3222
                })
            });

            commandServer.Shared.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonVariableNames.CommandServerEnabled,
                    true
                })
            });

            commandServer.Execute();
            
            Assert.IsNotNull(commandServer.CommandServerListener);
            Assert.IsNotNull(commandServer.CommandServerListener.Listener);

            commandServer.Dispose();
        }

        /// <summary>
        /// Tests the listener can be setup if the server is enabled, then the port is set.
        /// </summary>
        [Test]
        public void TestVariableEnabledEnabledThenPort() {
            var commandServer = new CommandServerController();

            commandServer.Shared.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonVariableNames.CommandServerEnabled,
                    true
                })
            });

            commandServer.Shared.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonVariableNames.CommandServerPort,
                    3222
                })
            });

            commandServer.Execute();

            Assert.IsNotNull(commandServer.CommandServerListener);
            Assert.IsNotNull(commandServer.CommandServerListener.Listener);

            commandServer.Dispose();
        }

        /// <summary>
        /// Tests that altering the command server enabled/disabled variable
        /// on an active listener will disable and null the listener.
        /// </summary>
        [Test]
        public void TestVariableDisabled() {
            var commandServer = new CommandServerController();

            commandServer.Shared.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonVariableNames.CommandServerPort,
                    3222
                })
            });

            commandServer.Shared.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonVariableNames.CommandServerEnabled,
                    true
                })
            });

            commandServer.Execute();

            commandServer.Shared.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonVariableNames.CommandServerEnabled,
                    false
                })
            });

            Assert.IsNull(commandServer.CommandServerListener);
            Assert.IsNotNull(commandServer.TunnelObjects);

            commandServer.Dispose();
        }

        /// <summary>
        /// Tests an event is logged when the command listener is started.
        /// </summary>
        [Test]
        public void TestEventLoggedOnConfiguredEnabled() {
            var commandServer = new CommandServerController();

            commandServer.Shared.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonVariableNames.CommandServerPort,
                    3222
                })
            });

            commandServer.Shared.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonVariableNames.CommandServerEnabled,
                    true
                })
            });

            commandServer.Execute();

            Assert.IsNotEmpty(commandServer.Shared.Events.LoggedEvents);
            Assert.AreEqual(GenericEventType.CommandServerStarted, commandServer.Shared.Events.LoggedEvents.First(e => e.GenericEventType == GenericEventType.CommandServerStarted).GenericEventType);
            Assert.AreEqual(CommandResultType.Success, commandServer.Shared.Events.LoggedEvents.First(e => e.GenericEventType == GenericEventType.CommandServerStarted).Status);
            Assert.IsTrue(commandServer.Shared.Events.LoggedEvents.First(e => e.GenericEventType == GenericEventType.CommandServerStarted).Success);

            commandServer.Dispose();
        }

        /// <summary>
        /// Tests an event is logged when the command listener is stopped.
        /// </summary>
        [Test]
        public void TestEventLoggedOnConfiguredDisabled() {
            var commandServer = new CommandServerController();

            commandServer.Shared.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonVariableNames.CommandServerPort,
                    3222
                })
            });

            commandServer.Shared.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonVariableNames.CommandServerEnabled,
                    true
                })
            });

            commandServer.Execute();

            commandServer.Shared.Variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonVariableNames.CommandServerEnabled,
                    false
                })
            });

            Assert.IsNotEmpty(commandServer.Shared.Events.LoggedEvents);
            Assert.AreEqual(GenericEventType.CommandServerStopped, commandServer.Shared.Events.LoggedEvents.First(e => e.GenericEventType == GenericEventType.CommandServerStopped).GenericEventType);
            Assert.AreEqual(CommandResultType.Success, commandServer.Shared.Events.LoggedEvents.First(e => e.GenericEventType == GenericEventType.CommandServerStarted).Status);
            Assert.IsTrue(commandServer.Shared.Events.LoggedEvents.First(e => e.GenericEventType == GenericEventType.CommandServerStarted).Success);

            commandServer.Dispose();
        }
    }
}
