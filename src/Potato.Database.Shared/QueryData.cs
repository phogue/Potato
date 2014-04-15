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
    /// The base shared data between Compiled and Parsed data.
    /// </summary>
    public abstract class QueryData : IQueryData {
        /// <summary>
        /// The number of documents to read or edit before ignoring.
        /// </summary>
        public long? Limit { get; set; }

        /// <summary>
        /// The number of documents to ignore before reading or editing.
        /// </summary>
        public long? Skip { get; set; }

        /// <summary>
        /// The method used in the SQL (SELECT, INSERT, UPDATE, DELETE)
        /// </summary>
        public List<String> Methods { get; set; }

        /// <summary>
        /// The databases to query against
        /// </summary>
        public List<String> Databases { get; set; }

        /// <summary>
        /// The fields to select from the collections
        /// </summary>
        public List<String> Fields { get; set; }

        /// <summary>
        /// The values to assign to fields
        /// </summary>
        public List<String> Values { get; set; }

        /// <summary>
        /// The fields used to when assigning a value to a field (update, insert)
        /// </summary>
        public List<String> Assignments { get; set; }

        /// <summary>
        /// The indices to apply when the alter/create
        /// </summary>
        public List<String> Indices { get; set; }

        /// <summary>
        /// The conditions placed on a select, update or delete method
        /// </summary>
        public List<String> Conditions { get; set; }

        /// <summary>
        /// The collections placed on a select, update, delete or insert method
        /// </summary>
        public List<String> Collections { get; set; }

        /// <summary>
        /// What fields and direction to sort by.
        /// </summary>
        public List<String> Sortings { get; set; }

        /// <summary>
        /// The base element in the query being serialized.
        /// </summary>
        public IDatabaseObject Root { get; set; }

        protected QueryData() {
            this.Methods = new List<String>();
            this.Databases = new List<String>();
            this.Fields = new List<String>();
            this.Values = new List<String>();
            this.Assignments = new List<String>();
            this.Indices = new List<String>();
            this.Conditions = new List<String>();
            this.Collections = new List<String>();
            this.Sortings = new List<String>();
            this.Root = null;
        }
    }
}
