using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Procon.Core.Shared.Models;

namespace Procon.Core.Shared {
    /// <summary>
    /// A simple command to be passed between executable objects, allowing for commands
    /// to originate for various sources but allow for security, serialization and general neatness.
    /// </summary>
    [Serializable]
    public class Command : ICommand {
        public String Name { get; set; }

        [JsonIgnore]
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

        public Guid CommandGuid { get; set; }

        public CommandScopeModel ScopeModel { get; set; }

        public CommandOrigin Origin { get; set; }
        
        public ICommandResult Result { get; set; }

        [JsonIgnore]
        public ICommandRequest Request { get; set; }

        public List<ICommandParameter> Parameters { get; set; }

        public CommandAuthenticationModel Authentication { get; set; }

        /// <summary>
        /// Initializes a new command with the default values.
        /// </summary>
        public Command() {
            this.CommandGuid = Guid.NewGuid();

            this.Authentication = new CommandAuthenticationModel();

            this.ScopeModel = new CommandScopeModel();
        }

        public ICommand SetOrigin(CommandOrigin origin) {
            this.Origin = origin;

            return this;
        }

        public ICommand SetUsername(String username) {
            this.Authentication.Username = username;

            return this;
        }

        /// <summary>
        /// Allows for essentially cloning a command, but then allows inline overrides of the 
        /// attributes.
        /// </summary>
        /// <param name="command"></param>
        public Command(ICommand command) {
            this.CommandType = command.CommandType;
            this.Name = command.Name;
            this.Authentication = command.Authentication;
            this.Origin = command.Origin;
            this.ScopeModel = command.ScopeModel;
            this.Parameters = new List<ICommandParameter>(command.Parameters ?? new List<ICommandParameter>());
        }

        public ICommand ToConfigCommand() {
            return new Command(this) {
                ScopeModel = null
            };
        }

        public ICommand ParseCommandType(String commandName) {
            if (Enum.IsDefined(typeof(CommandType), commandName)) {
                this.CommandType = (CommandType)Enum.Parse(typeof(CommandType), commandName);
            }
            else {
                this.Name = commandName;
            }

            return this;
        }
    }
}