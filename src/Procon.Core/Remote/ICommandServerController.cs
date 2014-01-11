using Procon.Core.Shared;

namespace Procon.Core.Remote {
    /// <summary>
    /// Listens for incoming connections, authenticates and dispatches commands
    /// </summary>
    public interface ICommandServerController : ICoreController {
        /// <summary>
        /// Pokes the underlying listener, ensuring that all clients held in memory are still
        /// active and not disconnected.
        /// </summary>
        void Poke();

        /// <summary>
        /// Executes the controller.
        /// </summary>
        ICoreController Execute();

        /// <summary>
        /// Writes the config for this controller
        /// </summary>
        /// <param name="config">The config to append data to</param>
        void WriteConfig(Config config);

        /// <summary>
        /// Disposes the object.
        /// </summary>
        void Dispose();
    }
}
