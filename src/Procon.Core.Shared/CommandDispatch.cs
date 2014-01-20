using System;
using System.Collections.Generic;

namespace Procon.Core.Shared {
    /// <summary>
    /// Command to be executed
    /// </summary>
    /// <remarks><para>Called to execute a command.</para></remarks>
    [Serializable]
    public class CommandDispatch : IDisposable {
        /// <summary>
        /// The command being executed. This is the only value used to match up a command.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The command to be executed, will be converted to a string in Name
        /// </summary>
        public CommandType CommandType {
            get { return this._mCommandType; }
            set {
                this._mCommandType = value;

                if (this._mCommandType != CommandType.None) {
                    this.Name = value.ToString();
                }
            }
        }
        private CommandType _mCommandType;

        /// <summary>
        /// When in the execution we want to capture the command (before, as the handler or after)
        /// </summary>
        public CommandAttributeType CommandAttributeType { get; set; }

        /// <summary>
        /// A list of parameter names with the type of parameter expected.
        /// </summary>
        public List<CommandParameterType> ParameterTypes { get; set; }

        /// <summary>
        /// The handler to dispatch the command to
        /// </summary>
        public Func<ICommand, Dictionary<String, ICommandParameter>, ICommandResult> Handler { get; set; }

        /// <summary>
        /// Initializes the dispatch with default values.
        /// </summary>
        public CommandDispatch() {
            this.CommandAttributeType = CommandAttributeType.Handler;
        }

        /// <summary>
        /// Parses a command type from an enum if it is valid.
        /// </summary>
        /// <param name="commandName"></param>
        public CommandDispatch ParseCommandType(String commandName) {
            if (Enum.IsDefined(typeof(CommandType), commandName)) {
                this.CommandType = (CommandType)Enum.Parse(typeof(CommandType), commandName);
            }
            else {
                this.Name = commandName;
            }

            return this;
        }
        
        /// <summary>
        /// Checks if this dspatcher can handle the command
        /// </summary>
        /// <returns></returns>
        public bool CanDispatch(CommandAttributeType attributeType, ICommand command) {
            return this.CommandAttributeType == attributeType && this.Name == command.Name;
        }

        public void Dispose() {
            this.CommandType = CommandType.None;
            this.Name = null;
        }
    }
}
