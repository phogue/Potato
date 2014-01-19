using System;
using System.Collections.Generic;
using Procon.Core.Shared;
using Procon.Core.Shared.Plugins;

namespace Procon.Examples.Plugins.Configs {
    public class Program : PluginController {

        // Critical: You need to create a new project, not reuse this project.
        //           The critical part is the assembly GUID, which must be unique per plugin
        //           but then remain constant over your releases.
        //           Procon uses the GUID to pipe through events/commands.

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
        protected ICommandResult ThisIsJustACommand(Command command, Dictionary<String, CommandParameter> parameters) {
            String param1 = parameters["param1"].First<String>();
            String param2 = parameters["param2"].First<String>();

            return command.Result;
        }

        public override void WriteConfig(IConfig config) {
            base.WriteConfig(config);

            // Writing configs may seem a little bit convoluted in Procon 2, but you should
            // think of it simply as Command serialization, which allows you to save complex
            // parameters.

            config.Append(new Command() {
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
