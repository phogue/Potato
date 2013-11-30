using System;
using System.Collections.Generic;
using System.Globalization;
using Procon.Core;
using Procon.Core.Connections.Plugins;

namespace Procon.Examples.Commands {
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

        /// <summary>
        /// Store instances of child objects that inherit from ExecutableBase
        /// </summary>
        public List<IExecutableBase> ChildObjects;

        public Program() : base() {
            this.ChildObjects = new List<IExecutableBase>() {
                new TunneledCommands() {
                    ParentObjects = new List<IExecutableBase>() {
                        this
                    }
                }
            };

            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        Name = "SingleParameterCommand",
                        CommandAttributeType = CommandAttributeType.Executed,
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
                        CommandAttributeType = CommandAttributeType.Executed,
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
                        CommandAttributeType = CommandAttributeType.Executed
                    },
                    new CommandDispatchHandler(this.NoParameterCommand)
                }
            });
        }

        /// <summary>
        /// Now override this method, called within ExecutableBase, to specify what objects
        /// we should pass the command to. We're just passing it to all of the commands
        /// here, but you could check the command and only dispatch to certain objects if
        /// you want.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected override IList<IExecutableBase> TunnelExecutableObjects(Command command) {
            return this.ChildObjects;
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
