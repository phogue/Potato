using System;
using System.Collections.Generic;
using System.Globalization;
using Procon.Core.Shared;
using Procon.Core.Shared.Plugins;

namespace Procon.Examples.Plugins.Commands {
    /// <summary>
    /// A basic empty plugin that does absolutely nothing at all.
    /// </summary>
    /// <remarks>
    /// This is deliberately empty as this is the minimal code to run by Procon.
    /// </remarks>
    public class Program : PluginController {

        // Critical: You need to create a new project, not reuse this project.
        //           The critical part is the assembly GUID, which must be unique per plugin
        //           but then remain constant over your releases.
        //           Procon uses the GUID to pipe through events/commands.

        public Program() : base() {
            // Store references to children so commands will tunnel on to them.
            this.TunnelObjects = new List<ICoreController>() {
                new TunneledCommands() {
                    // Make sure child classes have a reference back to our class, so they
                    // can bubble commands back up.
                    BubbleObjects = new List<ICoreController>() {
                        this
                    }
                }
            };

            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        Name = "SingleParameterCommand",
                        CommandAttributeType = CommandAttributeType.Handler,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "text",
                                // Commands must have a type that appears within Procon.Core.CommandParameterData
                                // but "Content" works as a generic, so you can accept integers here for instance
                                // and if it can be converted it will be.
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.SingleParameterCommand)
                },
                {
                    new CommandAttribute() {
                        Name = "SingleConvertedParameterCommand",
                        CommandAttributeType = CommandAttributeType.Handler,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "number",
                                Type = typeof(int)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.SingleConvertedParameterCommand)
                },
                {
                    new CommandAttribute() {
                        Name = "NoParameterCommand",
                        CommandAttributeType = CommandAttributeType.Handler
                    },
                    new CommandDispatchHandler(this.NoParameterCommand)
                }
            });
        }

        /// <summary>
        /// Accept parameters in your command.
        /// </summary>
        /// <remarks>You'll have to name the parameter and specify the type of object you are expecting when appending your dispatch handler.</remarks>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected CommandResultArgs SingleParameterCommand(Command command, Dictionary<String, CommandParameter> parameters) {
            String text = parameters["text"].First<String>();

            command.Result.Message = text;

            return command.Result;
        }

        /// <summary>
        /// Accept a parameter that has been converted from a string to a integer
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected CommandResultArgs SingleConvertedParameterCommand(Command command, Dictionary<String, CommandParameter> parameters) {
            int number = parameters["number"].First<int>();

            command.Result.Message = (number * 2).ToString(CultureInfo.InvariantCulture);

            return command.Result;
        }

        /// <summary>
        /// Easy enough to accept commands with zero parameters..
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected CommandResultArgs NoParameterCommand(Command command, Dictionary<String, CommandParameter> parameters) {
            command.Result.Message = "NoParameterCommandSetResult";

            return command.Result;
        }
    }
}
