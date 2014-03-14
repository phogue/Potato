#region

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Core.Variables;

#endregion

namespace Procon.Core.Test.Variables {
    [TestFixture]
    public class TestVariablesGet {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        internal class VariableComplexValue {
            public int PropertyOne { get; set; }
            public String PropertyTwo { get; set; }
        }

        /// <summary>
        ///     Validates that getting a VariableModel value directly with an invalid cast
        ///     will fallback to the default value.
        /// </summary>
        [Test]
        public void TestEmptyValueWithDefault() {
            var variables = new VariableController() {
                VolatileVariables = new List<VariableModel>() {
                    new VariableModel() {
                        Name = "key",
                        Value = ""
                    }
                }
            };

            Assert.AreEqual(10, variables.VolatileVariables.First().ToType(10));
        }

        /// <summary>
        ///     Fetches a simple VariableModel from the VariableModel controller.
        /// </summary>
        [Test]
        public void TestValue() {
            var variables = new VariableController() {
                VolatileVariables = new List<VariableModel>() {
                    new VariableModel() {
                        Name = "key",
                        Value = "value"
                    }
                }
            };

            ICommandResult result = variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesGet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "key"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
            Assert.AreEqual("value", result.Now.Variables.First().ToType(String.Empty));
        }

        /// <summary>
        ///     Tests fetching a VariableModel by a common name
        /// </summary>
        [Test]
        public void TestValueCommonName() {
            var variables = new VariableController() {
                VolatileVariables = new List<VariableModel>() {
                    new VariableModel() {
                        Name = CommonVariableNames.MaximumProtocolConnections.ToString(),
                        Value = "value"
                    }
                }
            };

            var value = variables.Tunnel(CommandBuilder.VariablesGet(CommonVariableNames.MaximumProtocolConnections).SetOrigin(CommandOrigin.Local)).Now.Variables.First().ToType<String>();

            Assert.AreEqual("value", value);
        }

        /// <summary>
        ///     Fetches the complex value from
        /// </summary>
        [Test]
        public void TestValueComplexValue() {
            var variables = new VariableController() {
                VolatileVariables = new List<VariableModel>() {
                    new VariableModel() {
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
        ///     Tests that passing an empty key to the Get parameter will return an error.
        /// </summary>
        [Test]
        public void TestValueEmptyKey() {
            var variables = new VariableController() {
                VolatileVariables = new List<VariableModel>() {
                    new VariableModel() {
                        Name = "key",
                        Value = "value"
                    }
                }
            };

            ICommandResult result = variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesGet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    String.Empty
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InvalidParameter, result.CommandResultType);
        }

        /// <summary>
        ///     Tests that a user without correct permissions to fetch a VariableModel will get an error.
        /// </summary>
        [Test]
        public void TestInsufficientPermission() {
            var variables = new VariableController() {
                Shared = {
                    Security = new SecurityController().Execute() as SecurityController,
                },
                VolatileVariables = new List<VariableModel>() {
                    new VariableModel() {
                        Name = "key",
                        Value = "value"
                    }
                }
            };

            ICommandResult result = variables.Tunnel(new Command() {
                CommandType = CommandType.VariablesGet,
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "key"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }

        /// <summary>
        ///     Validates that if no cast is possible and no default is supplied the value will
        ///     not equal what we expect.
        /// </summary>
        [Test]
        public void TestInvalidTypeCastNoDefault() {
            var variables = new VariableController() {
                VolatileVariables = new List<VariableModel>() {
                    new VariableModel() {
                        Name = "key",
                        Value = "10!"
                    }
                }
            };

            Assert.AreNotEqual(10, variables.Get<int>("key"));
        }

        /// <summary>
        ///     Validates that the default value will be used if no cast is possible.
        /// </summary>
        [Test]
        public void TestInvalidTypeCastWithDefault() {
            var variables = new VariableController() {
                VolatileVariables = new List<VariableModel>() {
                    new VariableModel() {
                        Name = "key",
                        Value = "10!"
                    }
                }
            };

            Assert.AreEqual(10, variables.Get("key", 10));
        }

        /// <summary>
        ///     Validates that if a valid cast exists then then the VariableModel will be cast to that type.
        /// </summary>
        [Test]
        public void TesValueValidTypeCast() {
            var variables = new VariableController() {
                VolatileVariables = new List<VariableModel>() {
                    new VariableModel() {
                        Name = "key",
                        Value = "10"
                    }
                }
            };

            Assert.AreEqual(10, variables.Get<int>("key"));
        }
    }
}