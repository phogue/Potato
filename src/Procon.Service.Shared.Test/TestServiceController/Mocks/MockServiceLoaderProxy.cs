using System;
using System.Collections.Generic;

namespace Procon.Service.Shared.Test.TestServiceController.Mocks {
    [Serializable]
    public class MockServiceLoaderProxy : IServiceLoaderProxy {

        public ServiceMessage WaitingMessage { get; set; }

        public Action OnDisposeHandler { get; set; }

        public bool OnStart { get; set; }
        public bool OnWriteConfig { get; set; }
        public bool OnDispose { get; set; }
        public List<String> OnParseCommandLineArguments { get; set; }
        public bool OnPollService { get; set; }
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

        public void Create() {
            this.OnCreate = true;
        }
    }
}
