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
using System.Text.RegularExpressions;

namespace Potato.Core.Shared {
    /// <summary>
    /// Description of how to dispatch a command where to dispatch it to
    /// </summary>
    public interface ICommandDispatch {
        /// <summary>
        /// The command being executed. This is the only value used to match up a command.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The command to be executed, will be converted to a string in Name
        /// </summary>
        CommandType CommandType { get; set; }

        /// <summary>
        /// The pattern to use for command handling. If set then a command name will be tested against this name
        /// </summary>
        Regex NamePattern { get; set; }

        /// <summary>
        /// When in the execution we want to capture the command (before, as the handler or after)
        /// </summary>
        CommandAttributeType CommandAttributeType { get; set; }

        /// <summary>
        /// A list of parameter names with the type of parameter expected.
        /// </summary>
        List<CommandParameterType> ParameterTypes { get; set; }

        /// <summary>
        /// The handler to dispatch the command to
        /// </summary>
        Func<ICommand, Dictionary<string, ICommandParameter>, ICommandResult> Handler { get; set; }

        /// <summary>
        /// Checks if this dspatcher can handle the command
        /// </summary>
        /// <returns></returns>
        bool CanDispatch(CommandAttributeType attributeType, ICommand command);

        /// <summary>
        /// Compares an expected parameter list against the parameters supplied. If the types match (or can be converted) then
        /// a dictionary of parameter names to the parameters supplied is returned, otherwise null is returned implying
        /// and error was encountered or a type wasn't found.
        /// </summary>
        Dictionary<string, ICommandParameter> BuildParameterDictionary(IList<ICommandParameter> parameters);
    }
}
