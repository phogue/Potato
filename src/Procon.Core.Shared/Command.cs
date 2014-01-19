using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Procon.Core.Shared.Models;
using Procon.Net.Shared.Protocols.CommandServer;

namespace Procon.Core.Shared {
    /// <summary>
    /// A simple command to be passed between executable objects, allowing for commands
    /// to originate for various sources but allow for security, serialization and general neatness.
    /// </summary>
    [Serializable]
    public class Command : CommandAttribute {

        /// <summary>
        /// The commands unique identifier, created when the command object is created.
        /// </summary>
        public Guid CommandGuid { get; set; }

        /// <summary>
        /// The scope that this commands execution should be limited to.
        /// </summary>
        public CommandScopeModel ScopeModel { get; set; }

        /// <summary>
        /// Where the command came from
        /// </summary>
        public CommandOrigin Origin { get; set; }

        /// <summary>
        /// The final result of this command.
        /// </summary>
        public CommandResult Result { get; set; }

        /// <summary>
        /// The original request from a remote source.
        /// </summary>
        [JsonIgnore]
        public CommandServerPacket RemoteRequest { get; set; }

        /// <summary>
        /// The raw parameters to be passed into the executable command.
        /// </summary>
        public List<CommandParameter> Parameters { get; set; }

        /// <summary>
        /// Holds the authentication information required to execute the command.
        /// </summary>
        public CommandAuthenticationModel Authentication { get; set; }

        /// <summary>
        /// Initializes a new command with the default values.
        /// </summary>
        public Command() {
            this.CommandGuid = Guid.NewGuid();

            this.Authentication = new CommandAuthenticationModel();

            this.ScopeModel = new CommandScopeModel();
        }

        /// <summary>
        /// Sets the origin of the command, then returns the command. Allows for method chaining
        /// </summary>
        /// <param name="origin">The origin to set this command</param>
        /// <returns>this</returns>
        public Command SetOrigin(CommandOrigin origin) {
            this.Origin = origin;

            return this;
        }

        /// <summary>
        /// Sets the username of the command, then returns the command. Allows for method chaining.
        /// </summary>
        /// <param name="username">The username to assign</param>
        /// <returns>this</returns>
        public Command SetUsername(String username) {
            this.Authentication.Username = username;

            return this;
        }

        /// <summary>
        /// Allows for essentially cloning a command, but then allows inline overrides of the 
        /// attributes.
        /// </summary>
        /// <param name="command"></param>
        public Command(Command command) {
            this.CommandType = command.CommandType;
            this.Name = command.Name;
            this.Authentication = command.Authentication;
            this.Origin = command.Origin;
            this.ScopeModel = command.ScopeModel;
            this.Parameters = new List<CommandParameter>(command.Parameters ?? new List<CommandParameter>());
        }

        /// <summary>
        /// The config only requires the name and parameters, everything else is ignored. We could just
        /// return the results of ToXElement() but we neaten it up a little bit just so the config
        /// isn't bloated with useless information.
        /// </summary>
        /// <returns></returns>
        public Command ToConfigCommand() {
            return new Command(this) {
                ScopeModel = null
            };
        }
    }
}