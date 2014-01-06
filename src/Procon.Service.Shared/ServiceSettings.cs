﻿using System;
using System.Collections.Generic;

namespace Procon.Service.Shared {
    /// <summary>
    /// Basic settings implementation for the service controller
    /// </summary>
    public class ServiceSettings : IServiceSettings {
        public bool ServiceUpdateCore { get; set; }

        public void ParseArguments(Dictionary<String, String> arguments) {
            // If we have not been told anything update updating core OR the update has been explictely set to true
            // default: check for update, unless "-updatecore false" is passed in.
            if (arguments.ContainsKey("serviceupdatecore") == true) {
                this.ServiceUpdateCore = ArgumentHelper.IsFalsey(arguments["serviceupdatecore"]) == false;
            }
        }

        /// <summary>
        /// Initializes the settings with the default values.
        /// </summary>
        public ServiceSettings() {
            this.ServiceUpdateCore = true;
        }

        /// <summary>
        /// Initializes the settings with the default values and processes input as arguments
        /// </summary>
        public ServiceSettings(IList<string> input) {
            this.ServiceUpdateCore = true;

            this.ParseArguments(ArgumentHelper.ToArguments(input));
        }
    }
}
