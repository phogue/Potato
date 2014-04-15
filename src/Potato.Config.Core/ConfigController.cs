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
using System.Linq;
using Potato.Config.Core.Models;
using Potato.Service.Shared;

namespace Potato.Config.Core {
    /// <summary>
    /// Handles commands that do not need to be dispatched to an active Potato instance.
    /// </summary>
    /// <remarks>
    /// It would be handy for this to inherit from CoreController, but instead we
    /// need to implement the basics ourselves.
    /// </remarks>
    public static class ConfigController {
        /// <summary>
        /// Generates a server pfx file used by the command server with the specified password within the arguments.
        /// </summary>
        /// <param name="arguments">
        ///     <para>The arguments for this command</para>
        ///     <para>Expecting "password", but it is optional. If no password is supplied a random password will be generated</para>
        /// </param>
        public static ServiceMessage CommandServerGenerateCertificate(IDictionary<String, String> arguments) {
            var model = new CertificateModel();

            if (arguments != null && arguments.Count > 0) {
                model.Password = arguments.First().Value;
            }
            else {
                model.RandomizePassword();
            }

            model.Generate();

            return new ServiceMessage() {
                Name = "result",
                Arguments = new Dictionary<String, String>() {
                    { "Command", "CommandServerCreateCertificate" },
                    { "Success", model.Exists.ToString() },
                    { "Message", String.Format("Created certificate with password: {0}", model.Password) },
                    { "Password", model.Password }
                }
            };
        }

        /// <summary>
        /// Updates the core of Potato. This won't validate that an update
        /// is successful, it just goes through the motions of updating.
        /// </summary>
        public static ServiceMessage ServiceUpdateCore() {
            ServiceController service = new ServiceController {
                Settings = {
                    ServiceUpdateCore = true
                }
            };

            service.UpdateCore();

            return new ServiceMessage() {
                Name = "result",
                Arguments = new Dictionary<String, String>() {
                    { "Command", "ServiceUpdateCore" },
                    { "Success", "True" }
                }
            };
        }

        /// <summary>
        /// Dispatches a very simple command to a handler within this controller.
        /// </summary>
        /// <param name="command">The command to be executed</param>
        /// <param name="arguments">Any arguments attached to this command.</param>
        public static ServiceMessage Dispatch(String command, IDictionary<String, String> arguments) {
            ServiceMessage result = null;

            if (String.Compare(command, "CommandServerGenerateCertificate", StringComparison.OrdinalIgnoreCase) == 0) {
                result = ConfigController.CommandServerGenerateCertificate(arguments);
            }
            else if (String.Compare(command, "ServiceUpdateCore", StringComparison.OrdinalIgnoreCase) == 0) {
                result = ConfigController.ServiceUpdateCore();
            }

            return result;
        }
    }
}
