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

namespace Procon.Service.Shared {
    /// <summary>
    /// Settings interface for controlling the service.
    /// </summary>
    public interface IServiceSettings {
        /// <summary>
        /// If the service should check & update the core of Procon before it starts
        /// </summary>
        bool ServiceUpdateCore { get; set; }

        /// <summary>
        /// How long the service should wait for a message from the instance before restarting
        /// </summary>
        int ServicePollingTimeout { get; set; }

        /// <summary>
        /// The maximum time to wait when saving the config for Procon.
        /// </summary>
        int WriteServiceConfigTimeout { get; set; }

        /// <summary>
        /// The maximum time to wait when disposing objects before collapsing the AppDomain.
        /// </summary>
        int DisposeServiceTimeout { get; set; }

        /// <summary>
        /// The main source repository uri to download core updates from.
        /// </summary>
        String PackagesDefaultSourceRepositoryUri { get; set; }

        /// <summary>
        /// Processes the command line arguments
        /// </summary>
        /// <param name="arguments">The arguments from the command line "-key value"</param>
        void ParseArguments(IDictionary<String, String> arguments);
    }
}
