using System.Security.Cryptography.X509Certificates;
using Procon.Core.Shared;

namespace Procon.Core.Remote {
    /// <summary>
    /// Manages loading and holding the X509Certificate object
    /// </summary>
    public interface ICertificateController : ISharedReferenceAccess {
        /// <summary>
        /// The loaded X509Certificate object
        /// </summary>
        X509Certificate2 Certificate { get; }

        /// <summary>
        /// Executes the controller.
        /// </summary>
        ICoreController Execute();
    }
}
