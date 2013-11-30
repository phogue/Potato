using System;
using System.Collections.Generic;
using Procon.Core;

namespace Procon.Examples.CommandRouting {
    /// <summary>
    /// Note we need to inherit from ExecutableBase which has all the methods
    /// required to dispatch commands
    /// </summary>
    public class TunneledCommands : ExecutableBase {

        /// <summary>
        /// Store instances of child objects that inherit from ExecutableBase
        /// </summary>
        public List<IExecutableBase> ParentObjects;

        public TunneledCommands() : base() {
            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        Name = "ThisCommandIsInAChildObject",
                        CommandAttributeType = CommandAttributeType.Executed
                    },
                    new CommandDispatchHandler(this.ThisCommandIsInAChildObject)
                },
                {
                    new CommandAttribute() {
                        Name = "NoParameterBubbleCommand",
                        CommandAttributeType = CommandAttributeType.Executed,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "number",
                                Type = typeof(int)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.NoParameterBubbleCommand)
                }
            });
        }

        protected override IList<IExecutableBase> BubbleExecutableObjects(Command command) {
            return this.ParentObjects;
        }

        protected CommandResultArgs ThisCommandIsInAChildObject(Command command, Dictionary<String, CommandParameter> parameters) {
            command.Result.Message = "ThisCommandIsInAChildObjectResult";

            return command.Result;
        }

        protected CommandResultArgs NoParameterBubbleCommand(Command command, Dictionary<String, CommandParameter> parameters) {
            command.Name = "SingleConvertedParameterCommand";

            // Bubble the command back up to Program.cs
            return this.Bubble(command);
        }
    }
}
