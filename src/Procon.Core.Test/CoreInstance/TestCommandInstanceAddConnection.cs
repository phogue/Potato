#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Core.Test.Mocks.Protocols;
using Procon.Core.Variables;
using Procon.Net.Shared.Protocols;
using Procon.Service.Shared;

#endregion

namespace Procon.Core.Test.CoreInstance {
    [TestFixture]
    public class TestCommandInstanceAddConnection {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();

            SupportedGameTypes.GetSupportedGames(new List<Assembly>() { typeof(MockProtocol).Assembly });

            ConfigFileInfo.Refresh();
            if (File.Exists(ConfigFileInfo.FullName)) {
                File.Delete(ConfigFileInfo.FullName);
            }
        }

        protected static FileInfo ConfigFileInfo = new FileInfo(Path.Combine(Defines.ConfigsDirectory.FullName, "Procon.Core.json"));

        /// <summary>
        ///     Tests that we cannot add the same connection twice.
        /// </summary>
        [Test]
        public void TestInstanceAddConnectionDuplicate() {
            var instance = (InstanceController)new InstanceController().Execute();

            instance.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.InstanceAddConnection,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Myrcon",
                    "MockProtocol",
                    "93.186.198.11",
                    27516,
                    "phogueisabutterfly",
                    ""
                })
            });

            // Make sure the initial connection was added successfully.
            Assert.AreEqual(1, instance.Connections.Count);

            // Now readd the same connection we just added.
            CommandResult result = instance.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.InstanceAddConnection,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Myrcon",
                    "MockProtocol",
                    "93.186.198.11",
                    27516,
                    "phogueisabutterfly",
                    ""
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.AlreadyExists, result.Status);

            instance.Dispose();
        }

        /// <summary>
        ///     Tests that a connection cannot be added if would go over the maximum connection limit
        ///     imposed by a VariableModel.
        /// </summary>
        [Test]
        public void TestInstanceAddConnectionExceedMaximumConnectionLimit() {
            var variables = new VariableController();
            var instance = (InstanceController)new InstanceController() {
                Shared = {
                    Variables = variables
                }
            }.Execute();

            // Lower the maximum connections to nothing
            variables.Set(new Command() {
                Origin = CommandOrigin.Local
            }, CommonVariableNames.MaximumProtocolConnections, 0);

            CommandResult result = instance.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.InstanceAddConnection,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Myrcon",
                    "MockProtocol",
                    "93.186.198.11",
                    27516,
                    "phogueisabutterfly",
                    ""
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.LimitExceeded, result.Status);
            Assert.AreEqual(0, instance.Connections.Count);

            instance.Dispose();
        }

        /// <summary>
        ///     Tests we receive a DoesNotExist status when a game type is not supported (or exist..)
        /// </summary>
        [Test]
        public void TestInstanceAddConnectionGameTypeDoesNotExist() {
            var instance = (InstanceController)new InstanceController().Execute();

            CommandResult result = instance.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.InstanceAddConnection,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Myrcon",
                    "la la la",
                    "93.186.198.11",
                    27516,
                    "phogueisabutterfly",
                    ""
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.Status);

            instance.Dispose();
        }

        /// <summary>
        ///     Tests a remote command to add a connection will fail if the username
        ///     supplied does not have permissions to add the connection.
        /// </summary>
        [Test]
        public void TestInstanceAddConnectionInsufficientPermissions() {
            var instance = (InstanceController)new InstanceController().Execute();

            CommandResult result = instance.Tunnel(new Command() {
                Origin = CommandOrigin.Remote,
                Authentication = {
                    Username = "Phogue"
                },
                CommandType = CommandType.InstanceAddConnection,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Myrcon",
                    "MockProtocol",
                    "93.186.198.11",
                    27516,
                    "phogueisabutterfly",
                    ""
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
            Assert.AreEqual(0, instance.Connections.Count);

            instance.Dispose();
        }

        /// <summary>
        ///     Tests that a connection can be added.
        /// </summary>
        [Test]
        public void TestInstanceAddConnectionSuccess() {
            var instance = (InstanceController)new InstanceController().Execute();

            CommandResult result = instance.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.InstanceAddConnection,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Myrcon",
                    "MockProtocol",
                    "93.186.198.11",
                    27516,
                    "phogueisabutterfly",
                    ""
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual(1, instance.Connections.Count);

            instance.Dispose();
        }
    }
}