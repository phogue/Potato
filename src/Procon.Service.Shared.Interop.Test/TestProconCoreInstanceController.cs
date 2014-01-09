﻿using System.Linq;
using NUnit.Framework;
using Procon.Core;

namespace Procon.Service.Shared.Interop.Test {
    [TestFixture]
    public class TestProconCoreInstanceController {
        /// <summary>
        /// Ensures the definition correctly matches the type name. Since Service.Shared does
        /// not depend on Procon.Core we have to late load the assembly. This just ensures
        /// that the type name matches in case some one refactors the names in 2 years. It happens and it's bad.
        /// </summary>
        [Test]
        public void TestTypeProconCoreInstanceControllerDefinition() {
            Assert.AreEqual(typeof(InstanceController).FullName, Defines.TypeProconCoreInstanceController);
        }

        /// <summary>
        /// Tests the InstanceController implements IService.
        /// </summary>
        [Test]
        public void TestProconCoreInstanceControllerServiceInterface() {
            Assert.IsTrue(typeof(InstanceController).GetInterfaces().Any(type => type == typeof(IService)));
        }
    }
}