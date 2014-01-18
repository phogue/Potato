using System;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using Procon.Core.Shared.Test.TestConfig.Mocks;

namespace Procon.Core.Shared.Test.TestConfig {
    [TestFixture]
    public class TestRootOf : TestConfigBase {
        /// <summary>
        /// Tests that a root can be fetched from an existing type
        /// </summary>
        [Test]
        public void TestRootOfSingleProperty() {
            IConfig config = new JsonConfig().Create<MockSimpleConcrete>();

            var root = config.RootOf<MockSimpleConcrete>();

            Assert.AreEqual("Procon.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete", ((JProperty)root.Parent).Name);
        }

        /// <summary>
        /// Tests that a root can be fetched when multiple properties exist on the document
        /// </summary>
        [Test]
        public void TestRootOfMultipleProperties() {
            IConfig config = new JsonConfig().Create<MockSimpleConcrete>();
            config.Document.Add(new JProperty("Procon.Core.Shared.Test.TestConfig.Mocks.DoesNotExist", new JArray()));

            var root = config.RootOf<MockSimpleConcrete>();

            Assert.AreEqual("Procon.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete", ((JProperty)root.Parent).Name);
        }

        /// <summary>
        /// Tests that a root can be found of a type that the document was not originally created for.
        /// </summary>
        [Test]
        public void TestRootOfNotCreatedForProperty() {
            IConfig config = new JsonConfig().Create<MockSimpleConcrete>();
            config.Document.Add(new JProperty("Procon.Core.Shared.Test.TestConfig.TestRootOf", new JArray()));

            var root = config.RootOf<TestRootOf>();

            Assert.AreEqual("Procon.Core.Shared.Test.TestConfig.TestRootOf", ((JProperty)root.Parent).Name);
        }

        /// <summary>
        /// Tests that a root can be found of a type that the document was not originally created for.
        /// </summary>
        [Test]
        public void TestRootOfNonExistantPropertyReturnsNotNull() {
            IConfig config = new JsonConfig().Create<MockSimpleConcrete>();

            var root = config.RootOf<TestRootOf>();

            Assert.IsNotNull(root);
        }

        /// <summary>
        /// Tests that a root can be fetched from an existing type
        /// </summary>
        [Test]
        public void TestExplicitTypeRootOfSingleProperty() {
            IConfig config = new JsonConfig().Create<MockSimpleConcrete>();

            var root = config.RootOf(typeof(MockSimpleConcrete));

            Assert.AreEqual("Procon.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete", ((JProperty)root.Parent).Name);
        }

        /// <summary>
        /// Tests that a root can be fetched when multiple properties exist on the document
        /// </summary>
        [Test]
        public void TestExplicitTypeRootOfMultipleProperties() {
            IConfig config = new JsonConfig().Create<MockSimpleConcrete>();
            config.Document.Add(new JProperty("Procon.Core.Shared.Test.TestConfig.Mocks.DoesNotExist", new JArray()));

            var root = config.RootOf(typeof(MockSimpleConcrete));

            Assert.AreEqual("Procon.Core.Shared.Test.TestConfig.Mocks.MockSimpleConcrete", ((JProperty)root.Parent).Name);
        }

        /// <summary>
        /// Tests that a root can be found of a type that the document was not originally created for.
        /// </summary>
        [Test]
        public void TestExplicitTypeRootOfNotCreatedForProperty() {
            IConfig config = new JsonConfig().Create<MockSimpleConcrete>();
            config.Document.Add(new JProperty("Procon.Core.Shared.Test.TestConfig.TestRootOf", new JArray()));

            var root = config.RootOf(typeof(TestRootOf));

            Assert.AreEqual("Procon.Core.Shared.Test.TestConfig.TestRootOf", ((JProperty)root.Parent).Name);
        }

        /// <summary>
        /// Tests that a root can be found of a type that the document was not originally created for.
        /// </summary>
        [Test]
        public void TestExplicitTypeRootOfNonExistantPropertyReturnsNotNull() {
            IConfig config = new JsonConfig().Create<MockSimpleConcrete>();

            var root = config.RootOf(typeof(TestRootOf));

            Assert.IsNotNull(root);
        }

        /// <summary>
        /// Tests an argument null exception is raised if null is passed into RootOf.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestNullException() {
            new JsonConfig().RootOf(null);
        }
    }
}
