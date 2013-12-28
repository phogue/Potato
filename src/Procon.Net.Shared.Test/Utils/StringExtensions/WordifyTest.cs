using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Net.Shared.Utils;

namespace Procon.Net.Shared.Test.Utils.StringExtensions {
    /// <summary>
    /// Tests various cases for StringExtensions.Wordify()
    /// </summary>
    [TestFixture]
    public class WordifyTest {
        /// <summary>
        /// Tests a single word will simply come through as a list with a single word in it.
        /// </summary>
        [Test]
        public void TestWordifySingleWord() {
            List<string> words = "single".Wordify();

            Assert.AreEqual(1, words.Count);
            Assert.AreEqual("single", words.First());
        }

        /// <summary>
        /// Tests two simple words will be split into two simple words.
        /// </summary>
        [Test]
        public void TestWordifyDoubleWord() {
            List<string> words = "first second".Wordify();

            Assert.AreEqual(2, words.Count);
            Assert.AreEqual("first", words.First());
            Assert.AreEqual("second", words.Last());
        }

        /// <summary>
        /// Tests three words, with the first two being quoted will yeild two words.
        /// </summary>
        [Test]
        public void TestWordifyTripleWordFirstTwoQuoted() {
            List<string> words = "\"first second\" third".Wordify();

            Assert.AreEqual(2, words.Count);
            Assert.AreEqual("first second", words.First());
            Assert.AreEqual("third", words.Last());
        }

        /// <summary>
        /// Tests three words, with the first two being quoted will yeild two words.
        /// </summary>
        [Test]
        public void TestWordifyTripleWordLastTwoQuoted() {
            List<string> words = "first \"second third\"".Wordify();

            Assert.AreEqual(2, words.Count);
            Assert.AreEqual("first", words.First());
            Assert.AreEqual("second third", words.Last());
        }

        /// <summary>
        /// Tests newline will be counted as a character and escaped properly
        /// </summary>
        [Test]
        public void TestWordifyEscapedNewLine() {
            List<string> words = "first\\nsecond".Wordify();

            Assert.AreEqual(1, words.Count);
            Assert.AreEqual("first\nsecond", words.First());
        }

        /// <summary>
        /// Tests newline will be counted as a character and escaped properly
        /// </summary>
        [Test]
        public void TestWordifyEscapedCarriageReturn() {
            List<string> words = "first\\rsecond".Wordify();

            Assert.AreEqual(1, words.Count);
            Assert.AreEqual("first\rsecond", words.First());
        }

        /// <summary>
        /// Tests tab will be counted as a character and escaped properly
        /// </summary>
        [Test]
        public void TestWordifyEscapedTab() {
            List<string> words = "first\\tsecond".Wordify();

            Assert.AreEqual(1, words.Count);
            Assert.AreEqual("first\tsecond", words.First());
        }

        /// <summary>
        /// Tests backslash will be counted as a character and escaped properly
        /// </summary>
        [Test]
        public void TestWordifyEscapedBackslash() {
            List<string> words = "first\\\\second".Wordify();

            Assert.AreEqual(1, words.Count);
            Assert.AreEqual("first\\second", words.First());
        }

        /// <summary>
        /// Tests backslash will be counted as a character and escaped properly
        /// </summary>
        [Test]
        public void TestWordifyQuoteWithinQuote() {
            List<string> words = "\"first \\\"second\" third".Wordify();

            Assert.AreEqual(2, words.Count);
            Assert.AreEqual("first \"second", words.First());
            Assert.AreEqual("third", words.Last());
        }
    }
}
