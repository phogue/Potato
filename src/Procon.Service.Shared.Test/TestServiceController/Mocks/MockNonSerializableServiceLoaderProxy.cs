#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
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

        public ServiceMessage ExecuteMessage(ServiceMessage message) {
            throw new NotImplementedException();
        }

        public void Create() {
            throw new NotImplementedException();
        }
    }
}
