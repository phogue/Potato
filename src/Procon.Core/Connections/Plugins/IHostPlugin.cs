namespace Procon.Core.Connections.Plugins {
    public interface IHostPlugin {

        /// <summary>
        /// Execute a command on Procon's side of the appdomain (host)
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        CommandResultArgs ProxyExecute(Command command);
    }
}
