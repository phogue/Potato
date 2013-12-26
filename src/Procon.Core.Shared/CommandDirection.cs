namespace Procon.Core.Shared {
    /// <summary>
    /// The direction a command should propogate in
    /// </summary>
    public enum CommandDirection {
        /// <summary>
        /// The command should go down the tree of children
        /// </summary>
        Tunnel,
        /// <summary>
        /// The command should go up through the ancestors
        /// </summary>
        Bubble
    }
}
