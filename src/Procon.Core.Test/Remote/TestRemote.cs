using System.Net;

namespace Procon.Core.Test.Remote {
    public class TestRemote {
        protected void SetUp() {

            // We could actually validate the certificate in /Certificates directory
            // but for unit testing I find this acceptable enough.

            // If you're reading this, never put this into production (anywhere.)
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }
    }
}