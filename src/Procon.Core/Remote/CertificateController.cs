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
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Procon.Core.Shared;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;
using Procon.Service.Shared;

namespace Procon.Core.Remote {
    /// <summary>
    /// Manages loading and holding the X509Certificate object
    /// </summary>
    public class CertificateController : CoreController, ICertificateController {
        public X509Certificate2 Certificate { get; set; }

        public SharedReferences Shared { get; private set; }

        /// <summary>
        /// Initializes the certificate with the default values.
        /// </summary>
        public CertificateController() {
            this.Shared = new SharedReferences();
        }

        public override ICoreController Execute() {
            this.Load();

            return base.Execute();
        }

        /// <summary>
        /// Loads the certificate, supplying details about the location and password of the certificate
        /// from the variables controller.
        /// </summary>
        /// <returns></returns>
        public bool Load() {
            return this.Load(this.Shared.Variables.Get(CommonVariableNames.CommandServerCertificatePath, Defines.CertificatesDirectoryCommandServerPfx.FullName), this.Shared.Variables.Get<String>(CommonVariableNames.CommandServerCertificatePassword));
        }

        /// <summary>
        /// Loads a certificate at the supplied path with the supplied password.
        /// </summary>
        /// <param name="path">The path to the certificate file.</param>
        /// <param name="password">The password of the certificate.</param>
        /// <returns>True if the certificate loaded correctly, false otherwise.</returns>
        public bool Load(String path, String password = null) {
            if (File.Exists(path) == true) {
                try {
                    this.Certificate = password != null ? new X509Certificate2(path, password) : new X509Certificate2(path);
                }
                catch (CryptographicException e) {
                    this.Certificate = null;

                    this.Shared.Events.Log(new GenericEvent() {
                        Message = String.Format("Error loading certificate @ path \"{0}\" \"{1}\".", path, e.Message),
                        GenericEventType = GenericEventType.CommandServerStarted,
                        Success = false,
                        CommandResultType = CommandResultType.Failed
                    });
                }
            }
            else {
                // Panic, no certificate exists. Cannot start server.
                this.Shared.Events.Log(new GenericEvent() {
                    Message = String.Format("Command server certificate @ path \"{0}\" does not exists.", path),
                    GenericEventType = GenericEventType.CommandServerStarted,
                    Success = false,
                    CommandResultType = CommandResultType.Failed
                });
            }

            return this.Certificate != null;
        }
    }
}
