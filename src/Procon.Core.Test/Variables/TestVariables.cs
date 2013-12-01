using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Procon.Core.Utils;

namespace Procon.Core.Test.Variables {
    using Procon.Core.Variables;
    using Procon.Net.Utils;

    [TestFixture]
    public class TestVariables {

        protected static FileInfo ConfigFileInfo = new FileInfo("Procon.Core.Test.Variables.xml");

        [SetUp]
        public void Initialize() {
            if (File.Exists(ConfigFileInfo.FullName)) {
                File.Delete(ConfigFileInfo.FullName);
            }
        }

        /// <summary>
        /// Tests that a config can be written in a specific format.
        /// </summary>
        [Test]
        public void TestVariablesWriteConfig() {
            VariableController variables = new VariableController().Execute() as VariableController;
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
                    CommonVariableNames.MaximumGameConnections,
                    10
                })
            });

            // Save a config of the variables controller
            Config saveConfig = new Config();
            saveConfig.Create(typeof(Procon.Core.Variables.VariableController));
            variables.WriteConfig(saveConfig);
            saveConfig.Save(TestVariables.ConfigFileInfo);

            // Load the config in a new config.
            Config loadConfig = new Config();
            loadConfig.Load(TestVariables.ConfigFileInfo);

            var commands = loadConfig.Root.Descendants("VariableController").Elements("Command").Select(xCommand => xCommand.FromXElement<Command>()).ToList();

            Assert.AreEqual("VariablesSetA", commands[0].Name);
            Assert.AreEqual("NameToWriteString", commands[0].Parameters[0].First<String>());
            Assert.AreEqual("this is a string", commands[0].Parameters[1].First<String>());

            Assert.AreEqual("VariablesSetA", commands[1].Name);
            Assert.AreEqual("NameToWriteInteger", commands[1].Parameters[0].First<String>());
            Assert.AreEqual("1", commands[1].Parameters[1].First<String>());

            Assert.AreEqual("VariablesSetA", commands[2].Name);
            Assert.AreEqual("MaximumGameConnections", commands[2].Parameters[0].First<String>());
            Assert.AreEqual("10", commands[2].Parameters[1].First<String>());
        }

        /// <summary>
        /// Tests that a config can be successfully loaded 
        /// </summary>
        [Test]
        public void TestVariablesLoadConfig() {
            VariableController saveVariables = new VariableController().Execute() as VariableController;
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
                    CommonVariableNames.MaximumGameConnections,
                    10
                })
            });

            // Save a config of the variables controller
            Config saveConfig = new Config();
            saveConfig.Create(typeof(Procon.Core.Variables.VariableController));
            saveVariables.WriteConfig(saveConfig);
            saveConfig.Save(TestVariables.ConfigFileInfo);

            // Load the config in a new config.
            VariableController loadVariables = new VariableController().Execute() as VariableController;
            Config loadConfig = new Config();
            loadConfig.Load(TestVariables.ConfigFileInfo);
            loadVariables.Execute(loadConfig);

            Assert.AreEqual("this is a string", loadVariables.GetA<String>("KeyToWriteString"));
            Assert.AreEqual(1, loadVariables.GetA<int>("KeyToWriteInteger"));
            Assert.AreEqual(10, loadVariables.GetA<int>("MaximumGameConnections"));
        }

        /// <summary>
        /// Tests that when disposing of the variables object, all other items are cleaned up.
        /// </summary>
        [Test]
        public void TestVariablesDispose() {
            VariableController variables = new VariableController();

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

            Variable volatileVariable = variables.VolatileVariables.First();

            variables.Dispose();

            // Test that all the lists and data within each item has been nulled.
            Assert.IsNull(variables.VolatileVariables);

            Assert.IsNull(volatileVariable.Name);
            Assert.IsNull(volatileVariable.Value);
        }

        /// <summary>
        /// Tests that a variable will be created and added if it does not exist
        /// to begin with.
        /// </summary>
        [Test]
        public void TestVariablesDynamicCreationStringKey() {
            String key = "TestVariablesDynamicCreation" + StringExtensions.RandomString(20);

            VariableController variables = new VariableController();

            // Validate the variable does not exist first.
            Assert.IsNull(variables.VolatileVariables.Find(v => v.Name == key));

            // Fetch the variable. This should create and add the variable.
            Variable variable = variables.Variable(key);

            // Now validate that the variable has been added.
            Assert.IsNotNull(variables.VolatileVariables.Find(v => v.Name == key));
        }

        /// <summary>
        /// Tests that a variable will be created and added if it does not exist
        /// to begin with.
        /// </summary>
        [Test]
        public void TestVariablesDynamicCreationCommonVariableKey() {
            VariableController variables = new VariableController();

            // Validate the variable does not exist first.
            Assert.IsNull(variables.VolatileVariables.Find(v => v.Name == CommonVariableNames.TextCommandPrivatePrefix.ToString()));

            // Fetch the variable. This should create and add the variable.
            Variable variable = variables.Variable(CommonVariableNames.TextCommandPrivatePrefix);

            // Now validate that the variable has been added.
            Assert.IsNotNull(variables.VolatileVariables.Find(v => v.Name == CommonVariableNames.TextCommandPrivatePrefix.ToString()));
        }

        /// <summary>
        /// Tests that an empty namespace will result in the common variable name being returned.
        /// </summary>
        [Test]
        public void TestVariablesBuildNamespaceVariableKeyEmptyNamespace() {
            Assert.AreEqual("DaemonEnabled", Variable.NamespaceVariableName("", CommonVariableNames.DaemonEnabled));
        }

        /// <summary>
        /// Tests that a key is generated correctly with a dot seperator when a
        /// namespace is passed in.
        /// </summary>
        [Test]
        public void TestVariablesBuildNamespaceVariableKeyWithNamespace() {
            Assert.AreEqual("my.namespace.DaemonEnabled", Variable.NamespaceVariableName("my.namespace", CommonVariableNames.DaemonEnabled));
        }
        
    }
}
