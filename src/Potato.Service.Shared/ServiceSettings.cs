#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;

namespace Potato.Service.Shared {
    /// <summary>
    /// Basic settings implementation for the service controller
    /// </summary>
    public class ServiceSettings : IServiceSettings {
        public bool ServiceUpdateCore { get; set; }
        public int ServicePollingTimeout { get; set; }
        public int WriteServiceConfigTimeout { get; set; }
        public int DisposeServiceTimeout { get; set; }
        public string PackagesDefaultSourceRepositoryUri { get; set; }

        public void ParseArguments(IDictionary<String, String> arguments) {
            // If we have not been told anything update updating core OR the update has been explictely set to true
            // default: check for update, unless "-updatecore false" is passed in.
            if (arguments.ContainsKey("serviceupdatecore") == true) {
                this.ServiceUpdateCore = ArgumentHelper.IsFalsey(arguments["serviceupdatecore"]) == false;
            }

            if (arguments.ContainsKey("servicepollingtimeout") == true) {
                // Attempt to parse the value or maintain existing value if an error occurs.
                this.ServicePollingTimeout = (int)ArgumentHelper.ParseNumeric(arguments["servicepollingtimeout"], this.ServicePollingTimeout);
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
            this.ServicePollingTimeout = Defines.DefaultServicePollingTimeout;
            this.WriteServiceConfigTimeout = Defines.DefaultWriteServiceConfigTimeout;
            this.DisposeServiceTimeout = Defines.DefaultDisposeServiceTimeout;
            this.PackagesDefaultSourceRepositoryUri = Defines.PackagesDefaultSourceRepositoryUri;

            this.ParseArguments(ArgumentHelper.ToArguments(input));
        }
    }
}
