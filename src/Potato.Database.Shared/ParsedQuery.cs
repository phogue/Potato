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

namespace Potato.Database.Shared {
    /// <summary>
    /// The initial parse to collection peices of information but
    /// remain in a hierarchy
    /// </summary>
    [Serializable]
    public class ParsedQuery : QueryData, IParsedQuery {
        /// <summary>
        /// List of parsed child queries.
        /// </summary>
        public List<IParsedQuery> Children { get; set; }

        /// <summary>
        /// Initializes all defaults
        /// </summary>
        public ParsedQuery() : base() {
            this.Children = new List<IParsedQuery>();
        }
    }
}
