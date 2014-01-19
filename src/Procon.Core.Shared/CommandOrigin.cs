using System;

namespace Procon.Core.Shared {
    /// <summary>
    /// Where the command originated from (within Procon, over the command server or from a plugin)
    /// </summary>
    [Serializable]
    public enum CommandOrigin {
        /// <summary>
        /// Unknown location for where the command came from. This will always fail security checks.
        /// </summary>
        None,
        /// <summary>
        /// The command came from a plugin or was generated internally
        /// </summary>
        Local,
        /// <summary>
        /// The command came from a remote client
        /// </summary>
        Remote,
        /// <summary>
        /// The command came from a plugin
        /// </summary>
        Plugin
    }
}
