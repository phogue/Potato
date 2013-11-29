using System;
using System.Collections.Generic;
using Procon.Core;
using Procon.Core.Connections.Plugins;

namespace Procon.Examples.CommandRouting {
    /// <summary>
    /// A basic empty plugin that does absolutely nothing at all.
    /// </summary>
    /// <remarks>
    /// This is deliberately empty as this is the minimal code to run by Procon.
    /// </remarks>
    public class Program : RemotePlugin {

        // Critical: You need to create a new project, not reuse this project.
        //           The critical part is the assembly GUID, which must be unique per plugin
        //           but then remain constant over your releases.
        //           Procon uses the GUID to pipe through events/commands.

        public Program() : base() {
            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        Name = "SingleParameterCommand",
                        CommandAttributeType = CommandAttributeType.Executed,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "text",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.SingleParameterCommand)
                },
                {
                    new CommandAttribute() {
                        Name = "NoParameterCommand",
                        CommandAttributeType = CommandAttributeType.Executed
                    },
                    new CommandDispatchHandler(this.NoParameterCommand)
                }
            });
        }

        protected CommandResultArgs SingleParameterCommand(Command command, Dictionary<String, CommandParameter> parameters) {
            String text = parameters["text"].First<String>();

            command.Result.Message = text;

            return command.Result;
        }

        protected CommandResultArgs NoParameterCommand(Command command, Dictionary<String, CommandParameter> parameters) {
            command.Result.Message = "NoParameterCommand";

            return command.Result;
        }
    }
}
