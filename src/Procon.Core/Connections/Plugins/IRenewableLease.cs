namespace Procon.Core.Connections.Plugins {
    public interface IRenewableLease {

        /// <summary>
        /// Renews the lease on any objects that have an expiration
        /// </summary>
        void RenewLease();
    }
}
