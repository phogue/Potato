using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Procon.Properties;
using Procon.Service.Shared;

namespace Procon {
    internal class Program {

        private static Dictionary<String, String> Arguments(IList<String> input) {
            Dictionary<String, String> arguments = new Dictionary<String, String>();

            if (input.Count() % 2 == 0) {
                IEnumerator<String> pair = input.GetEnumerator();

                while (pair.MoveNext() == true) {
                    String key = pair.Current.Trim('-', ' ').ToLower();
                    pair.MoveNext();
                    String value = pair.Current;

                    if (arguments.ContainsKey(key) == false) arguments.Add(key.ToLower(), value);
                    // Ignore it if it's already added.
                }
            }
            else {
                Console.WriteLine(@"Invalid argument input. Must be in key-value-pair syntax e.g ""--key value""");
                arguments = null;
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
                Arguments = new List<String>(args)
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
                    var words = new List<String>(input.Split(' '));
                    var command = words.FirstOrDefault();
                    var arguments = Program.Arguments(words.Skip(1).ToList());

                    if (command != null && arguments != null) {
                        service.SignalMessage(new ServiceMessage() {
                            Name = command,
                            Arguments = arguments
                        });
                    }
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