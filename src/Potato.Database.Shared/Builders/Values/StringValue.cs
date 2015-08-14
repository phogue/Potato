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

namespace Potato.Database.Shared.Builders.Values {
    /// <summary>
    /// A simple string value
    /// </summary>
    [Serializable]
    public class StringValue : Value {

        /// <summary>
        /// The text attached to this object
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Casts the text into a simple object
        /// </summary>
        /// <returns></returns>
        public override object ToObject() {
            return Data;
        }
    }
}
