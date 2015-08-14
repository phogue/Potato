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
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Potato.Core.Security;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;
using Potato.Core.Variables;

namespace Potato.Core.Test.Variables {
    [TestFixture]
    public class TestVariablesSet {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        ///     Tests that we can set a value, getting the successful flag back from the command.
        /// </summary>
        [Test]
        public void TestValue() {
            var variables = new VariableController();

            var result = variables.Tunnel(CommandBuilder.VariablesSet("key", "value").SetOrigin(CommandOrigin.Local));

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
            Assert.AreEqual("value", variables.Get("key", string.Empty));
        }

        /// <summary>
        ///     Tests that setting a VariableModel will fetch/set as case insensitive
        /// </summary>
        [Test]
        public void TestCaseInsensitive() {
            var variables = new VariableController();

            var result = variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "key",
                    "TestVariablesSetValueCaseInsensitive"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
            Assert.AreEqual("TestVariablesSetValueCaseInsensitive", variables.Get("Key", string.Empty));
            Assert.AreEqual("TestVariablesSetValueCaseInsensitive", variables.Get("KEY", string.Empty));
            Assert.AreEqual("TestVariablesSetValueCaseInsensitive", variables.Get("keY", string.Empty));
            Assert.AreEqual("TestVariablesSetValueCaseInsensitive", variables.Get("Key", string.Empty));
        }

        /// <summary>
        ///     Tests that an empty key will result in a invalid parameter.
        /// </summary>
        [Test]
        public void TestEmptyKeyValue() {
            var variables = new VariableController();

            // Set the value of a empty key
            var result = variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    string.Empty,
                    "value"
                })
            });

            // Validate that the command failed (can't have an empty key)
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InvalidParameter, result.CommandResultType);
            Assert.AreEqual(0, variables.VolatileVariables.Count);
        }

        /// <summary>
        ///     Tests that setting a VariableModel will fail if the user does not have permission.
        /// </summary>
        [Test]
        public void TestInsufficientPermission() {
            var variables = new VariableController() {
                Shared = {
                    Security = new SecurityController().Execute() as SecurityController
                }
            };

            var result = variables.Tunnel(new Command() {
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Remote,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "key",
                    "value"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }

        /// <summary>
        ///     Tests that setting an existing VariableModel will succeed (different code branch because it does not need to be added first)
        /// </summary>
        [Test]
        public void TestOverrideExisting() {
            var variables = new VariableController();

            variables.VolatileVariables.TryAdd("key", new VariableModel() {
                Name = "key",
                Value = "initial value"
            });

            // Set the value of a empty key
            var result = variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "key",
                    "modified value"
                })
            });

            // Validate that the command failed (can't have an empty key)
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
            Assert.AreEqual("modified value", variables.Get("key", string.Empty));
        }

        /// <summary>
        ///     Tests setting a read only VariableModel returns an error and the VariableModel remains unchanged.
        /// </summary>
        [Test]
        public void TestReadOnly() {
            var variables = new VariableController();

            variables.VolatileVariables.TryAdd("key", new VariableModel() {
                Name = "key",
                Value = "value",
                Readonly = true
            });

            // Set the value of a empty key
            var result = variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "key",
                    "modified value"
                })
            });

            // Validate that the command failed (can't have an empty key)
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.Failed, result.CommandResultType);
            Assert.AreEqual("value", variables.Get("key", string.Empty));
        }

        /// <summary>
        ///     Tests that we can set a VariableModel to a list of strings via a command.
        /// </summary>
        [Test]
        public void TestValueStringList() {
            var variables = new VariableController();

            // Set the value of a empty key
            var result = variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                "key"
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                "value1",
                                "value2"
                            }
                        }
                    }
                }
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
            Assert.IsNotNull(variables.Get<List<string>>("key"));
            Assert.AreEqual("value1", variables.Get<List<string>>("key").First());
            Assert.AreEqual("value2", variables.Get<List<string>>("key").Last());
        }
    }
}