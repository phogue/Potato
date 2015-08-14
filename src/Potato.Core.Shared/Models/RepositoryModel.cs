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

namespace Potato.Core.Shared.Models {
    /// <summary>
    /// A nuget repository source known to Potato.core
    /// </summary>
    [Serializable]
    public class RepositoryModel : CoreModel {
        /// <summary>
        /// The base url of the repository
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Short directory safe url
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// The name of this repository
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of packages available in the repository
        /// </summary>
        public List<PackageWrapperModel> Packages { get; set; }

        /// <summary>
        /// If this is the location orphaned packages should be added if we cannot find a source for them.
        /// </summary>
        public bool IsOrphanage { get; set; }

        /// <summary>
        /// Stores the last error that occured during a cache rebuild.
        /// </summary>
        public string CacheError { get; set; }

        /// <summary>
        /// When the repository was last cached.
        /// </summary>
        public DateTime CacheStamp { get; set; }

        /// <summary>
        /// Initializes a repository model with the default values.
        /// </summary>
        public RepositoryModel() : base() {
            Packages = new List<PackageWrapperModel>();
        }
    }
}
