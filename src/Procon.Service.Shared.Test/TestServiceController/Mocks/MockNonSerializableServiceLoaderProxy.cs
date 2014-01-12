using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Service.Shared.Test.TestServiceController.Mocks {
    /// <summary>
    /// This class is designed to fail anywhere it can to raise an exception (any)
    /// </summary>
    public class MockNonSerializableServiceLoaderProxy : IServiceLoaderProxy {
        public void Start() {
            throw new NotImplementedException();
        }

        public void WriteConfig() {
            throw new NotImplementedException();
        }

        public void Dispose() {
            throw new NotImplementedException();
        }

        public void ParseCommandLineArguments(List<string> arguments) {
            throw new NotImplementedException();
        }

        public ServiceMessage PollService() {
            throw new NotImplementedException();
        }

        public void Create() {
            throw new NotImplementedException();
        }
    }
}
