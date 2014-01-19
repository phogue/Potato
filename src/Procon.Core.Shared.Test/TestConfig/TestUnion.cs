using System;
using NUnit.Framework;
using Newtonsoft.Json.Linq;

namespace Procon.Core.Shared.Test.TestConfig {
    [TestFixture]
    public class TestUnion : TestConfigBase {
        /// <summary>
        /// Tests that unique root elements that exist in a and b will both
        /// appear in c.
        /// </summary>
        [Test]
        public void TestCombiningUniqueRootElements() {
            IConfig a = new Config() {
                Document = new JObject() {
                    new JProperty("A", new JArray())
                }
            };

            IConfig b = new Config() {
                Document = new JObject() {
                    new JProperty("B", new JArray())
                }
            };

            IConfig c = a.Union(b);

            Assert.IsNotNull(c.Document["A"]);
            Assert.IsNotNull(c.Document["B"]);
        }

        /// <summary>
        /// Tests that unique root elements that exist in a and b will both
        /// appear in c.
        /// </summary>
        [Test]
        public void TestCombiningIdenticalRootElements() {
            IConfig a = new Config() {
                Document = new JObject() {
                    new JProperty("identical", new JArray() {
                        new JObject() {
                            new JProperty("keyA", "valueA")
                        }
                    })
                }
            };

            IConfig b = new Config() {
                Document = new JObject() {
                    new JProperty("identical", new JArray() {
                        new JObject() {
                            new JProperty("keyB", "valueB")
                        }
                    })
                }
            };

            IConfig c = a.Union(b);
            
            Assert.AreEqual("valueA", c.Document["identical"].First["keyA"].Value<String>());
            Assert.AreEqual("valueB", c.Document["identical"].Last["keyB"].Value<String>());
        }

        /// <summary>
        /// Tests an argument null exception is raised if null is passed into union.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestNullException() {
            IConfig config = new Config();

            config.Union(null);
        }
    }
}
