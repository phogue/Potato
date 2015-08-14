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
namespace Potato.Fuzzy.Tokens.Primitive.Temporal {
    public enum TimeType {
        /// <summary>
        /// No specifics given for this time
        /// </summary>
        None,

        /// <summary>
        /// The date/time should be considered a single moment regardless of the current
        /// date/time.
        /// </summary>
        Definitive,

        /// <summary>
        /// The date/time should be used as an offset +/- for now
        /// </summary>
        Relative
    }
}