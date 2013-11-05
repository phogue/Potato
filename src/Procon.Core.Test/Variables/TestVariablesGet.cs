using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Security;

namespace Procon.Core.Test.Variables {
    using Procon.Core.Variables;

    [TestFixture]
    public class TestVariablesGet {

        internal class VariableComplexValue {
            public int PropertyOne { get; set; }
            public String PropertyTwo { get; set; }
        }

        /// <summary>
        /// Fetches a simple variable from the variable controller.
        /// </summary>
        [Test]
        public void TestVariablesGetValue() {
            VariableController variables = new VariableController() {
                VolatileVariables = new List<Variable>() {
                    new Variable() {
                        Name = "key",
                        Value = "value"
                    }
                }
            };

            CommandResultArgs result = variables.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesGet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "key"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual("value", result.Now.Variables.First().ToType<String>(String.Empty));
        }

        /// <summary>
        /// Tests that passing an empty key to the Get parameter will return an error.
        /// </summary>
        [Test]
        public void TestVariablesGetValueEmptyKey() {
            VariableController variables = new VariableController() {
                VolatileVariables = new List<Variable>() {
                    new Variable() {
                        Name = "key",
                        Value = "value"
                    }
                }
            };

            CommandResultArgs result = variables.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesGet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    String.Empty
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InvalidParameter, result.Status);
        }

        /// <summary>
        /// Tests that a user without correct permissions to fetch a variable will get an error.
        /// </summary>
        [Test]
        public void TestVariablesGetValueInsufficientPermission() {
            VariableController variables = new VariableController() {
                Security = new SecurityController().Execute() as SecurityController,
                VolatileVariables = new List<Variable>() {
                    new Variable() {
                        Name = "key",
                        Value = "value"
                    }
                }
            };

            CommandResultArgs result = variables.Execute(new Command() {
                CommandType = CommandType.VariablesGet,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "key"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        /// Tests fetching a variable by a common name
        /// </summary>
        [Test]
        public void TestVariablesGetValueCommonName() {
            VariableController variables = new VariableController() {
                VolatileVariables = new List<Variable>() {
                    new Variable() {
                        Name = CommonVariableNames.MaximumGameConnections.ToString(),
                        Value = "value"
                    }
                }
            };

            Assert.AreEqual("value", variables.Get(new Command() {
                Origin = CommandOrigin.Local
            }, CommonVariableNames.MaximumGameConnections).Now.Variables.First().ToType<String>());
        }

        /// <summary>
        /// Validates that if a valid cast exists then then the variable will be cast to that type.
        /// </summary>
        [Test]
        public void TestVariablesGetValueValidTypeCast() {
            VariableController variables = new VariableController() {
                VolatileVariables = new List<Variable>() {
                    new Variable() {
                        Name = "key",
                        Value = "10"
                    }
                }
            };

            Assert.AreEqual(10, variables.Get<int>("key"));
        }

        /// <summary>
        /// Validates that if no cast is possible and no default is supplied the value will
        /// not equal what we expect.
        /// </summary>
        [Test]
        public void TestVariablesGetValueInvalidTypeCastNoDefault() {
            VariableController variables = new VariableController() {
                VolatileVariables = new List<Variable>() {
                    new Variable() {
                        Name = "key",
                        Value = "10!"
                    }
                }
            };

            Assert.AreNotEqual(10, variables.Get<int>("key"));
        }

        /// <summary>
        /// Validates that the default value will be used if no cast is possible.
        /// </summary>
        [Test]
        public void TestVariablesGetValueInvalidTypeCastWithDefault() {
            VariableController variables = new VariableController() {
                VolatileVariables = new List<Variable>() {
                    new Variable() {
                        Name = "key",
                        Value = "10!"
                    }
                }
            };

            Assert.AreEqual(10, variables.Get<int>("key", 10));
        }

        /// <summary>
        /// Fetches the first type that matches in the variable list.
        /// </summary>
        [Test]
        public void TestVariablesGetValueFirstValueType() {
            VariableController variables = new VariableController() {
                VolatileVariables = new List<Variable>() {
                    new Variable() {
                        Name = "key",
                        Value = "ten"
                    },
                    new Variable() {
                        Name = "key",
                        Value = "20"
                    },
                    new Variable() {
                        Name = "key",
                        Value = 30
                    }
                }
            };

            Assert.AreEqual(variables.Get<int>(), 30);
        }

        /// <summary>
        /// Validates that getting a variable value directly with an invalid cast
        /// will fallback to the default value.
        /// </summary>
        [Test]
        public void TestVariablesDirectVariableEmptyValueWithDefault() {
            VariableController variables = new VariableController() {
                VolatileVariables = new List<Variable>() {
                    new Variable() {
                        Name = "key",
                        Value = ""
                    }
                }
            };

            Assert.AreEqual(10, variables.VolatileVariables.First().ToType<int>(10));
        }

        /// <summary>
        /// Fetches the complex value from
        /// </summary>
        [Test]
        public void TestVariablesGetValueComplexValue() {
            VariableController variables = new VariableController() {
                VolatileVariables = new List<Variable>() {
                    new Variable() {
                        Name = "key",
                        Value = new VariableComplexValue() {
                            PropertyOne = 1,
                            PropertyTwo = "two"
                        }
                    }
                }
            };

            VariableComplexValue value = variables.Get(new Command() {
                Origin = CommandOrigin.Local
            }, "key").Now.Variables.First().ToType<VariableComplexValue>();

            Assert.AreEqual(1, value.PropertyOne);
            Assert.AreEqual("two", value.PropertyTwo);
        }

        /// <summary>
        /// Fetches the complex value from the archive.
        /// </summary>
        [Test]
        public void TestVariablesGetValueAComplexValue() {
            VariableController variables = new VariableController();

            variables.SetA(new Command() {
                Origin = CommandOrigin.Local
            }, "key", new VariableComplexValue() {
                PropertyOne = 1,
                PropertyTwo = "two"
            });

            VariableComplexValue value = variables.GetA<VariableComplexValue>("key");

            Assert.AreEqual(1, value.PropertyOne);
            Assert.AreEqual("two", value.PropertyTwo);
        }
    }
}
