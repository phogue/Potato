using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Procon.Core;
using Procon.Core.Connections.Plugins;

namespace Procon.Examples.Configs {
    public class Program : RemotePlugin {

        public Program() : base() {
            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        Name = "ThisIsJustACommand",
                        CommandAttributeType = CommandAttributeType.Executed,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "param1",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "param2",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.ThisIsJustACommand)
                }
            });
        }

        /// <summary>
        /// A command to accept from the config.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected CommandResultArgs ThisIsJustACommand(Command command, Dictionary<String, CommandParameter> parameters) {
            String param1 = parameters["param1"].First<String>();
            String param2 = parameters["param2"].First<String>();



            return command.Result;
        }

        public override void WriteConfig(Config config) {
            base.WriteConfig(config);

            // Writing configs may seem a little bit convoluted in Procon 2, but you should
            // think of it simply as Command serialization, which allows you to save complex
            // parameters.

            config.Root.Add(new Command() {
                Name = "ThisIsJustACommand",
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                "Parameter1Value"
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                "Parameter2Value"
                            }
                        }
                    }
                }
            }.ToConfigCommand());
        }

    }
}
