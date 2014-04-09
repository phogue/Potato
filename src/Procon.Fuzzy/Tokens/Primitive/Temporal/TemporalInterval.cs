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
namespace Procon.Fuzzy.Tokens.Primitive.Temporal {
    public enum TemporalInterval {
        /// <summary>
        /// Loop forever (default)
        /// </summary>
        Infinite,

        /// <summary>
        /// Only on the first of something
        /// </summary>
        First,

        /// <summary>
        /// Only on the second of something
        /// </summary>
        Second,

        /// <summary>
        /// Only on the third of something
        /// </summary>
        Third,

        /// <summary>
        /// Only on the fourth of something
        /// </summary>
        Fourth
    }
}