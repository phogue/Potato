#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;
using Potato.Core.Shared.Serialization;
using Potato.Core.Variables;
using Potato.Net.Shared.Utils;

#endregion

namespace Potato.Core.Test.Variables {
    [TestFixture]
    public class TestVariableController {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();

            if (File.Exists(ConfigFileInfo.FullName)) {
                File.Delete(ConfigFileInfo.FullName);
            }
        }

        protected static FileInfo ConfigFileInfo = new FileInfo("Potato.Core.Test.Variables.xml");

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
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "VolatileKey",
                    "value"
                })
            });

            variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSetA,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "ArchiveKey",
                    "value"
                })
            });

            var volatileVariable = variables.VolatileVariables.Values.First();

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
            Assert.IsNull(variables.VolatileVariables.Values.FirstOrDefault(v => v.Name == CommonVariableNames.TextCommandPrivatePrefix.ToString()));

            // Fetch the VariableModel. This should create and add the VariableModel.
            variables.Variable(CommonVariableNames.TextCommandPrivatePrefix);

            // Now validate that the VariableModel has been added.
            Assert.IsNotNull(variables.VolatileVariables.Values.First(v => v.Name == CommonVariableNames.TextCommandPrivatePrefix.ToString()));
        }

        /// <summary>
        ///     Tests that a VariableModel will be created and added if it does not exist
        ///     to begin with.
        /// </summary>
        [Test]
        public void TestDynamicCreationStringKey() {
            var key = "TestDynamicCreation" + StringExtensions.RandomString(20);

            var variables = new VariableController();

            // Validate the VariableModel does not exist first.
            Assert.IsNull(variables.VolatileVariables.Values.FirstOrDefault(v => v.Name == key));

            // Fetch the VariableModel. This should create and add the VariableModel.
            variables.Variable(key);

            // Now validate that the VariableModel has been added.
            Assert.IsNotNull(variables.VolatileVariables.Values.First(v => v.Name == key));
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
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "KeyToWriteString",
                    "this is a string"
                })
            });

            saveVariables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSetA,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "KeyToWriteInteger",
                    1
                })
            });

            saveVariables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "KeyToNotWrite",
                    "This shouldn't appear in the saved file."
                })
            });

            saveVariables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSetA,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
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

            Assert.AreEqual("this is a string", loadVariables.ArchiveVariables.Values.First(v => v.Name == "KeyToWriteString").ToType<string>());
            Assert.AreEqual(1, loadVariables.ArchiveVariables.Values.First(v => v.Name == "KeyToWriteInteger").ToType<int>());
            Assert.AreEqual(10, loadVariables.ArchiveVariables.Values.First(v => v.Name == "MaximumProtocolConnections").ToType<int>());
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
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "NameToWriteString",
                    "this is a string"
                })
            });

            variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSetA,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "NameToWriteInteger",
                    1
                })
            });

            // Empty strings should not be written. No point saving nothing.
            variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSetA,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "NameToignore",
                    ""
                })
            });

            variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "NameToNotWrite",
                    "This shouldn't appear in the saved file."
                })
            });

            variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSetA,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
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

            var commands = loadConfig.RootOf<VariableController>().Children<JObject>().Select(item => item.ToObject<IConfigCommand>(JsonSerialization.Minimal)).ToList();

            // Order is not maintained so we can only check that the values in some disorder are output.
            // Nope, not perfect.
            foreach (var command in commands) {
                Assert.AreEqual("VariablesSetA", command.Command.Name);
                Assert.IsTrue(new List<string>() { "NameToWriteString", "NameToWriteInteger", "MaximumProtocolConnections" }.Contains(command.Command.Parameters[0].First<string>()));
                Assert.IsTrue(new List<string>() { "this is a string", "1", "10" }.Contains(command.Command.Parameters[1].First<string>()));
            }
        }
    }
}