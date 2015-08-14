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

namespace Potato.Service.Shared.Test.TestServiceController.Mocks {
    [Serializable]
    public class MockServiceLoaderProxy : IServiceLoaderProxy {

        public ServiceMessage WaitingMessage { get; set; }

        public ServiceMessage ExecuteResultMessage { get; set; }

        public Action OnDisposeHandler { get; set; }

        public bool OnStart { get; set; }
        public bool OnWriteConfig { get; set; }
        public bool OnDispose { get; set; }
        public List<string> OnParseCommandLineArguments { get; set; }
        public bool OnPollService { get; set; }
        public bool OnExecuteMessage { get; set; }
        public bool OnCreate { get; set; }

        public void Start() {
            OnStart = true;
        }

        public void WriteConfig() {
            OnWriteConfig = true;
        }

        public void Dispose() {
            OnDispose = true;

            if (OnDisposeHandler != null) OnDisposeHandler();
        }

        public void ParseCommandLineArguments(List<string> arguments) {
            OnParseCommandLineArguments = arguments;
        }

        public ServiceMessage PollService() {
            OnPollService = true;
            return WaitingMessage;
        }

        public ServiceMessage ExecuteMessage(ServiceMessage message) {
            OnExecuteMessage = true;
            return ExecuteResultMessage;
        }

        public void Create() {
            OnCreate = true;
        }
    }
}
