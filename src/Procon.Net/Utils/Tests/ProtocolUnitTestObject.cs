using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Procon.Net.Utils.Tests {

    [Serializable]
    public class ProtocolUnitTestObject : IDisposable {

        /// <summary>
        /// A basic text string with no regex.
        /// </summary>
        public String Text;

        [XmlIgnore]
        public Regex Regex { get; set; }

        /// <summary>
        /// Xml serialization for the TimeSpan (without wrapper since Procon.Fuzzy should
        /// not need to worry about such things.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement(DataType = "string", ElementName = "Regex")]
        public String RegexString {
            get {
                return this.Regex != null ? this.Regex.ToString() : String.Empty;
            }
            set {
                this.Regex = String.IsNullOrEmpty(value) == false ? new Regex(value, RegexOptions.Compiled) : null;
            }
        }

        /// <summary>
        /// The plain text or regular expression to use when replacing.
        /// </summary>
        [OptionalField]
        public String Replace;

        /// <summary>
        /// The plain text or regular expression to use when replacing.
        /// </summary>
        [OptionalField]
        public ProtocolUnitRandomObject Random;

        public void ReplaceWith(ProtocolUnitTestObject replacement) {

            if (String.IsNullOrEmpty(replacement.Text) == false) {
                if (String.IsNullOrEmpty(this.Text) == false) {

                    this.Text = this.Text.Replace(replacement.Text, replacement.Random != null ? replacement.Random.ToString() : replacement.Replace);
                }
            }
        }

        public bool Matches(String text) {
            bool matches = false;

            if (String.IsNullOrEmpty(this.Text) == false) {
                matches = System.String.CompareOrdinal(this.Text, text) == 0;
            }
            else if (this.Regex != null) {
                matches = this.Regex.IsMatch(text);
            }

            return matches;
        }

        public void Dispose() {
            this.Text = null;
            this.Regex = null;
            this.Replace = null;
            if (this.Random != null) this.Random.Dispose();
        }
    }
}
