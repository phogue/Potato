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

namespace Procon.Service.Shared.Test.TestServiceController.Mocks {
    [Serializable]
    public class MockServiceLoaderProxy : IServiceLoaderProxy {

        public ServiceMessage WaitingMessage { get; set; }

        public ServiceMessage ExecuteResultMessage { get; set; }

        public Action OnDisposeHandler { get; set; }

        public bool OnStart { get; set; }
        public bool OnWriteConfig { get; set; }
        public bool OnDispose { get; set; }
        public List<String> OnParseCommandLineArguments { get; set; }
        public bool OnPollService { get; set; }
        public bool OnExecuteMessage { get; set; }
        public bool OnCreate { get; set; }

        public void Start() {
            this.OnStart = true;
        }

        public void WriteConfig() {
            this.OnWriteConfig = true;
        }

        public void Dispose() {
            this.OnDispose = true;

            if (this.OnDisposeHandler != null) this.OnDisposeHandler();
        }

        public void ParseCommandLineArguments(List<String> arguments) {
            this.OnParseCommandLineArguments = arguments;
        }

        public ServiceMessage PollService() {
            this.OnPollService = true;
            return this.WaitingMessage;
        }

        public ServiceMessage ExecuteMessage(ServiceMessage message) {
            this.OnExecuteMessage = true;
            return this.ExecuteResultMessage;
        }

        public void Create() {
            this.OnCreate = true;
        }
    }
}
