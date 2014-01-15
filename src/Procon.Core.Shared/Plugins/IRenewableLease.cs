namespace Procon.Core.Shared.Plugins {
    /// <summary>
    /// Implies this object has a lease that needs renewing at a set interval
    /// </summary>
    /// <remarks>If we decide to sign Procon.Core this may be redundant</remarks>
    public interface IRenewableLease {
        /// <summary>
        /// Renews the lease on any objects that have an expiration
        /// </summary>
        void RenewLease();
    }
}
