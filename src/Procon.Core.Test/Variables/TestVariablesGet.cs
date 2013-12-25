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
        internal class VariableComplexValue {
            public int PropertyOne { get; set; }
            public String PropertyTwo { get; set; }
        }

        /// <summary>
        ///     Validates that getting a VariableModel value directly with an invalid cast
        ///     will fallback to the default value.
        /// </summary>
        [Test]
        public void TestVariablesDirectVariableEmptyValueWithDefault() {
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
        public void TestVariablesGetValue() {
            var variables = new VariableController() {
                VolatileVariables = new List<VariableModel>() {
                    new VariableModel() {
                        Name = "key",
                        Value = "value"
                    }
                }
            };

            CommandResultArgs result = variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesGet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "key"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual("value", result.Now.Variables.First().ToType(String.Empty));
        }

        /// <summary>
        ///     Fetches the complex value from the archive.
        /// </summary>
        [Test]
        public void TestVariablesGetValueAComplexValue() {
            var variables = new VariableController();

            variables.SetA(new Command() {
                Origin = CommandOrigin.Local
            }, "key", new VariableComplexValue() {
                PropertyOne = 1,
                PropertyTwo = "two"
            });

            var value = variables.GetA<VariableComplexValue>("key");

            Assert.AreEqual(1, value.PropertyOne);
            Assert.AreEqual("two", value.PropertyTwo);
        }

        /// <summary>
        ///     Tests fetching a VariableModel by a common name
        /// </summary>
        [Test]
        public void TestVariablesGetValueCommonName() {
            var variables = new VariableController() {
                VolatileVariables = new List<VariableModel>() {
                    new VariableModel() {
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
        ///     Fetches the complex value from
        /// </summary>
        [Test]
        public void TestVariablesGetValueComplexValue() {
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
        public void TestVariablesGetValueEmptyKey() {
            var variables = new VariableController() {
                VolatileVariables = new List<VariableModel>() {
                    new VariableModel() {
                        Name = "key",
                        Value = "value"
                    }
                }
            };

            CommandResultArgs result = variables.Tunnel(new Command() {
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
        ///     Tests that a user without correct permissions to fetch a VariableModel will get an error.
        /// </summary>
        [Test]
        public void TestVariablesGetValueInsufficientPermission() {
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

            CommandResultArgs result = variables.Tunnel(new Command() {
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
        ///     Validates that if no cast is possible and no default is supplied the value will
        ///     not equal what we expect.
        /// </summary>
        [Test]
        public void TestVariablesGetValueInvalidTypeCastNoDefault() {
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
        public void TestVariablesGetValueInvalidTypeCastWithDefault() {
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
        public void TestVariablesGetValueValidTypeCast() {
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