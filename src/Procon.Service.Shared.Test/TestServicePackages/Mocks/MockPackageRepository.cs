using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;

namespace Procon.Service.Shared.Test.TestServicePackages.Mocks {
    public class MockPackageRepository : PackageRepositoryBase {

        protected IEnumerable<IPackage> Packages { get; set; }

        public String Uri { get; set; }

        public MockPackageRepository() : this(new List<IPackage>()) { }

        public MockPackageRepository(IEnumerable<IPackage> packages) {
            this.Uri = "";
            this.Packages = packages;
        }

        public override IQueryable<IPackage> GetPackages() {
            return this.Packages.AsQueryable();
        }

        public override String Source {
            get { return this.Uri; }
        }

        public override bool SupportsPrereleasePackages {
            get { return false; }
        }
    }
}
