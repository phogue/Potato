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
using System.Globalization;
using System.Runtime.Serialization;
using Potato.Net.Shared.Utils;

namespace Potato.Net.Utils.Tests {
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
        protected string GeneratedText { get; set; }

        public override string ToString() {
            if (GeneratedText == null) {
                int minimum = Minimum, maximum = Maximum > 0 ? Maximum : 10;

                if (Type == ProtocolUnitRandomType.String) {
                    GeneratedText = StringExtensions.RandomString(maximum);
                }
                else if (Type == ProtocolUnitRandomType.Integer) {
                    var r = new Random();
                    GeneratedText = r.Next(minimum, maximum).ToString(CultureInfo.InvariantCulture);
                }
                else if (Type == ProtocolUnitRandomType.Float) {
                    var r = new Random();
                    GeneratedText = r.NextDouble().ToString(CultureInfo.InvariantCulture);
                }
            }

            return GeneratedText ?? string.Empty;
        }

        public void Dispose() {
            GeneratedText = null;
        }
    }
}
