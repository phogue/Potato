#region Copyright
// Copyright 2015 Geoff Green.
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

        public void ParseArguments(IDictionary<string, string> arguments) {
            // If we have not been told anything update updating core OR the update has been explictely set to true
            // default: check for update, unless "-updatecore false" is passed in.
            if (arguments.ContainsKey("serviceupdatecore") == true) {
                ServiceUpdateCore = ArgumentHelper.IsFalsey(arguments["serviceupdatecore"]) == false;
            }

            if (arguments.ContainsKey("servicepollingtimeout") == true) {
                // Attempt to parse the value or maintain existing value if an error occurs.
                ServicePollingTimeout = (int)ArgumentHelper.ParseNumeric(arguments["servicepollingtimeout"], ServicePollingTimeout);
            }

            if (arguments.ContainsKey("packagesdefaultsourcerepositoryuri") == true) {
                PackagesDefaultSourceRepositoryUri = arguments["packagesdefaultsourcerepositoryuri"];
            }
        }

        /// <summary>
        /// Initializes the settings with the default values.
        /// </summary>
        public ServiceSettings() : this(new List<string>()) { }

        /// <summary>
        /// Initializes the settings with the default values and processes input as arguments
        /// </summary>
        public ServiceSettings(IList<string> input) {
            ServiceUpdateCore = true;
            ServicePollingTimeout = Defines.DefaultServicePollingTimeout;
            WriteServiceConfigTimeout = Defines.DefaultWriteServiceConfigTimeout;
            DisposeServiceTimeout = Defines.DefaultDisposeServiceTimeout;
            PackagesDefaultSourceRepositoryUri = Defines.PackagesDefaultSourceRepositoryUri;

            ParseArguments(ArgumentHelper.ToArguments(input));
        }
    }
}
