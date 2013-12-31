using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Net.Shared.Truths;

namespace Procon.Net.Shared.Test.Models {
    [TestFixture]
    public class TestModels {

        /// <summary>
        /// Test that all non-static classes within the Procon.Net.Shared.Models namespace
        /// are marked as serializable.
        /// </summary>
        [Test]
        public void TestClassesAreSerializable() {
            IEnumerable<Type> nonSerializableTypes = typeof(ITruth).Assembly
                .GetTypes()
                // Where it's a class
                .Where(type => type.IsClass == true)
                // Where it's not static (both are true when static)
                .Where(type => type.IsAbstract == false || type.IsSealed == false)
                // Where the type exists in our namespace
                .Where(type => type.Namespace != null && type.Namespace.Equals("Procon.Net.Shared.Models"))
                // The class is not generated for an anonymous delegate (I assume this is what it is?)
                .Where(type => type.IsNested == false)
                // Where they are not serializable
                .Where(type => type.IsSerializable == false);

            Assert.IsEmpty(nonSerializableTypes);
        }
    }
}
