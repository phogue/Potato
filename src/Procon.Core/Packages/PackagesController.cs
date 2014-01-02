using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;

namespace Procon.Core.Packages {
    public class PackagesController : CoreController {

        /// <summary>
        /// A list of repositories we are connected to
        /// </summary>
        List<RepositoryModel> Repositories { get; set; }  

        public PackagesController() {
            this.Repositories = new List<RepositoryModel>();

        }
    }
}
