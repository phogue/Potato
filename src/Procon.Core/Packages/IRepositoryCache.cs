using System;
using System.Collections.Concurrent;
using NuGet;
using Procon.Core.Shared.Models;

namespace Procon.Core.Packages {
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
        void Add(String uri);

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
