namespace Procon.Core.Connections.Plugins {

    /// <summary>
    /// Interface to go from remote -> host.
    /// </summary>
    public interface IPluginCallback {

        /// <summary>
        /// Execute a command on Procon's side of the appdomain (host)
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        CommandResultArgs ProxyExecute(Command command);
    }
}
