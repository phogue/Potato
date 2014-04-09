using System;

namespace Procon.Net.Shared.Sandbox {
    /// <summary>
    /// Remoting interface for Procon.Core to communicate with remote Plugin.
    /// </summary>
    public interface ISandboxProtocolController : IProtocolShared {
        /// <summary>
        /// Object to bubble events to
        /// </summary>
        SandboxProtocolCallback Bubble { get; set; }

        /// <summary>
        /// Loads a protocol assembly, loads a new IProtocol instance with the setup provided.
        /// </summary>
        /// <param name="assemblyFile"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        bool Create(String assemblyFile, IProtocolType type);
    }
}
