using System.Collections.Generic;
using System.Linq;
using NuGet;

namespace Procon.Core.Test.Packages.Mocks {
    public class MockPackageRepository : PackageRepositoryBase {

        protected IEnumerable<IPackage> Packages { get; set; }

        public MockPackageRepository() {
            this.Packages = new List<IPackage>();
        }

        public MockPackageRepository(IEnumerable<IPackage> packages) {
            this.Packages = packages;
        }

        public override IQueryable<IPackage> GetPackages() {
            return this.Packages.AsQueryable();
        }

        public override string Source {
            get { return ""; }
        }

        public override bool SupportsPrereleasePackages {
            get { return false; }
        }
    }
}
