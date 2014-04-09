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
using System.Globalization;
using System.Runtime.Serialization;
using Procon.Net.Shared.Utils;

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
