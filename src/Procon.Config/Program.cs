using System;
using Procon.Config.Core;
using Procon.Service.Shared;

namespace Procon.Config {
    /// <summary>
    /// Entry point for single command execution on the procon config.
    /// Load up the procon instance, run a command, shut it down (saving the config)
    /// </summary>
    /// <remarks>
    /// Really this is just supposed to run initially by hosts to create the certificate
    /// and initial accounts for the user. 
    /// </remarks>
    public class Program {
        static void Main(string[] args) {
            var arguments = ArgumentHelper.ToArguments(args);

            if (arguments.ContainsKey(@"command")) {
                // Split the command/arguments.
                var command = arguments[@"command"];

                arguments.Remove(@"command");

                // Create a service to at least handle output to the console for signals.
                ServiceController service = new ServiceController {
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
                    }
                };

                // See if we can process the command without loading up the procon instance (e.g certificate generaton)
                var result = ConfigController.Dispatch(command, arguments);

                if (result != null) {
                    service.SignalMessage(result);
                }
                // Else we need to start up and execute a message on the Procon instance.
                else {
                    service.ExecuteMessage(new ServiceMessage() {
                        Name = command,
                        Arguments = arguments
                    });
                }

                // We've executed the command locally or on the procon instance, which may have altered the config files.
                // Now we dispose the service which will write the configs.
                service.Dispose();
            }
            else {
                Console.WriteLine(@"Missing command argument");
            }

            Console.ReadKey();
        }
    }
}
