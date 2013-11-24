using System;
using NUnit.Framework;
using Procon.Net.Utils;

namespace Procon.Net.Test.Utils.StringExtensions {
    [TestFixture]
    public class RemoveDiacriticsTest {

        /// <summary>
        /// Tests that a string with diacritics is replaced.
        /// </summary>
        [Test]
        public void TestRemoveDiacriticsSuccess() {
            String removed = "Die Brösel eines Käsekuchen verzücken mich.".RemoveDiacritics();

            Assert.AreEqual("Die Brosel eines Kasekuchen verzucken mich.", removed);
        }
    }
}
