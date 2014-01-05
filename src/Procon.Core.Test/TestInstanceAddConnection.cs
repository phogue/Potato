﻿#region

using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Core.Variables;
using Procon.Net.Shared.Protocols;
using Procon.Service.Shared;

#endregion

namespace Procon.Core.Test {
    [TestFixture]
    public class TestInstanceAddConnection {
        [SetUp]
        public void Initialize() {
            if (File.Exists(ConfigFileInfo.FullName)) {
                File.Delete(ConfigFileInfo.FullName);
            }
        }

        protected static FileInfo ConfigFileInfo = new FileInfo(Path.Combine(Defines.ConfigsDirectory, "Procon.Core.xml"));

        /// <summary>
        ///     Tests that we cannot add the same connection twice.
        /// </summary>
        [Test]
        public void TestInstanceAddConnectionDuplicate() {
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

            // Make sure the initial connection was added successfully.
            Assert.AreEqual(1, instance.Connections.Count);

            // Now readd the same connection we just added.
            CommandResultArgs result = instance.Tunnel(new Command() {
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
        ///     Tests that a connection cannot be added if would go over the maximum connection limit
        ///     imposed by a VariableModel.
        /// </summary>
        [Test]
        public void TestInstanceAddConnectionExceedMaximumConnectionLimit() {
            var variables = new VariableController();
            var instance = new Instance() {
                Shared = {
                    Variables = variables
                }
            }.Execute() as Instance;

            // Lower the maximum connections to nothing
            variables.Set(new Command() {
                Origin = CommandOrigin.Local
            }, CommonVariableNames.MaximumProtocolConnections, 0);

            CommandResultArgs result = instance.Tunnel(new Command() {
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

        /// <summary>
        ///     Tests we receive a DoesNotExist status when a game type is not supported (or exist..)
        /// </summary>
        [Test]
        public void TestInstanceAddConnectionGameTypeDoesNotExist() {
            var instance = new Instance().Execute() as Instance;

            CommandResultArgs result = instance.Tunnel(new Command() {
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
        ///     Tests a remote command to add a connection will fail if the username
        ///     supplied does not have permissions to add the connection.
        /// </summary>
        [Test]
        public void TestInstanceAddConnectionInsufficientPermissions() {
            var instance = new Instance().Execute() as Instance;

            CommandResultArgs result = instance.Tunnel(new Command() {
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
        ///     Tests that a connection can be added.
        /// </summary>
        [Test]
        public void TestInstanceAddConnectionSuccess() {
            var instance = new Instance().Execute() as Instance;

            CommandResultArgs result = instance.Tunnel(new Command() {
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
    }
}