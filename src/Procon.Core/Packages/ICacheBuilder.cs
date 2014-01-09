using System.Collections.Generic;
using NuGet;
using Procon.Core.Shared.Models;

namespace Procon.Core.Packages {
    /// <summary>
    /// Interface for building various information about the repository from a source.
    /// </summary>
    public interface ICacheBuilder {
        /// <summary>
        /// The repository to build the cache on.
        /// </summary>
        RepositoryModel Repository { get; set; }

        /// <summary>
        /// The repository to use as a reference.
        /// </summary>
        IList<IPackage> Packages { get; set; }

        /// <summary>
        /// Build the cache within the repository, appending known information to the repository packages.
        /// </summary>
        void Build();
    }
}
