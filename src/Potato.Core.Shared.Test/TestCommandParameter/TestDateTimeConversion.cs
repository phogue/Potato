using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Potato.Core.Shared.Test.TestCommandParameter {
    public class TestDateTimeConversion {

        /// <summary>
        ///     Tests that if all values can't be converted to date/times then the result will return null.
        /// </summary>
        [Test]
        public void TestAllDateTimeConversionFailure() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<string>() {
                        "2014-04-14 13:50:59",
                        "2014-04-11 03:23:13",
                        "Invalid"
                    }
                }
            };

            var items = parameter.All(typeof(DateTime)) as List<object>;

            Assert.IsNull(items);
        }

        [Test]
        public void TestAllDateTimeConversionSuccess() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<string>() {
                        "2014-04-14 13:50:59",
                        "2014-04-11 03:23:13",
                        "2014-04-10 03:23:13"
                    }
                }
            };
            
            var list = (List<object>)parameter.All(typeof(DateTime));

            Assert.AreEqual(DateTime.Parse("2014-04-14 13:50:59"), list[0]);
            Assert.AreEqual(DateTime.Parse("2014-04-11 03:23:13"), list[1]);
            Assert.AreEqual(DateTime.Parse("2014-04-10 03:23:13"), list[2]);
        }

        /// <summary>
        ///     Tests that the default is returned when a conversion does not exist for a given type.
        /// </summary>
        [Test]
        public void TestFirstDateTimeConversionFailed() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<string>() {
                        "Invalid"
                    }
                }
            };

            Assert.AreEqual(default(DateTime), parameter.First<DateTime>());
        }

        /// <summary>
        ///     Tests that we can pull and convert the first value from a list of strings
        ///     to an date/time
        /// </summary>
        [Test]
        public void TestFirstDateTimeConversionSuccess() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<string>() {
                        "2014-04-14 13:50:59"
                    }
                }
            };

            Assert.AreEqual(DateTime.Parse("2014-04-14 13:50:59"), parameter.First<DateTime>());
        }

        /// <summary>
        ///     Tests that if a single value in the string array cannot be converted to the date/time
        ///     type then it does not "have many" of type date/time.
        /// </summary>
        [Test]
        public void TestHasManyDateTimeConversionFailed() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<string>() {
                        "2014-04-14 13:50:59",
                        "2014-04-11 03:23:13",
                        "Invalid"
                    }
                }
            };

            Assert.IsFalse(parameter.HasMany<DateTime>());
        }

        /// <summary>
        ///     Tests that if an array of strings can be converted to the date/time type then
        ///     it does "have many" of the type date/time.
        /// </summary>
        [Test]
        public void TestHasManyDateTimeConversionSuccess() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<string>() {
                        "2014-04-14 13:50:59",
                        "2014-04-11 03:23:13"
                    }
                }
            };

            Assert.IsTrue(parameter.HasMany<DateTime>());
        }

        /// <summary>
        ///     Tests if no conversion exists for the string to the type then
        ///     it does not "have one" of that type.
        /// </summary>
        [Test]
        public void TestHasOneIntegerConversionFailure() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<string>() {
                        "one"
                    }
                }
            };

            Assert.IsFalse(parameter.HasOne<DateTime>());
        }

        /// <summary>
        ///     Tests that if a string can be converted to an date/time then
        ///     the parameter does "have one" of type date/time.
        /// </summary>
        [Test]
        public void TestHasOneDateTimeConversionSuccess() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<string>() {
                        "2014-04-14 13:50:59"
                    }
                }
            };

            Assert.IsTrue(parameter.HasOne<DateTime>());
        }

        /// <summary>
        ///     Tests that if a string can be converted to an date/time then
        ///     the parameter does "have one" of type date/time even when multiple string exist
        /// </summary>
        [Test]
        public void TestHasOneDateTimeConversionSuccessWithMultiple() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<string>() {
                        "2014-04-14 13:50:59",
                        "2014-04-11 03:23:13"
                    }
                }
            };

            Assert.IsTrue(parameter.HasOne<DateTime>());
        }

        /// <summary>
        ///     Tests that if a string can be converted to an date/time then
        ///     the parameter does "have one" of type date/time even when multiple string exist
        ///     and anything beyond the first string is invalid
        /// </summary>
        [Test]
        public void TestHasOneDateTimeConversionSuccessWithMultipleAndInvalid() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<string>() {
                        "2014-04-14 13:50:59",
                        "Invalid"
                    }
                }
            };

            Assert.IsTrue(parameter.HasOne<DateTime>());
        }
    }
}
