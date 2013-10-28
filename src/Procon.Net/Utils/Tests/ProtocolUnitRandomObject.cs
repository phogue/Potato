using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Procon.Net.Utils.Tests {
    public class ProtocolUnitRandomObject : IDisposable {

        /// <summary>
        /// The type of random value to generate
        /// </summary>
        public ProtocolUnitRandomType Type { get; set; }

        [OptionalField]
        public int Minimum;

        [OptionalField]
        public int Maximum;

        /// <summary>
        /// The generated text. This is only generated when required and then saved for this object.
        /// </summary>
        protected String GeneratedText { get; set; }

        public override string ToString() {
            if (this.GeneratedText == null) {
                int minimum = this.Minimum, maximum = this.Maximum > 0 ? this.Maximum : 10;

                if (this.Type == ProtocolUnitRandomType.String) {
                    this.GeneratedText = StringExtensions.RandomString(maximum);
                }
                else if (this.Type == ProtocolUnitRandomType.Integer) {
                    Random r = new Random();
                    this.GeneratedText = r.Next(minimum, maximum).ToString(CultureInfo.InvariantCulture);
                }
                else if (this.Type == ProtocolUnitRandomType.Float) {
                    Random r = new Random();
                    this.GeneratedText = r.NextDouble().ToString(CultureInfo.InvariantCulture);
                }
            }

            return this.GeneratedText ?? String.Empty;
        }

        public void Dispose() {
            this.GeneratedText = null;
        }
    }
}
