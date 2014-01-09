﻿using System.Collections.Generic;
using System.Linq;
using NuGet;
using Procon.Core.Shared.Models;

namespace Procon.Core.Packages {
    /// <summary>
    /// Updates existing package wrappers with those found in the local source
    /// </summary>
    public class InstalledCacheBuilder : ICacheBuilder {
        public RepositoryModel Repository { get; set; }

        public IList<IPackage> Packages { get; set; }

        public void Build() {
            foreach (var installedPackage in this.Packages) {
                PackageWrapperModel packageWrapper = this.Repository.Packages.FirstOrDefault(pack => pack.Id == installedPackage.Id);

                if (packageWrapper != null) {
                    packageWrapper.Installed = PackageFactory.CreatePackageModelFromNugetPackage(installedPackage);
                    packageWrapper.State = installedPackage.Version.CompareTo(new SemanticVersion(packageWrapper.Available.Version)) < 0 ? PackageState.UpdateAvailable : PackageState.Installed;
                }
                // else we don't actually know if the package belongs to us or not, so we can't add it.
            }
        }
    }
}