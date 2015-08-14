#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
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
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Potato.Net.Shared.Utils;

namespace Potato.Net.Shared.Test.Utils.StringExtensions {

    /// <summary>
    /// Full of one-liner tests for the simple extension methods we have.
    /// </summary>
    [TestFixture]
    public class StringExtensionsTest {

        /// <summary>
        /// Tests calling random string with 20 calls won't produce a duplicate. If this simple test produces a
        /// collision then for our purposes 
        /// </summary>
        [Test]
        public void TestRandomStringSuccess() {
            var generated = new List<string>();

            for (var count = 0; count < 20; count++) {
                generated.Add(Shared.Utils.StringExtensions.RandomString(30));
            }

            Assert.AreEqual(20, generated.Distinct().Count());
        }

        /// <summary>
        /// Tests that a string will be split into multiple strings for wrapping
        /// </summary>
        [Test]
        public void TestWordWrapSuccess() {
            var wrapped = "Die Brösel eines Käsekuchen verzücken mich.".WordWrap(30);

            Assert.AreEqual(2, wrapped.Count);
            Assert.AreEqual("Die Brösel eines Käsekuchen", wrapped.First());
            Assert.AreEqual("verzücken mich.", wrapped.Last());
        }

        /// <summary>
        /// Tests that a string with diacritics is replaced.
        /// </summary>
        [Test]
        public void TestRemoveDiacriticsSuccess() {
            var removed = "Die Brösel eines Käsekuchen verzücken mich.".RemoveDiacritics();

            Assert.AreEqual("Die Brosel eines Kasekuchen verzucken mich.", removed);
        }

        /// <summary>
        /// Tests that a poxy leet speek text will be converted to something some-what legible.
        /// </summary>
        [Test]
        public void TestRemoveLeetSpeekSuccess() {
            var removed = "P]-[0gu3".RemoveLeetSpeek();

            Assert.AreEqual("PHOguE", removed);
        }

        /// <summary>
        /// Tests that a string with leet speek and diacritics will be stripped to a plain ASCII string
        /// </summary>
        [Test]
        public void TestStripSuccess() {
            var removed = "P]-[0gu3 Brösel".Strip();

            Assert.AreEqual("PHOguE Brosel", removed);
        }

        /// <summary>
        /// Tests a directory with forward slashes will be sanitized.
        /// </summary>
        [Test]
        public void TestSanitizeDirectoryForwardSlashes() {
            var sanitized = @"c:/projects/something/test/".SanitizeDirectory();

            Assert.AreEqual("c-projects-something-test", sanitized);
        }

        /// <summary>
        /// Tests a directory/file name with back slashes will be sanitized.
        /// </summary>
        [Test]
        public void TestSanitizeDirectoryBackSlashes() {
            var sanitized = @"c:\projects\something\test\".SanitizeDirectory();

            Assert.AreEqual("c-projects-something-test", sanitized);
        }

        /// <summary>
        /// Tests a url is slugged, but this is a little off at the moment. I would expect "forum-myrcon-com"
        /// instead of the current result.
        /// </summary>
        [Test]
        public void TestUrlSlug() {
            var slugged = "http://forum.myrcon.com".Slug();

            Assert.AreEqual("forum-myrcon-com", slugged);
        }
    }
}
