using System;
using System.Collections.Generic;
using System.Threading;
using Procon.Properties;
using Procon.Service.Shared;

namespace Procon {
    using Procon.Service;

    internal class Program {

        [STAThread, LoaderOptimization(LoaderOptimization.MultiDomainHost)]
        private static void Main(string[] args) {

            Console.WriteLine(Resources.ConsoleHeader);
            
            if (args.Length > 0) {
                // Support for --help command?

                System.Console.WriteLine("Starting with arguments: {0}", String.Join(" ", args));
            }

            ServiceController service = new ServiceController {
                Arguments = new List<String>(args)
            };

            service.SignalMessage(new ServiceMessage() {
                Name = "help"
            });

            service.SignalMessage(new ServiceMessage() {
                Name = "start"
            });

            String input = String.Empty;

            do {
                input = Console.ReadLine();

                service.SignalMessage(new ServiceMessage() {
                    Name = input
                });

            } while (String.Compare(input, "exit", StringComparison.OrdinalIgnoreCase) != 0);

            service.SignalMessage(new ServiceMessage() {
                Name = "stop"
            });

            service.Dispose();

            System.Console.WriteLine("Closing..");
            Thread.Sleep(1000);
        }
    }
}