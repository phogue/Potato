#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Core.Variables;
using Procon.Net.Shared.Utils;

#endregion

namespace Procon.Core.Test.Variables {
    [TestFixture]
    public class TestVariableController {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();

            if (File.Exists(ConfigFileInfo.FullName)) {
                File.Delete(ConfigFileInfo.FullName);
            }
        }

        protected static FileInfo ConfigFileInfo = new FileInfo("Procon.Core.Test.Variables.xml");

        /// <summary>
        ///     Tests that an empty namespace will result in the common VariableModel name being returned.
        /// </summary>
        [Test]
        public void TestBuildNamespaceVariableKeyEmptyNamespace() {
            Assert.AreEqual("CommandServerEnabled", VariableModel.NamespaceVariableName("", CommonVariableNames.CommandServerEnabled));
        }

        /// <summary>
        ///     Tests that a key is generated correctly with a dot seperator when a
        ///     namespace is passed in.
        /// </summary>
        [Test]
        public void TestBuildNamespaceVariableKeyWithNamespace() {
            Assert.AreEqual("my.namespace.CommandServerEnabled", VariableModel.NamespaceVariableName("my.namespace", CommonVariableNames.CommandServerEnabled));
        }

        /// <summary>
        ///     Tests that when disposing of the variables object, all other items are cleaned up.
        /// </summary>
        [Test]
        public void TestDispose() {
            var variables = new VariableController();

            variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "VolatileKey",
                    "value"
                })
            });

            variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSetA,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "ArchiveKey",
                    "value"
                })
            });

            VariableModel volatileVariable = variables.VolatileVariables.First();

            variables.Dispose();

            // Test that all the lists and data within each item has been nulled.
            Assert.IsNull(variables.VolatileVariables);

            Assert.IsNull(volatileVariable.Name);
            Assert.IsNull(volatileVariable.Value);
        }

        /// <summary>
        ///     Tests that a VariableModel will be created and added if it does not exist
        ///     to begin with.
        /// </summary>
        [Test]
        public void TestDynamicCreationCommonVariableKey() {
            var variables = new VariableController();

            // Validate the VariableModel does not exist first.
            Assert.IsNull(variables.VolatileVariables.Find(v => v.Name == CommonVariableNames.TextCommandPrivatePrefix.ToString()));

            // Fetch the VariableModel. This should create and add the VariableModel.
            variables.Variable(CommonVariableNames.TextCommandPrivatePrefix);

            // Now validate that the VariableModel has been added.
            Assert.IsNotNull(variables.VolatileVariables.Find(v => v.Name == CommonVariableNames.TextCommandPrivatePrefix.ToString()));
        }

        /// <summary>
        ///     Tests that a VariableModel will be created and added if it does not exist
        ///     to begin with.
        /// </summary>
        [Test]
        public void TestDynamicCreationStringKey() {
            String key = "TestDynamicCreation" + StringExtensions.RandomString(20);

            var variables = new VariableController();

            // Validate the VariableModel does not exist first.
            Assert.IsNull(variables.VolatileVariables.Find(v => v.Name == key));

            // Fetch the VariableModel. This should create and add the VariableModel.
            variables.Variable(key);

            // Now validate that the VariableModel has been added.
            Assert.IsNotNull(variables.VolatileVariables.Find(v => v.Name == key));
        }

        /// <summary>
        ///     Tests that a config can be successfully loaded
        /// </summary>
        [Test]
        public void TestLoadConfig() {
            var saveVariables = (VariableController)new VariableController().Execute();
            saveVariables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSetA,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "KeyToWriteString",
                    "this is a string"
                })
            });

            saveVariables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSetA,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "KeyToWriteInteger",
                    1
                })
            });

            saveVariables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "KeyToNotWrite",
                    "This shouldn't appear in the saved file."
                })
            });

            saveVariables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSetA,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonVariableNames.MaximumProtocolConnections,
                    10
                })
            });

            // Save a config of the variables controller
            var saveConfig = new Config();
            saveConfig.Create(typeof (VariableController));
            saveVariables.WriteConfig(saveConfig);
            saveConfig.Save(ConfigFileInfo);

            // Load the config in a new config.
            var loadVariables = (VariableController)new VariableController().Execute();
            var loadConfig = new Config();
            loadConfig.Load(ConfigFileInfo);
            loadVariables.Execute(loadConfig);

            Assert.AreEqual("this is a string", loadVariables.ArchiveVariables.First(v => v.Name == "KeyToWriteString").ToType<String>());
            Assert.AreEqual(1, loadVariables.ArchiveVariables.First(v => v.Name == "KeyToWriteInteger").ToType<int>());
            Assert.AreEqual(10, loadVariables.ArchiveVariables.First(v => v.Name == "MaximumProtocolConnections").ToType<int>());
        }

        /// <summary>
        ///     Tests that a config can be written in a specific format.
        /// </summary>
        [Test]
        public void TestWriteConfig() {
            var variables = (VariableController)new VariableController().Execute();
            variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSetA,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "NameToWriteString",
                    "this is a string"
                })
            });

            variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSetA,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "NameToWriteInteger",
                    1
                })
            });

            variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "NameToNotWrite",
                    "This shouldn't appear in the saved file."
                })
            });

            variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSetA,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonVariableNames.MaximumProtocolConnections,
                    10
                })
            });

            // Save a config of the variables controller
            var saveConfig = new Config();
            saveConfig.Create(typeof (VariableController));
            variables.WriteConfig(saveConfig);
            saveConfig.Save(ConfigFileInfo);

            // Load the config in a new config.
            var loadConfig = new Config();
            loadConfig.Load(ConfigFileInfo);

            var commands = loadConfig.RootOf<VariableController>().Children<JObject>().Select(item => item.ToObject<Command>()).ToList();

            Assert.AreEqual("VariablesSetA", commands[0].Name);
            Assert.AreEqual("NameToWriteString", commands[0].Parameters[0].First<String>());
            Assert.AreEqual("this is a string", commands[0].Parameters[1].First<String>());

            Assert.AreEqual("VariablesSetA", commands[1].Name);
            Assert.AreEqual("NameToWriteInteger", commands[1].Parameters[0].First<String>());
            Assert.AreEqual("1", commands[1].Parameters[1].First<String>());

            Assert.AreEqual("VariablesSetA", commands[2].Name);
            Assert.AreEqual("MaximumProtocolConnections", commands[2].Parameters[0].First<String>());
            Assert.AreEqual("10", commands[2].Parameters[1].First<String>());
        }
    }
}