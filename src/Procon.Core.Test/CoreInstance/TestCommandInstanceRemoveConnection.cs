#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Shared;
using Procon.Net.Shared.Protocols;
using Procon.Service.Shared;

#endregion

namespace Procon.Core.Test.CoreInstance {
    [TestFixture]
    public class TestCommandInstanceRemoveConnection {
        [SetUp]
        public void Initialize() {
            if (File.Exists(ConfigFileInfo.FullName)) {
                File.Delete(ConfigFileInfo.FullName);
            }
        }

        protected static FileInfo ConfigFileInfo = new FileInfo(Path.Combine(Defines.ConfigsDirectory, "Procon.Core.xml"));

        /// <summary>
        ///     Tests that a connection can be removed.
        /// </summary>
        [Test]
        public void TestInstanceRemoveConnectionByGuidSuccess() {
            var instance = new Instance().Execute() as Instance;

            instance.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.InstanceAddConnection,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Myrcon",
                    CommonGameType.BF_3,
                    "93.186.198.11",
                    27516,
                    "phogueisabutterfly",
                    ""
                })
            });

            // Make sure we have at least one connection added.
            Assert.AreEqual(1, instance.Connections.Count);

            CommandResultArgs result = instance.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.InstanceRemoveConnection,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    instance.Connections.First().ConnectionModel.ConnectionGuid.ToString()
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual(0, instance.Connections.Count);
        }

        /// <summary>
        ///     Tests that a DoesNotExist error is returned when trying to remove
        ///     a connection on an empty instance object.
        /// </summary>
        [Test]
        public void TestInstanceRemoveConnectionDoesNotExist() {
            var instance = new Instance().Execute() as Instance;

            // Now readd the same connection we just added.
            CommandResultArgs result = instance.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.InstanceRemoveConnection,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Myrcon",
                    CommonGameType.BF_3,
                    "93.186.198.11",
                    27516
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.Status);
        }

        /// <summary>
        ///     Tests a remote command to remove a connection will fail if the username
        ///     supplied does not have permissions to add the connection.
        /// </summary>
        [Test]
        public void TestInstanceRemoveConnectionInsufficientPermissions() {
            var instance = new Instance().Execute() as Instance;

            CommandResultArgs result = instance.Tunnel(new Command() {
                Origin = CommandOrigin.Remote,
                Username = "Phogue",
                CommandType = CommandType.InstanceRemoveConnection,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Myrcon",
                    CommonGameType.BF_3,
                    "93.186.198.11",
                    27516
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
            Assert.AreEqual(0, instance.Connections.Count);
        }

        /// <summary>
        ///     Tests that a connection can be removed.
        /// </summary>
        [Test]
        public void TestInstanceRemoveConnectionSuccess() {
            var instance = new Instance().Execute() as Instance;

            instance.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.InstanceAddConnection,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Myrcon",
                    CommonGameType.BF_3,
                    "93.186.198.11",
                    27516,
                    "phogueisabutterfly",
                    ""
                })
            });

            // Make sure we have at least one connection added.
            Assert.AreEqual(1, instance.Connections.Count);

            CommandResultArgs result = instance.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.InstanceRemoveConnection,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Myrcon",
                    CommonGameType.BF_3,
                    "93.186.198.11",
                    27516
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual(0, instance.Connections.Count);
        }
    }
}