using System;
using System.Collections.Generic;

namespace Procon.Service.Shared {
    /// <summary>
    /// Basic settings implementation for the service controller
    /// </summary>
    public class ServiceSettings : IServiceSettings {
        public bool ServiceUpdateCore { get; set; }
        public string PackagesDefaultSourceRepositoryUri { get; set; }

        public void ParseArguments(Dictionary<String, String> arguments) {
            // If we have not been told anything update updating core OR the update has been explictely set to true
            // default: check for update, unless "-updatecore false" is passed in.
            if (arguments.ContainsKey("serviceupdatecore") == true) {
                this.ServiceUpdateCore = ArgumentHelper.IsFalsey(arguments["serviceupdatecore"]) == false;
            }

            if (arguments.ContainsKey("packagesdefaultsourcerepositoryuri") == true) {
                this.PackagesDefaultSourceRepositoryUri = arguments["packagesdefaultsourcerepositoryuri"];
            }
        }

        /// <summary>
        /// Initializes the settings with the default values.
        /// </summary>
        public ServiceSettings() : this(new List<String>()) { }

        /// <summary>
        /// Initializes the settings with the default values and processes input as arguments
        /// </summary>
        public ServiceSettings(IList<string> input) {
            this.ServiceUpdateCore = true;
            this.PackagesDefaultSourceRepositoryUri = Defines.PackagesDefaultSourceRepositoryUri;

            this.ParseArguments(ArgumentHelper.ToArguments(input));
        }
    }
}
