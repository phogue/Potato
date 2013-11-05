using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Security;

namespace Procon.Core.Test.Variables {
    using Procon.Core.Variables;

    [TestFixture]
    public class TestVariablesSet {

        /// <summary>
        /// Tests that we can set a value, getting the successful flag back from the command.
        /// </summary>
        [Test]
        public void TestVariablesSetValue() {
            VariableController variables = new VariableController();

            CommandResultArgs result = variables.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "key",
                    "value"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual("value", variables.Get("key", String.Empty));
        }

        /// <summary>
        /// Tests that setting a variable will fetch/set as case insensitive
        /// </summary>
        [Test]
        public void TestVariablesSetValueCaseInsensitive() {
            VariableController variables = new VariableController();

            CommandResultArgs result = variables.Execute(new Command() {
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
        /// Tests that setting a variable will fail if the user does not have permission.
        /// </summary>
        [Test]
        public void TestVariablesSetValueInsufficientPermission() {
            VariableController variables = new VariableController() {
                Security = new SecurityController().Execute() as SecurityController
            };

            CommandResultArgs result = variables.Execute(new Command() {
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
        /// Tests that an empty key will result in a invalid parameter.
        /// </summary>
        [Test]
        public void TestVariablesSetValueEmptyKey() {
            VariableController variables = new VariableController();

            // Set the value of a empty key
            CommandResultArgs result = variables.Execute(new Command() {
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
        /// Tests setting a read only variable returns an error and the variable remains unchanged.
        /// </summary>
        [Test]
        public void TestVariablesSetValueReadOnly() {
            VariableController variables = new VariableController() {
                VolatileVariables = new List<Variable>() {
                    new Variable() {
                        Name = "key",
                        Value = "value",
                        Readonly = true
                    }
                }
            };

            // Set the value of a empty key
            CommandResultArgs result = variables.Execute(new Command() {
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
        /// Tests that setting an existing variable will succeed (different code branch because it does not need to be added first)
        /// </summary>
        [Test]
        public void TestVariablesSetValueOverrideExisting() {
            VariableController variables = new VariableController() {
                VolatileVariables = new List<Variable>() {
                    new Variable() {
                        Name = "key",
                        Value = "initial value"
                    }
                }
            };

            // Set the value of a empty key
            CommandResultArgs result = variables.Execute(new Command() {
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
        /// Sets a value for the archive, follows same code path as Set, but sets
        /// the same variable in VariablesArchive.
        /// </summary>
        [Test]
        public void TestVariablesSetValueA() {
            VariableController variables = new VariableController();

            // Set an archive variable
            CommandResultArgs result = variables.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSetA,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "key",
                    "value"
                })
            });

            // Validate that the command was successful and the key was set to the passed value.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual("value", variables.Get("key", String.Empty));
            Assert.AreEqual("value", variables.GetA("key", String.Empty));
        }

        /// <summary>
        /// Checks that a user must have permission to set a archive variable.
        /// </summary>
        [Test]
        public void TestVariablesSetValueAInsufficientPermission() {
            VariableController variables = new VariableController();

            CommandResultArgs result = variables.Execute(new Command() {
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                CommandType = CommandType.VariablesSetA,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "key",
                    "value"
                })
            });

            // Validate the command failed because we don't have permissions to execute it.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        /// Checks that we can override the value of an existing key in the variable archive.
        /// </summary>
        [Test]
        public void TestVariablesSetValueAOverrideExisting() {
            VariableController variables = new VariableController();

            // Set an archive variable
            CommandResultArgs result = variables.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSetA,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "key",
                    "value"
                })
            });

            // Validate that initially setting the variable is successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual("value", variables.Get("key", String.Empty));
            Assert.AreEqual("value", variables.GetA("key", String.Empty));

            result = variables.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSetA,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "key",
                    "changed value"
                })
            });

            // Validate that we changed changed an existing variable value.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual("changed value", variables.Get("key", String.Empty));
            Assert.AreEqual("changed value", variables.GetA("key", String.Empty));
        }

        [Test]
        public void TestVariablesSetValueAEmptyKey() {
            VariableController variables = new VariableController();

            // Set an archive variable
            CommandResultArgs result = variables.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSetA,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    String.Empty,
                    "value"
                })
            });

            // Validate that the command failed
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InvalidParameter, result.Status);
        }

        /// <summary>
        /// Sets a value for the archive using the common name enum.
        /// </summary>
        [Test]
        public void TestVariablesSetValueACommonName() {
            VariableController variables = new VariableController();

            // Set an archive variable
            variables.SetA(new Command() {
                Origin = CommandOrigin.Local
            }, CommonVariableNames.MaximumGameConnections, "value");

            // Validate that the command was successful and the key was set to the passed value.
            Assert.AreEqual("value", variables.Get(CommonVariableNames.MaximumGameConnections, String.Empty));
            Assert.AreEqual("value", variables.GetA(CommonVariableNames.MaximumGameConnections, String.Empty));
        }

        /// <summary>
        /// Tests that we can set a variable to a list of strings via a command.
        /// </summary>
        [Test]
        public void TestVariablesSetValueStringList() {
            VariableController variables = new VariableController();

            // Set the value of a empty key
            CommandResultArgs result = variables.Execute(new Command() {
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
