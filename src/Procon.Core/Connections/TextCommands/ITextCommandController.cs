using System;
using Procon.Core.Shared;

namespace Procon.Core.Connections.TextCommands {
    /// <summary>
    /// Manages registering, dispatching text commands
    /// </summary>
    public interface ITextCommandController : ICoreController {
        /// <summary>
        /// Checks if a prefix is an allowed prefix
        /// </summary>
        /// <param name="prefix">The prefix to check (e.g !, @ etc.)</param>
        /// <returns>The parameter prefix, or null if the prefix is invalid</returns>
        String GetValidTextCommandPrefix(String prefix);
    }
}
