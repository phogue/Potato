using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Procon.Net.Shared.Protocols;
using Procon.Net.Shared.Protocols.CommandServer;
using Procon.Net.Shared.Utils;

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
        public CommandScope Scope { get; set; }

        /// <summary>
        /// Where the command came from
        /// </summary>
        public CommandOrigin Origin { get; set; }

        /// <summary>
        /// The final result of this command.
        /// </summary>
        public CommandResultArgs Result { get; set; }

        /// <summary>
        /// The original request from a remote source.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public CommandServerPacket RemoteRequest { get; set; }

        /// <summary>
        /// The raw parameters to be passed into the executable command.
        /// </summary>
        public List<CommandParameter> Parameters { get; set; } 

        #region Executing User - Should we move this to it's own object?

        /// <summary>
        /// The username of the initiator
        /// </summary>
        public String Username { get; set; }

        /// <summary>
        /// The password of the user executing the command. Used to authenticate
        /// remote requests.
        /// </summary>
        /// <remarks>Will change to much more secure password authentication</remarks>
        public String PasswordPlainText { get; set; }

        /// <summary>
        /// The game type of the initiators player Uid
        /// </summary>
        public String GameType { get; set; }

        /// <summary>
        /// The uid of the player initiating the command
        /// </summary>
        public String Uid { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new command with the default values.
        /// </summary>
        public Command() {
            this.CommandGuid = Guid.NewGuid();
            this.Username = null;
            this.GameType = CommonGameType.None;
            this.Uid = null;

            this.Scope = new CommandScope();
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
            this.Username = username;

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
            this.Username = command.Username;
            this.GameType = command.GameType;
            this.Uid = command.Uid;
            this.Origin = command.Origin;
            this.PasswordPlainText = command.PasswordPlainText;
            this.Scope = command.Scope;
        }

        /// <summary>
        /// The config only requires the name and parameters, everything else is ignored. We could just
        /// return the results of ToXElement() but we neaten it up a little bit just so the config
        /// isn't bloated with useless information.
        /// </summary>
        /// <returns></returns>
        public XElement ToConfigCommand() {
            XElement result = this.ToXElement();

            if (result != null) {
                XElement scope = result.Element("Scope");
                XElement origin = result.Element("Origin");
                XElement gameType = result.Element("GameType");

                // Remove the scope attribute if it has no effect
                if (scope != null && this.Scope.ConnectionGuid == Guid.Empty && this.Scope.PluginGuid == Guid.Empty) scope.Remove();
                if (origin != null) origin.Remove();
                if (gameType != null) gameType.Remove();
            }
            
            return result;
        }
    }
}