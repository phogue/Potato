using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Events;
using Procon.Core.Localization;
using Procon.Core.Security;
using Procon.Core.Test.ExecutableCommands.Objects;
using Procon.Core.Test.Security;
using Procon.Core.Utils;
using Procon.Core.Variables;
using Procon.Net.Protocols;
using Procon.Net.Utils;

namespace Procon.Core.Test {
    [TestClass]
    public class TestInstance {

        protected static FileInfo ConfigFileInfo = new FileInfo(Path.Combine(Defines.ConfigsDirectory, "Procon.Core.xml"));

        [TestInitialize]
        public void Initialize() {
            if (File.Exists(ConfigFileInfo.FullName)) {
                File.Delete(ConfigFileInfo.FullName);
            }
        }

        /// <summary>
        /// Tests that providing no connection scope will bubble the command over all
        /// executable objects in the instance. The variable should be set.
        /// </summary>
        [TestMethod]
        public void TestInstanceCommandScopeNoScope() {
            VariableController variables = new VariableController();

            Instance instance = new Instance() {
                Variables = variables,
                Security = new SecurityController(),
                Events = new EventsController(),
                Languages = new LanguageController()
            }.Execute() as Instance;

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

            // Tests that there is at least one connection.
            Assert.AreEqual(1, instance.Connections.Count);

            CommandResultArgs result = instance.Execute(new Command() {
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
        }

        /// <summary>
        /// Tests that variable will not set on the instance as it will
        /// bypass the instance executable objects and execute only on the connection.
        /// </summary>
        [TestMethod]
        public void TestInstanceCommandScopeWithConnectionScope() {
            VariableController variables = new VariableController();

            Instance instance = new Instance() {
                Variables = variables,
                Security = new SecurityController(),
                Events = new EventsController(),
                Languages = new LanguageController()
            }.Execute() as Instance;

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

            // Tests that there is at least one connection.
            Assert.AreEqual(1, instance.Connections.Count);

            CommandResultArgs result = instance.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Scope = {
                    ConnectionGuid = instance.Connections.First().ConnectionGuid
                },
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "key",
                    "value"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Continue, result.Status);
            Assert.AreEqual("default value", variables.Get("key", "default value"));
        }

        /// <summary>
        /// Tests that everything is nulled after disposing.
        /// </summary>
        /// <remarks>The controllers have their own individual dispose methods that are tested.</remarks>
        [TestMethod]
        public void TestInstanceDispose() {
            AutoResetEvent requestWait = new AutoResetEvent(false);

            Instance instance = new Instance() {
                Variables = new VariableController(),
                Security = new SecurityController(),
                Events = new EventsController(),
                Languages = new LanguageController()
            }.Execute() as Instance;

            // Add a single connection, just so we can validate that it has been removed.
            instance.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.InstanceAddConnection,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonGameType.BF_3,
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
            Assert.IsNull(instance.Variables);
            Assert.IsNull(instance.Security);
            Assert.IsNull(instance.Events);
            Assert.IsNull(instance.Languages);
            Assert.IsNull(instance.Daemon);
            Assert.IsNull(instance.Connections);
            Assert.IsNull(instance.Packages);
            Assert.IsNull(instance.Tasks);
            Assert.IsNull(instance.PushEvents);
        }

        /// <summary>
        /// Tests the integrity of the config written by the instance.
        /// </summary>
        /// <remarks>We test individual controllers configs in other unit tests.</remarks>
        [TestMethod]
        public void TestInstanceConfigWritten() {
            Instance instance = new Instance() {
                Variables = new VariableController().Execute() as VariableController,
                Security = new SecurityController().Execute() as SecurityController,
                Events = new EventsController().Execute() as EventsController,
                Languages = new LanguageController().Execute() as LanguageController
            }.Execute() as Instance;

            // Add a single connection, just so we can validate that it has been removed.
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

            instance.WriteConfig();

            Config loadConfig = new Config();
            loadConfig.LoadFile(TestInstance.ConfigFileInfo);

            var commands = loadConfig.Root.Descendants("Instance").Elements("Command").Select(xCommand => xCommand.FromXElement<Command>()).ToList();

            Assert.AreEqual<String>("InstanceAddConnection", commands[0].Name);
            Assert.AreEqual<String>("Myrcon", commands[0].Parameters[0].First<String>());
            Assert.AreEqual<String>("BF_3", commands[0].Parameters[1].First<String>());
            Assert.AreEqual<String>("93.186.198.11", commands[0].Parameters[2].First<String>());
            Assert.AreEqual<String>("27516", commands[0].Parameters[3].First<String>());
            Assert.AreEqual<String>("phogueisabutterfly", commands[0].Parameters[4].First<String>());
            Assert.AreEqual<String>("", commands[0].Parameters[5].First<String>());
        }
    }
}
