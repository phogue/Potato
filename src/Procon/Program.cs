using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Procon.Properties;
using Procon.Service.Shared;

namespace Procon {
    internal class Program {

        private static Dictionary<String, String> Arguments(IList<String> input) {
            Dictionary<String, String> arguments = new Dictionary<String, String>();

            for (int offset = 0; offset < input.Count; offset++) {
                String key = input[offset];

                // if the argument is a switch.
                if (key[0] == '-') {
                    // Trims any hyphens from the start of the argument. Allows for "-argument" and "--argument"
                    key = key.TrimStart('-');
                    
                    // Does another argument exist?
                    if (offset + 1 < arguments.Count && input[offset + 1][0] != '-') {
                        // No, the next string is not an argument switch. It's the value of the
                        // argument.
                        if (arguments.ContainsKey(key) == false) arguments.Add(key.ToLower(), input[offset + 1]);
                    }
                    else {
                        // Set to "true"
                        if (arguments.ContainsKey(key) == false) arguments.Add(key.ToLower(), "1");
                    }
                }
            }

            return arguments;
        }

        [STAThread, LoaderOptimization(LoaderOptimization.MultiDomainHost)]
        private static void Main(string[] args) {

            Console.WriteLine(Resources.ConsoleHeader);
            
            if (args.Length > 0) {
                // Support for --help command?

                System.Console.WriteLine(@"Starting with arguments: {0}", String.Join(" ", args));
            }

            ServiceController service = new ServiceController {
                Arguments = new List<String>(args),
                Settings = Program.Arguments(new List<String>(args))
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
                        Arguments = Program.Arguments(words.Skip(1).ToList())
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