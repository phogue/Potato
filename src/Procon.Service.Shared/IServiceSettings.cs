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
        /// The main source repository uri to download core updates from.
        /// </summary>
        String PackagesDefaultSourceRepositoryUri { get; set; }

        /// <summary>
        /// Processes the command line arguments
        /// </summary>
        /// <param name="arguments">The arguments from the command line "-key value"</param>
        void ParseArguments(Dictionary<String, String> arguments);
    }
}
