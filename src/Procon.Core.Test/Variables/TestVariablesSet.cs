using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Core.Variables;

namespace Procon.Core.Test.Variables {
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

            CommandResult result = variables.Tunnel(CommandBuilder.VariablesSet("key", "value").SetOrigin(CommandOrigin.Local));

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual("value", variables.Get("key", String.Empty));
        }

        /// <summary>
        ///     Tests that setting a VariableModel will fetch/set as case insensitive
        /// </summary>
        [Test]
        public void TestCaseInsensitive() {
            var variables = new VariableController();

            CommandResult result = variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "key",
                    "TestVariablesSetValueCaseInsensitive"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual("TestVariablesSetValueCaseInsensitive", variables.Get("Key", String.Empty));
            Assert.AreEqual("TestVariablesSetValueCaseInsensitive", variables.Get("KEY", String.Empty));
            Assert.AreEqual("TestVariablesSetValueCaseInsensitive", variables.Get("keY", String.Empty));
            Assert.AreEqual("TestVariablesSetValueCaseInsensitive", variables.Get("Key", String.Empty));
        }

        /// <summary>
        ///     Tests that an empty key will result in a invalid parameter.
        /// </summary>
        [Test]
        public void TestEmptyKeyValue() {
            var variables = new VariableController();

            // Set the value of a empty key
            CommandResult result = variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    String.Empty,
                    "value"
                })
            });

            // Validate that the command failed (can't have an empty key)
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InvalidParameter, result.Status);
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

            CommandResult result = variables.Tunnel(new Command() {
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "key",
                    "value"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        ///     Tests that setting an existing VariableModel will succeed (different code branch because it does not need to be added first)
        /// </summary>
        [Test]
        public void TestOverrideExisting() {
            var variables = new VariableController() {
                VolatileVariables = new List<VariableModel>() {
                    new VariableModel() {
                        Name = "key",
                        Value = "initial value"
                    }
                }
            };

            // Set the value of a empty key
            CommandResult result = variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "key",
                    "modified value"
                })
            });

            // Validate that the command failed (can't have an empty key)
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual("modified value", variables.Get("key", String.Empty));
        }

        /// <summary>
        ///     Tests setting a read only VariableModel returns an error and the VariableModel remains unchanged.
        /// </summary>
        [Test]
        public void TestReadOnly() {
            var variables = new VariableController() {
                VolatileVariables = new List<VariableModel>() {
                    new VariableModel() {
                        Name = "key",
                        Value = "value",
                        Readonly = true
                    }
                }
            };

            // Set the value of a empty key
            CommandResult result = variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "key",
                    "modified value"
                })
            });

            // Validate that the command failed (can't have an empty key)
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.Failed, result.Status);
            Assert.AreEqual("value", variables.Get("key", String.Empty));
        }

        /// <summary>
        ///     Tests that we can set a VariableModel to a list of strings via a command.
        /// </summary>
        [Test]
        public void TestValueStringList() {
            var variables = new VariableController();

            // Set the value of a empty key
            CommandResult result = variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                "key"
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                "value1",
                                "value2"
                            }
                        }
                    }
                }
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.IsNotNull(variables.Get<List<String>>("key"));
            Assert.AreEqual("value1", variables.Get<List<String>>("key").First());
            Assert.AreEqual("value2", variables.Get<List<String>>("key").Last());
        }
    }
}