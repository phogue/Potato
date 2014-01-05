using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Service.Shared;

namespace Procon.Core.Packages {
    public class PackagesController : CoreController {

        /// <summary>
        /// A list of repositories we are connected to
        /// </summary>
        List<RepositoryModel> Repositories { get; set; }  

        public PackagesController() {
            this.Repositories = new List<RepositoryModel>();

            var repository = PackageRepositoryFactory.Default.CreateRepository("http://localhost:30505/nuget");
            
            var manager = new PackageManager(repository, Defines.PackagesDirectory);

            var packages = manager.SourceRepository.GetPackages();
            // manager.InstallPackage("Myrcon.Procon");
            // manager.InstallPackage("Procon.Core");
            
            var x = 0;
        }
    }
}
