using System;
using System.Collections.Generic;

namespace Procon.Core.Shared {
    /// <summary>
    /// Description of how to dispatch a command where to dispatch it to
    /// </summary>
    public interface ICommandDispatch {
        /// <summary>
        /// The command being executed. This is the only value used to match up a command.
        /// </summary>
        String Name { get; set; }

        /// <summary>
        /// The command to be executed, will be converted to a string in Name
        /// </summary>
        CommandType CommandType { get; set; }

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
        Func<ICommand, Dictionary<String, ICommandParameter>, ICommandResult> Handler { get; set; }

        /// <summary>
        /// Checks if this dspatcher can handle the command
        /// </summary>
        /// <returns></returns>
        bool CanDispatch(CommandAttributeType attributeType, ICommand command);
    }
}
