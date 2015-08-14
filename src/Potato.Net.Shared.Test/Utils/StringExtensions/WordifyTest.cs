#region Copyright
// Copyright 2015 Geoff Green.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Potato.Net.Shared.Utils;

namespace Potato.Net.Shared.Test.Utils.StringExtensions {
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
            var words = "single".Wordify();

            Assert.AreEqual(1, words.Count);
            Assert.AreEqual("single", words.First());
        }

        /// <summary>
        /// Tests two simple words will be split into two simple words.
        /// </summary>
        [Test]
        public void TestWordifyDoubleWord() {
            var words = "first second".Wordify();

            Assert.AreEqual(2, words.Count);
            Assert.AreEqual("first", words.First());
            Assert.AreEqual("second", words.Last());
        }

        /// <summary>
        /// Tests three words, with the first two being quoted will yeild two words.
        /// </summary>
        [Test]
        public void TestWordifyTripleWordFirstTwoQuoted() {
            var words = "\"first second\" third".Wordify();

            Assert.AreEqual(2, words.Count);
            Assert.AreEqual("first second", words.First());
            Assert.AreEqual("third", words.Last());
        }

        /// <summary>
        /// Tests three words, with the first two being quoted will yeild two words.
        /// </summary>
        [Test]
        public void TestWordifyTripleWordLastTwoQuoted() {
            var words = "first \"second third\"".Wordify();

            Assert.AreEqual(2, words.Count);
            Assert.AreEqual("first", words.First());
            Assert.AreEqual("second third", words.Last());
        }

        /// <summary>
        /// Tests newline will be counted as a character and escaped properly
        /// </summary>
        [Test]
        public void TestWordifyEscapedNewLine() {
            var words = "first\\nsecond".Wordify();

            Assert.AreEqual(1, words.Count);
            Assert.AreEqual("first\nsecond", words.First());
        }

        /// <summary>
        /// Tests newline will be counted as a character and escaped properly
        /// </summary>
        [Test]
        public void TestWordifyEscapedCarriageReturn() {
            var words = "first\\rsecond".Wordify();

            Assert.AreEqual(1, words.Count);
            Assert.AreEqual("first\rsecond", words.First());
        }

        /// <summary>
        /// Tests tab will be counted as a character and escaped properly
        /// </summary>
        [Test]
        public void TestWordifyEscapedTab() {
            var words = "first\\tsecond".Wordify();

            Assert.AreEqual(1, words.Count);
            Assert.AreEqual("first\tsecond", words.First());
        }

        /// <summary>
        /// Tests backslash will be counted as a character and escaped properly
        /// </summary>
        [Test]
        public void TestWordifyEscapedBackslash() {
            var words = "first\\\\second".Wordify();

            Assert.AreEqual(1, words.Count);
            Assert.AreEqual("first\\second", words.First());
        }

        /// <summary>
        /// Tests backslash will be counted as a character and escaped properly
        /// </summary>
        [Test]
        public void TestWordifyQuoteWithinQuote() {
            var words = "\"first \\\"second\" third".Wordify();

            Assert.AreEqual(2, words.Count);
            Assert.AreEqual("first \"second", words.First());
            Assert.AreEqual("third", words.Last());
        }
    }
}
