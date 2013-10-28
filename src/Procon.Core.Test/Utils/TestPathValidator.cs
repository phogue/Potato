using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Utils;

namespace Procon.Core.Test.Utils {
    [TestClass]
    public class TestPathValidator {

        /// <summary>
        /// Tests that a valid file name will be returned as it was passed in.
        /// </summary>
        [TestMethod]
        public void TestValidateAlreadyValid() {
            const string path = "valid";

            Assert.AreEqual(path, PathValidator.Valdiate(path));
        }

        /// <summary>
        /// Tests that diacritics are substituted with english'ier alternatives.
        /// </summary>
        [TestMethod]
        public void TestValidateSubstitutedDiacritics() {
            const string path = "üdiäcriticöß";
            const string substituted = "uediaecriticoess";

            Assert.AreEqual(substituted, PathValidator.Valdiate(path));
        }

        /// <summary>
        /// Tests that invalid characters are replaced with underscores
        /// </summary>
        [TestMethod]
        public void TestValidateInvalidStripped() {
            const string path = "invalid%character";
            const string stripped = "invalid_character";

            Assert.AreEqual(stripped, PathValidator.Valdiate(path));
        }

        /// <summary>
        /// Tests that invalid characters are replaced with underscores but 
        /// multiple underscores are trimmed to a single underscore.
        /// </summary>
        [TestMethod]
        public void TestValidateInvalidStrippedAndTrimmed() {
            const string path = "invalid%*&character";
            const string stripped = "invalid_character";

            Assert.AreEqual(stripped, PathValidator.Valdiate(path));
        }
    }
}
