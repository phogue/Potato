namespace Procon.Service.Shared {
    /// <summary>
    /// The proxy to be loaded in the service appdomain
    /// </summary>
    public interface IServiceLoaderProxy : IService {
        /// <summary>
        /// Creates the procon instance in the procon instance appdomain
        /// </summary>
        void Create();
    }
}
