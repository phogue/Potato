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
using System.Collections.Concurrent;
using NuGet;
using Potato.Core.Shared.Models;

namespace Potato.Core.Packages {
    /// <summary>
    /// Manages storage and caching of multiple repository sources.
    /// </summary>
    public interface IRepositoryCache {
        /// <summary>
        /// A list of repositories
        /// </summary>
        ConcurrentBag<RepositoryModel> Repositories { get; }

        /// <summary>
        /// When the cache was last rebuilt.
        /// </summary>
        DateTime? RebuiltTime { get; }

        /// <summary>
        /// Add a new repository, provided we don't already know about it.
        /// </summary>
        /// <param name="uri">The uri of the repository</param>
        void Add(string uri);

        /// <summary>
        /// Builds/Rebuilds the entire cache.
        /// </summary>
        /// <param name="localRepository">The nuget local repository to check for installed packages</param>
        void Build(IPackageRepository localRepository);

        /// <summary>
        /// Empty the cache of repositories
        /// </summary>
        void Clear();
    }
}
