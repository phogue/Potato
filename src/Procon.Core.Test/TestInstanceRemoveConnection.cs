using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Utils;
using Procon.Core.Variables;
using Procon.Net.Protocols;

namespace Procon.Core.Test {
    [TestClass]
    public class TestInstanceRemoveConnection {

        protected static FileInfo ConfigFileInfo = new FileInfo(Path.Combine(Defines.ConfigsDirectory, "Procon.Core.xml"));

        [TestInitialize]
        public void Initialize() {
            if (File.Exists(ConfigFileInfo.FullName)) {
                File.Delete(ConfigFileInfo.FullName);
            }
        }

        /// <summary>
        /// Tests that a connection can be removed.
        /// </summary>
        [TestMethod]
        public void TestInstanceRemoveConnectionSuccess() {
            Instance instance = new Instance().Execute() as Instance;

            instance.Execute(new Command() {
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

            CommandResultArgs result = instance.Execute(new Command() {
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

        /// <summary>
        /// Tests that a connection can be removed.
        /// </summary>
        [TestMethod]
        public void TestInstanceRemoveConnectionByGuidSuccess() {
            Instance instance = new Instance().Execute() as Instance;

            instance.Execute(new Command() {
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

            CommandResultArgs result = instance.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.InstanceRemoveConnection,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    instance.Connections.First().ConnectionGuid.ToString()
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual(0, instance.Connections.Count);
        }

        /// <summary>
        /// Tests a remote command to remove a connection will fail if the username
        /// supplied does not have permissions to add the connection.
        /// </summary>
        [TestMethod]
        public void TestInstanceRemoveConnectionInsufficientPermissions() {
            Instance instance = new Instance().Execute() as Instance;

            CommandResultArgs result = instance.Execute(new Command() {
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
        /// Tests that a DoesNotExist error is returned when trying to remove
        /// a connection on an empty instance object.
        /// </summary>
        [TestMethod]
        public void TestInstanceRemoveConnectionDoesNotExist() {
            Instance instance = new Instance().Execute() as Instance;

            // Now readd the same connection we just added.
            CommandResultArgs result = instance.Execute(new Command() {
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
    }
}
