using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Net.Shared.Utils;
using Procon.Net.Utils;

namespace Procon.Net.Test.Utils.StringExtensions {
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
            List<String> words = "single".Wordify();

            Assert.AreEqual(1, words.Count);
            Assert.AreEqual("single", words.First());
        }

        /// <summary>
        /// Tests two simple words will be split into two simple words.
        /// </summary>
        [Test]
        public void TestWordifyDoubleWord() {
            List<String> words = "first second".Wordify();

            Assert.AreEqual(2, words.Count);
            Assert.AreEqual("first", words.First());
            Assert.AreEqual("second", words.Last());
        }

        /// <summary>
        /// Tests three words, with the first two being quoted will yeild two words.
        /// </summary>
        [Test]
        public void TestWordifyTripleWordFirstTwoQuoted() {
            List<String> words = "\"first second\" third".Wordify();

            Assert.AreEqual(2, words.Count);
            Assert.AreEqual("first second", words.First());
            Assert.AreEqual("third", words.Last());
        }

        /// <summary>
        /// Tests three words, with the first two being quoted will yeild two words.
        /// </summary>
        [Test]
        public void TestWordifyTripleWordLastTwoQuoted() {
            List<String> words = "first \"second third\"".Wordify();

            Assert.AreEqual(2, words.Count);
            Assert.AreEqual("first", words.First());
            Assert.AreEqual("second third", words.Last());
        }

        /// <summary>
        /// Tests newline will be counted as a character and escaped properly
        /// </summary>
        [Test]
        public void TestWordifyEscapedNewLine() {
            List<String> words = "first\\nsecond".Wordify();

            Assert.AreEqual(1, words.Count);
            Assert.AreEqual("first\nsecond", words.First());
        }

        /// <summary>
        /// Tests newline will be counted as a character and escaped properly
        /// </summary>
        [Test]
        public void TestWordifyEscapedCarriageReturn() {
            List<String> words = "first\\rsecond".Wordify();

            Assert.AreEqual(1, words.Count);
            Assert.AreEqual("first\rsecond", words.First());
        }

        /// <summary>
        /// Tests tab will be counted as a character and escaped properly
        /// </summary>
        [Test]
        public void TestWordifyEscapedTab() {
            List<String> words = "first\\tsecond".Wordify();

            Assert.AreEqual(1, words.Count);
            Assert.AreEqual("first\tsecond", words.First());
        }

        /// <summary>
        /// Tests backslash will be counted as a character and escaped properly
        /// </summary>
        [Test]
        public void TestWordifyEscapedBackslash() {
            List<String> words = "first\\\\second".Wordify();

            Assert.AreEqual(1, words.Count);
            Assert.AreEqual("first\\second", words.First());
        }

        /// <summary>
        /// Tests backslash will be counted as a character and escaped properly
        /// </summary>
        [Test]
        public void TestWordifyQuoteWithinQuote() {
            List<String> words = "\"first \\\"second\" third".Wordify();

            Assert.AreEqual(2, words.Count);
            Assert.AreEqual("first \"second", words.First());
            Assert.AreEqual("third", words.Last());
        }
    }
}
