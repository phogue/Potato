using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Net.Shared.Truths;

namespace Procon.Net.Shared.Test.Truths {
    [TestFixture]
    public class TestTruths {
        /// <summary>
        /// Test that all non-static classes within the Procon.Net.Shared.Truths namespace
        /// are marked as serializable.
        /// </summary>
        [Test]
        public void TestClassesAreSerializable() {
            IEnumerable<Type> nonSerializableTypes = typeof(ITruth).Assembly
                .GetTypes()
                // Where it's a class
                .Where(type => type.IsClass == true)
                // Where it's not static (both are true when static)
                .Where(type => type.IsAbstract == false && type.IsSealed == false)
                // Where the type exists in our namespace
                .Where(type => type.Namespace != null && type.Namespace.StartsWith("Procon.Net.Shared.Truths"))
                // Where they are not serializable
                .Where(type => type.IsSerializable == false);

            Assert.IsEmpty(nonSerializableTypes);
        }
    }
}
