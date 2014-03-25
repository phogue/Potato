﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NuGet;
using Procon.Core.Shared.Models;
using Procon.Net.Shared.Utils;
using Procon.Service.Shared;

namespace Procon.Core.Packages {
    /// <summary>
    /// Implementation of the repository cache. Maintains a package orphanage.
    /// </summary>
    public class RepositoryCache : IRepositoryCache {
        public ConcurrentBag<RepositoryModel> Repositories { get; set; }
        public DateTime? RebuiltTime { get; set; }

        /// <summary>
        /// Where to send packages when they are installed but we don't know what repository they belong to.
        /// </summary>
        protected RepositoryModel PackageOrphanage { get; set; }

        /// <summary>
        /// A dictionary of source repositories from the list of repositories. Used as a cache.
        /// </summary>
        public ConcurrentDictionary<String, IPackageRepository> SourceRepositories { get; set; } 

        /// <summary>
        /// Initializes the repository cache with the default values.
        /// </summary>
        public RepositoryCache() {
            this.PackageOrphanage = new RepositoryModel() {
                IsOrphanage = true
            };

            this.Repositories = new ConcurrentBag<RepositoryModel>() {
                this.PackageOrphanage
            };

            this.SourceRepositories = new ConcurrentDictionary<String, IPackageRepository>();
        }

        /// <summary>
        /// Checks if enough time has elapsed since the last build, just so we don't
        /// end up spamming he source repositories.
        /// </summary>
        /// <returns>True if we can rebuild the cache, false if we should hold off.</returns>
        public bool IsCacheBuildable() {
            return this.RebuiltTime.HasValue == false || DateTime.Now.AddSeconds(-20) > this.RebuiltTime;
        }

        /// <summary>
        /// Fetches a cached source repository based on the uri
        /// </summary>
        /// <param name="uri">The uri to search for a cached package repository</param>
        /// <returns>The cached or newly cached repository</returns>
        public IPackageRepository GetCachedSourceRepository(String uri) {
            IPackageRepository repository = null;
            
            if (this.SourceRepositories.TryGetValue(uri, out repository) == false) {
                repository = PackageRepositoryFactory.Default.CreateRepository(uri);

                this.SourceRepositories.TryAdd(uri, repository);
            }

            return repository;
        }

        public void Add(String uri) {
            var sluggedUri = uri.Slug();

            if (this.Repositories.Any(repository => repository.Slug == sluggedUri) == false) {
                this.Repositories.Add(new RepositoryModel() {
                    Name = uri,
                    Uri = uri,
                    Slug = sluggedUri
                });
            }
        }

        public void Build(IPackageRepository localRepository) {
            if (this.IsCacheBuildable() == true) {
                foreach (RepositoryModel repository in this.Repositories.Where(repository => String.IsNullOrEmpty(repository.Uri) == false)) {
                    var cache = new List<PackageWrapperModel>();

                    repository.CacheStamp = DateTime.Now;

                    try {
                        // Append all available packages for this repository.
                        new AvailableCacheBuilder() {
                            Cache = cache,
                            Source = this.GetCachedSourceRepository(repository.Uri)
                                           .GetPackages()
                                           .Where(package => package.Tags != null && package.Tags.Contains(Defines.PackageRequiredTag) && package.IsLatestVersion == true)
                                           .ToList()
                                           .OrderByDescending(pack => pack.Version)
                                           .ToList()
                        }.Build();

                        // Update all available packages with those that are installed.
                        new InstalledCacheBuilder() {
                            Cache = cache,
                            Source = localRepository
                                .GetPackages()
                                .ToList()
                        }.Build();

                        // No errors occured during fetch, null it out.
                        repository.CacheError = null;

                        repository.Packages = cache;
                    }
                    catch (Exception e) {
                        // Record the error that occured while fetching the packages.
                        repository.CacheError = e.Message;
                    }
                }

                // A list of package id's that we know the source of
                IEnumerable<String> availablePackageIds = this.Repositories.SelectMany(repository => repository.Packages).Select(packageWrapper => packageWrapper.Id);

                // Now orphan all remaining packages that are installed but do not belong to any repository.
                new OrphanedCacheBuilder() {
                    Cache = this.Repositories.First(repository => repository.IsOrphanage == true).Packages,
                    Source = localRepository
                        .GetPackages()
                        .Where(package => availablePackageIds.Contains(package.Id) == false)
                        .ToList()
                }.Build();

                this.RebuiltTime = DateTime.Now;
            }
        }

        public void Clear() {
            while (this.Repositories.IsEmpty == false) {
                RepositoryModel result;
                this.Repositories.TryTake(out result);
            }

            this.Repositories.Add(this.PackageOrphanage);
        }
    }
}
