#region Copyright
// Copyright 2015 Geoff Green.
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
using System.Threading;

namespace Potato.Service.Shared.Test.TestServiceController.Mocks {
    [Serializable]
    public class MockSlowServiceLoaderProxy : IServiceLoaderProxy {
        public int StartSleep = 0;
        public int WriteConfigSleep = 0;
        public int DisposeSleep = 0;
        public int ParseCommandLineArgumentsSleep = 0;
        public int PollServiceSleep = 0;
        public int ExecuteMessageSleep = 0;
        public int CreateSleep = 0;

        public void Start() {
            if (StartSleep > 0) {
                Thread.Sleep(StartSleep);
            }
        }

        public void WriteConfig() {
            if (WriteConfigSleep > 0) {
                Thread.Sleep(WriteConfigSleep);
            }
        }

        public void Dispose() {
            if (DisposeSleep > 0) {
                Thread.Sleep(DisposeSleep);
            }
        }

        public void ParseCommandLineArguments(List<string> arguments) {
            if (ParseCommandLineArgumentsSleep > 0) {
                Thread.Sleep(ParseCommandLineArgumentsSleep);
            }
        }

        public ServiceMessage PollService() {
            if (PollServiceSleep > 0) {
                Thread.Sleep(PollServiceSleep);
            }

            return new ServiceMessage();
        }

        public ServiceMessage ExecuteMessage(ServiceMessage message) {
            if (PollServiceSleep > 0) {
                Thread.Sleep(ExecuteMessageSleep);
            }

            return new ServiceMessage();
        }

        public void Create() {
            if (CreateSleep > 0) {
                Thread.Sleep(CreateSleep);
            }
        }
    }
}
