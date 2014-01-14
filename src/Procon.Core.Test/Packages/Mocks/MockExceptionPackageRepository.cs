using System;
using System.Linq;
using NuGet;

namespace Procon.Core.Test.Packages.Mocks {
    public class MockExceptionPackageRepository : PackageRepositoryBase {
        public override IQueryable<IPackage> GetPackages() {
            throw new Exception("GetPackages Exception");
        }

        public override string Source {
            get { return ""; }
        }

        public override bool SupportsPrereleasePackages {
            get { return false; }
        }
    }
}
