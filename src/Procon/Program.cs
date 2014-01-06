using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Procon.Properties;
using Procon.Service.Shared;

namespace Procon {
    internal class Program {

        [STAThread, LoaderOptimization(LoaderOptimization.MultiDomainHost)]
        private static void Main(string[] args) {

            Console.WriteLine(Resources.ConsoleHeader);
            
            if (args.Length > 0) {
                // Support for --help command?

                System.Console.WriteLine(@"Starting with arguments: {0}", String.Join(" ", args));
            }

            ServiceController service = new ServiceController {
                Arguments = new List<String>(args),
                Settings = new ServiceSettings(new List<String>(args))
            };

            service.SignalMessage(new ServiceMessage() {
                Name = "help"
            });

            service.SignalMessage(new ServiceMessage() {
                Name = "start"
            });

            var input = String.Empty;

            do {
                input = Console.ReadLine();
                
                if (input != null) {
                    var words = Regex.Matches(input, @"([^\s]*""[^""]+""[^\s]*)|[^""]?\w+[^""]?")
                        .Cast<Match>()
                        .Select(match => match.Captures[0].Value.Trim('"', ' '))
                        .ToList();

                    service.SignalMessage(new ServiceMessage() {
                        Name = words.FirstOrDefault(),
                        Arguments = ArgumentHelper.ToArguments(words.Skip(1).ToList())
                    });
                }

            } while (String.Compare(input, "exit", StringComparison.OrdinalIgnoreCase) != 0);

            service.SignalMessage(new ServiceMessage() {
                Name = "stop"
            });

            service.Dispose();

            System.Console.WriteLine(@"Closing..");
            Thread.Sleep(1000);
        }
    }
}