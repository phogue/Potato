using System;
using System.Collections.Generic;
using NuGet;
using Procon.Core.Shared.Models;

namespace Procon.Core.Packages {
    /// <summary>
    /// Series of helpers to convert between Nuget packages and Procon packages.
    /// </summary>
    public static class PackageFactory {
        /// <summary>
        /// Takes a NuGet package and creates a new PackageModel with similar data
        /// </summary>
        /// <remarks>IPackage isn't used outside of Procon.Core and Procon.Service, plugins do not know the type.</remarks>
        /// <param name="source">The source package to extract information</param>
        /// <returns>A new PackageModel with the details taken from the source.</returns>
        public static PackageModel CreatePackageModelFromNugetPackage(IPackage source) {
            return new PackageModel() {
                Authors = new List<String>(source.Authors),
                Copyright = source.Copyright,
                Description = source.Description,
                IconUrl = source.IconUrl != null ? source.IconUrl.OriginalString : "",
                Id = source.Id,
                Language = source.Language,
                LicenseUrl = source.LicenseUrl != null ? source.LicenseUrl.OriginalString : "",
                Owners = new List<String>(source.Owners),
                ProjectUrl = source.ProjectUrl != null ? source.ProjectUrl.OriginalString : "",
                ReleaseNotes = source.ReleaseNotes,
                RequireLicenseAcceptance = source.RequireLicenseAcceptance,
                Summary = source.Summary,
                Tags = source.Tags,
                Title = source.Title,
                Version = source.Version != null ? source.Version.ToString() : "",
                Listed = source.Listed,
                Published = source.Published
            };
        }
    }
}
