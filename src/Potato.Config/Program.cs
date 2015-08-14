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
using Potato.Config.Core;
using Potato.Service.Shared;

namespace Potato.Config {
    /// <summary>
    /// Entry point for single command execution on the Potato config.
    /// Load up the Potato instance, run a command, shut it down (saving the config)
    /// </summary>
    /// <remarks>
    /// Really this is just supposed to run initially by hosts to create the certificate
    /// and initial accounts for the user. 
    /// </remarks>
    public static class Program {
        static void Main(string[] args) {
            var arguments = ArgumentHelper.ToArguments(args);

            if (arguments.ContainsKey(@"command")) {
                // Split the command/arguments.
                var command = arguments[@"command"];

                arguments = ArgumentHelper.ScrubAlphaNumericKeys(arguments);

                // Create a service to at least handle output to the console for signals.
                var service = new ServiceController {
                    Settings = {
                        // Force no update checks. We're just here to configure.
                        ServiceUpdateCore = false
                    },
                    SignalBegin = (controller, message) => Console.WriteLine(@"Signal: {0}", message.Name),
                    SignalEnd = (controller, message, seconds) => Console.WriteLine(@"Signal: {0} completed in {1} seconds", message.Name, seconds),
                    WriteServiceConfigBegin = controller => Console.Write(@"Writing config.. "),
                    WriteServiceConfigEnd = controller => Console.WriteLine(@"Complete"),
                    DisposeServiceBegin = controller => Console.Write(@"Disposing service.. "),
                    DisposeServiceEnd = controller => Console.WriteLine(@"Complete"),
                    UnloadServiceBegin = controller => Console.Write(@"Unloading service domain.. "),
                    UnloadServiceEnd = controller => Console.WriteLine(@"Complete"),
                    Observer = {
                        StatusChange = (observer, type) => Console.WriteLine(@"Status: {0}", type.ToString())
                    },
                    SignalResult = (controller, message) => {
                        foreach (var item in message.Arguments) {
                            Console.WriteLine(@"{0}: {1}", item.Key, item.Value);
                        }
                    },
                    Arguments = new List<string>(args)
                };

                // See if we can process the command without loading up the Potato instance (e.g certificate generaton)
                var result = ConfigController.Dispatch(command, arguments);

                if (result != null) {
                    service.SignalMessage(result);
                }
                // Else we need to start up and execute a message on the Potato instance.
                else {
                    service.ExecuteMessage(new ServiceMessage() {
                        Name = command,
                        Arguments = arguments
                    });
                }

                // We've executed the command locally or on the Potato instance, which may have altered the config files.
                // Now we dispose the service which will write the configs.
                service.Dispose();
            }
            else {
                Console.WriteLine(@"Missing command argument");
            }
        }
    }
}
