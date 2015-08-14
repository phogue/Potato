#region Copyright
// Copyright 2015 Geoff Green.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using Potato.Core.Shared.Models;

namespace Potato.Core.Shared {
    /// <summary>
    /// A simple command to be passed between executable objects, allowing for commands
    /// to originate for various sources but allow for security, serialization and general neatness.
    /// </summary>
    public interface ICommand {
        /// <summary>
        /// The command being executed. This is the only value used to match up a command.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The command to be executed, will be converted to a string in Name
        /// </summary>
        CommandType CommandType { get; set; }

        /// <summary>
        /// The commands unique identifier, created when the command object is created.
        /// </summary>
        Guid CommandGuid { get; set; }

        /// <summary>
        /// The scope that this commands execution should be limited to.
        /// </summary>
        CommandScopeModel Scope { get; set; }

        /// <summary>
        /// Where the command came from
        /// </summary>
        CommandOrigin Origin { get; set; }

        /// <summary>
        /// The final result of this command.
        /// </summary>
        ICommandResult Result { get; set; }

        /// <summary>
        /// The original request from a remote source.
        /// </summary>
        ICommandRequest Request { get; set; }

        /// <summary>
        /// The raw parameters to be passed into the executable command.
        /// </summary>
        List<ICommandParameter> Parameters { get; set; }

        /// <summary>
        /// Holds the authentication information required to execute the command.
        /// </summary>
        CommandAuthenticationModel Authentication { get; set; }

        /// <summary>
        /// Sets the origin of the command, then returns the command. Allows for method chaining
        /// </summary>
        /// <param name="origin">The origin to set this command</param>
        /// <returns>this</returns>
        ICommand SetOrigin(CommandOrigin origin);

        /// <summary>
        /// Sets the authentication of the command, then returns the command. Allows for method chaining.
        /// </summary>
        /// <param name="authentication">The authentication model to set</param>
        /// <returns>this</returns>
        ICommand SetAuthentication(CommandAuthenticationModel authentication);

        /// <summary>
        /// Sets the scope of the command, then returns the command. Allows for method chaining
        /// </summary>
        /// <param name="scope">The scope to set this command</param>
        /// <returns>this</returns>
        ICommand SetScope(CommandScopeModel scope);

        /// <summary>
        /// The config only requires the name and parameters, everything else is ignored. We could just
        /// return the results of ToXElement() but we neaten it up a little bit just so the config
        /// isn't bloated with useless information.
        /// </summary>
        /// <returns></returns>
        IConfigCommand ToConfigCommand();

        /// <summary>
        /// Parses a command type from an enum if it is valid.
        /// </summary>
        /// <param name="commandName"></param>
        ICommand ParseCommandType(string commandName);
    }
}
