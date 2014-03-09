using System;
using System.Collections.Generic;
using System.Linq;
using Procon.Config.Core.Models;
using Procon.Service.Shared;

namespace Procon.Config.Core {
    /// <summary>
    /// Handles commands that do not need to be dispatched to an active procon instance.
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
        public static ServiceMessage CommandServerGenerateCertificate(Dictionary<String, String> arguments) {
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
        /// Dispatches a very simple command to a handler within this controller.
        /// </summary>
        /// <param name="command">The command to be executed</param>
        /// <param name="arguments">Any arguments attached to this command.</param>
        public static ServiceMessage Dispatch(String command, Dictionary<String, String> arguments) {
            ServiceMessage result = null;

            if (String.Compare(command, "CommandServerGenerateCertificate", StringComparison.OrdinalIgnoreCase) == 0) {
                result = ConfigController.CommandServerGenerateCertificate(arguments);
            }

            return result;
        }
    }
}
