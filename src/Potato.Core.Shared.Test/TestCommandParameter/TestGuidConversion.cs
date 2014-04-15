using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Potato.Core.Shared.Test.TestCommandParameter {
    public class TestGuidConversion {

        /// <summary>
        ///     Tests that if all values can't be converted to guids then the result will return null.
        /// </summary>
        [Test]
        public void TestAllGuidConversionFailure() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "9D2B0228-4D0D-4C23-8B49-01A698857709",
                        "f380eb1e-1438-48c0-8c3d-ad55f2d40538",
                        "Invalid"
                    }
                }
            };

            var items = parameter.All(typeof(Guid)) as List<Object>;

            Assert.IsNull(items);
        }

        [Test]
        public void TestAllGuidConversionSuccess() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "9D2B0228-4D0D-4C23-8B49-01A698857709",
                        "f380eb1e-1438-48c0-8c3d-ad55f2d40538",
                        "76268850-2029-4b5f-b421-5b5ee4f17b6b"
                    }
                }
            };
            
            var list = (List<Object>)parameter.All(typeof(Guid));

            Assert.AreEqual(new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709"), list[0]);
            Assert.AreEqual(new Guid("f380eb1e-1438-48c0-8c3d-ad55f2d40538"), list[1]);
            Assert.AreEqual(new Guid("76268850-2029-4b5f-b421-5b5ee4f17b6b"), list[2]);
        }

        /// <summary>
        ///     Tests that the default is returned when a conversion does not exist for a given type.
        /// </summary>
        [Test]
        public void TestFirstGuidConversionFailed() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "Invalid"
                    }
                }
            };

            Assert.AreEqual(default(Guid), parameter.First<Guid>());
        }

        /// <summary>
        ///     Tests that we can pull and convert the first value from a list of strings
        ///     to an guid
        /// </summary>
        [Test]
        public void TestFirstGuidConversionSuccess() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "9D2B0228-4D0D-4C23-8B49-01A698857709"
                    }
                }
            };

            Assert.AreEqual(new Guid("9D2B0228-4D0D-4C23-8B49-01A698857709"), parameter.First<Guid>());
        }

        /// <summary>
        ///     Tests that if a single value in the string array cannot be converted to the guid
        ///     type then it does not "have many" of type guid.
        /// </summary>
        [Test]
        public void TestHasManyGuidConversionFailed() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "9D2B0228-4D0D-4C23-8B49-01A698857709",
                        "f380eb1e-1438-48c0-8c3d-ad55f2d40538",
                        "Invalid"
                    }
                }
            };

            Assert.IsFalse(parameter.HasMany<Guid>());
        }

        /// <summary>
        ///     Tests that if an array of strings can be converted to the guid type then
        ///     it does "have many" of the type guid.
        /// </summary>
        [Test]
        public void TestHasManyGuidConversionSuccess() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "9D2B0228-4D0D-4C23-8B49-01A698857709",
                        "f380eb1e-1438-48c0-8c3d-ad55f2d40538"
                    }
                }
            };

            Assert.IsTrue(parameter.HasMany<Guid>());
        }

        /// <summary>
        ///     Tests if no conversion exists for the string to the type then
        ///     it does not "have one" of that type.
        /// </summary>
        [Test]
        public void TestHasOneIntegerConversionFailure() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "one"
                    }
                }
            };

            Assert.IsFalse(parameter.HasOne<Guid>());
        }

        /// <summary>
        ///     Tests that if a string can be converted to an guid then
        ///     the parameter does "have one" of type guid.
        /// </summary>
        [Test]
        public void TestHasOneGuidConversionSuccess() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "9D2B0228-4D0D-4C23-8B49-01A698857709"
                    }
                }
            };

            Assert.IsTrue(parameter.HasOne<Guid>());
        }

        /// <summary>
        ///     Tests that if a string can be converted to an guid then
        ///     the parameter does "have one" of type guid even when multiple string exist
        /// </summary>
        [Test]
        public void TestHasOneGuidConversionSuccessWithMultiple() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "9D2B0228-4D0D-4C23-8B49-01A698857709",
                        "f380eb1e-1438-48c0-8c3d-ad55f2d40538"
                    }
                }
            };

            Assert.IsTrue(parameter.HasOne<Guid>());
        }

        /// <summary>
        ///     Tests that if a string can be converted to an guid then
        ///     the parameter does "have one" of type guid even when multiple string exist
        ///     and anything beyond the first string is invalid
        /// </summary>
        [Test]
        public void TestHasOneGuidConversionSuccessWithMultipleAndInvalid() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "9D2B0228-4D0D-4C23-8B49-01A698857709",
                        "Invalid"
                    }
                }
            };

            Assert.IsTrue(parameter.HasOne<Guid>());
        }
    }
}
