#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using Procon.Core.Events;
using Procon.Core.Localization;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Serialization;
using Procon.Core.Test.Mocks.Protocols;
using Procon.Core.Variables;
using Procon.Net.Shared.Protocols;
using Procon.Net.Shared.Utils;
using Procon.Service.Shared;

#endregion

namespace Procon.Core.Test.CoreInstance {
    [TestFixture]
    public class TestInstance {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();

            SupportedGameTypes.GetSupportedGames(new List<Assembly>() { typeof(MockProtocol).Assembly });

            if (File.Exists(ConfigFileInfo.FullName)) {
                File.Delete(ConfigFileInfo.FullName);
            }
        }

        protected static FileInfo ConfigFileInfo = new FileInfo(Path.Combine(Defines.ConfigsDirectory.FullName, "Procon.Core.json"));

        /// <summary>
        ///     Tests that providing no connection scope will tunnel the command over all
        ///     executable objects in the instance. The VariableModel should be set.
        /// </summary>
        [Test]
        public void TestInstanceCommandScopeNoScope() {
            var variables = new VariableController();

            var instance = new InstanceController() {
                Shared = {
                    Variables = variables,
                    Security = new SecurityController(),
                    Events = new EventsController(),
                    Languages = new LanguageController()
                }
            }.Execute() as InstanceController;

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

            // Tests that there is at least one connection.
            Assert.AreEqual(1, instance.Connections.Count);

            ICommandResult result = instance.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "key",
                    "value"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual("value", variables.Get("key", "default value"));

            instance.Dispose();
        }

        /// <summary>
        ///     Tests that VariableModel will not set on the instance as it will
        ///     bypass the instance executable objects and execute only on the connection.
        /// </summary>
        [Test]
        public void TestInstanceCommandScopeWithConnectionScope() {
            var variables = new VariableController();

            var instance = new InstanceController() {
                Shared = {
                    Variables = variables,
                    Security = new SecurityController(),
                    Events = new EventsController(),
                    Languages = new LanguageController()
                }
            }.Execute() as InstanceController;

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

            // Tests that there is at least one connection.
            Assert.AreEqual(1, instance.Connections.Count);

            ICommandResult result = instance.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                ScopeModel = {
                    ConnectionGuid = instance.Connections.First().ConnectionModel.ConnectionGuid
                },
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "key",
                    "value"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Continue, result.Status);
            Assert.AreEqual("default value", variables.Get("key", "default value"));

            instance.Dispose();
        }

        /// <summary>
        ///     Tests the integrity of the config written by the instance.
        /// </summary>
        /// <remarks>We test individual controllers configs in other unit tests.</remarks>
        [Test]
        public void TestInstanceConfigWritten() {
            var instance = new InstanceController() {
                Shared = {
                    Variables = new VariableController().Execute() as VariableController,
                    Security = new SecurityController().Execute() as SecurityController,
                    Events = new EventsController().Execute() as EventsController,
                    Languages = new LanguageController().Execute() as LanguageController
                }
            }.Execute() as InstanceController;

            // Add a single connection, just so we can validate that it has been removed.
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

            instance.WriteConfig();

            var loadConfig = new Config();
            loadConfig.Load(ConfigFileInfo);
            // .Where(item => item.Name == "InstanceAddConnection")
            var commands = loadConfig.RootOf<InstanceController>().Children<JObject>().Select(item => item.ToObject<Command>(JsonSerialization.Minimal)).ToList();

            Assert.AreEqual("InstanceAddConnection", commands[0].Name);
            Assert.AreEqual("Myrcon", commands[0].Parameters[0].First<String>());
            Assert.AreEqual("MockProtocol", commands[0].Parameters[1].First<String>());
            Assert.AreEqual("93.186.198.11", commands[0].Parameters[2].First<String>());
            Assert.AreEqual("27516", commands[0].Parameters[3].First<String>());
            Assert.AreEqual("phogueisabutterfly", commands[0].Parameters[4].First<String>());
            Assert.AreEqual("", commands[0].Parameters[5].First<String>());

            instance.Dispose();
        }

        /// <summary>
        ///     Tests that everything is nulled after disposing.
        /// </summary>
        /// <remarks>The controllers have their own individual dispose methods that are tested.</remarks>
        [Test]
        public void TestInstanceDispose() {
            var requestWait = new AutoResetEvent(false);

            var instance = new InstanceController() {
                Shared = {
                    Variables = new VariableController(),
                    Security = new SecurityController(),
                    Events = new EventsController(),
                    Languages = new LanguageController()
                }
            }.Execute() as InstanceController;

            // Add a single connection, just so we can validate that it has been removed.
            instance.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.InstanceAddConnection,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "MockProtocol",
                    "93.186.198.11",
                    27516,
                    "phogueisabutterfly",
                    ""
                })
            });

            instance.Disposed += (sender, args) => requestWait.Set();

            instance.Dispose();

            Assert.IsTrue(requestWait.WaitOne(60000));

            // Now validate everything is nulled.
            // We test if each object has been disposed of with its own unit test elsewhere.
            Assert.IsNull(instance.Shared.Variables);
            Assert.IsNull(instance.Shared.Security);
            Assert.IsNull(instance.Shared.Events);
            Assert.IsNull(instance.Shared.Languages);
            Assert.IsNull(instance.CommandServer);
            Assert.IsNull(instance.Connections);
            Assert.IsNull(instance.Packages);
            Assert.IsNull(instance.Tasks);
            Assert.IsNull(instance.PushEvents);
        }
    }
}