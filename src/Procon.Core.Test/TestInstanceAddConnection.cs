using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using Procon.Core.Variables;
using Procon.Net.Protocols;
using Procon.Service.Shared;

namespace Procon.Core.Test {
    [TestFixture]
    public class TestInstanceAddConnection {

        protected static FileInfo ConfigFileInfo = new FileInfo(Path.Combine(Defines.ConfigsDirectory, "Procon.Core.xml"));

        [SetUp]
        public void Initialize() {
            if (File.Exists(ConfigFileInfo.FullName)) {
                File.Delete(ConfigFileInfo.FullName);
            }
        }

        /// <summary>
        /// Tests that a connection can be added.
        /// </summary>
        [Test]
        public void TestInstanceAddConnectionSuccess() {
            Instance instance = new Instance().Execute() as Instance;

            CommandResultArgs result = instance.Execute(new Command() {
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

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual(1, instance.Connections.Count);
        }

        /// <summary>
        /// Tests a remote command to add a connection will fail if the username
        /// supplied does not have permissions to add the connection.
        /// </summary>
        [Test]
        public void TestInstanceAddConnectionInsufficientPermissions() {
            Instance instance = new Instance().Execute() as Instance;

            CommandResultArgs result = instance.Execute(new Command() {
                Origin = CommandOrigin.Remote,
                Username = "Phogue",
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

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
            Assert.AreEqual(0, instance.Connections.Count);
        }

        /// <summary>
        /// Tests we receive a DoesNotExist status when a game type is not supported (or exist..)
        /// </summary>
        [Test]
        public void TestInstanceAddConnectionGameTypeDoesNotExist() {
            Instance instance = new Instance().Execute() as Instance;

            CommandResultArgs result = instance.Execute(new Command() {
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
        }

        /// <summary>
        /// Tests that we cannot add the same connection twice.
        /// </summary>
        [Test]
        public void TestInstanceAddConnectionDuplicate() {
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

            // Make sure the initial connection was added successfully.
            Assert.AreEqual(1, instance.Connections.Count);

            // Now readd the same connection we just added.
            CommandResultArgs result = instance.Execute(new Command() {
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

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.AlreadyExists, result.Status);
        }

        /// <summary>
        /// Tests that a connection cannot be added if would go over the maximum connection limit
        /// imposed by a variable.
        /// </summary>
        [Test]
        public void TestInstanceAddConnectionExceedMaximumConnectionLimit() {
            VariableController variables = new VariableController();
            Instance instance = new Instance() {
                Variables = variables
            }.Execute() as Instance;

            // Lower the maximum connections to nothing
            variables.Set(new Command() {
                Origin = CommandOrigin.Local
            }, CommonVariableNames.MaximumGameConnections, 0);

            CommandResultArgs result = instance.Execute(new Command() {
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

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.LimitExceeded, result.Status);
            Assert.AreEqual(0, instance.Connections.Count);
        }
    }
}
