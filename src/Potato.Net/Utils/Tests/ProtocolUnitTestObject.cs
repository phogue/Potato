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
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Potato.Net.Utils.Tests {

    [Serializable]
    public class ProtocolUnitTestObject : IDisposable {

        /// <summary>
        /// A basic text string with no regex.
        /// </summary>
        public string Text;

        [XmlIgnore]
        public Regex Regex { get; set; }

        /// <summary>
        /// Xml serialization for the TimeSpan (without wrapper since Potato.Fuzzy should
        /// not need to worry about such things.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement(DataType = "string", ElementName = "Regex")]
        public string RegexString {
            get {
                return Regex != null ? Regex.ToString() : string.Empty;
            }
            set {
                Regex = string.IsNullOrEmpty(value) == false ? new Regex(value, RegexOptions.Compiled) : null;
            }
        }

        /// <summary>
        /// The plain text or regular expression to use when replacing.
        /// </summary>
        [OptionalField]
        public string Replace;

        /// <summary>
        /// The plain text or regular expression to use when replacing.
        /// </summary>
        [OptionalField]
        public ProtocolUnitRandomObject Random;

        public void ReplaceWith(ProtocolUnitTestObject replacement) {

            if (string.IsNullOrEmpty(replacement.Text) == false) {
                if (string.IsNullOrEmpty(Text) == false) {

                    Text = Text.Replace(replacement.Text, replacement.Random != null ? replacement.Random.ToString() : replacement.Replace);
                }
            }
        }

        public bool Matches(string text) {
            var matches = false;

            if (string.IsNullOrEmpty(Text) == false) {
                matches = string.CompareOrdinal(Text, text) == 0;
            }
            else if (Regex != null) {
                matches = Regex.IsMatch(text);
            }

            return matches;
        }

        public void Dispose() {
            Text = null;
            Regex = null;
            Replace = null;
            if (Random != null) Random.Dispose();
        }
    }
}
