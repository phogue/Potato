using System;
using System.Collections.Generic;
using System.Threading;

namespace Procon.Service.Shared.Test.TestServiceController.Mocks {
    [Serializable]
    public class MockSlowServiceLoaderProxy : IServiceLoaderProxy {
        public int StartSleep = 0;
        public int WriteConfigSleep = 0;
        public int DisposeSleep = 0;
        public int ParseCommandLineArgumentsSleep = 0;
        public int PollServiceSleep = 0;
        public int CreateSleep = 0;

        public void Start() {
            if (this.StartSleep > 0) {
                Thread.Sleep(StartSleep);
            }
        }

        public void WriteConfig() {
            if (this.WriteConfigSleep > 0) {
                Thread.Sleep(WriteConfigSleep);
            }
        }

        public void Dispose() {
            if (this.DisposeSleep > 0) {
                Thread.Sleep(DisposeSleep);
            }
        }

        public void ParseCommandLineArguments(List<String> arguments) {
            if (this.ParseCommandLineArgumentsSleep > 0) {
                Thread.Sleep(ParseCommandLineArgumentsSleep);
            }
        }

        public ServiceMessage PollService() {
            if (this.PollServiceSleep > 0) {
                Thread.Sleep(PollServiceSleep);
            }

            return new ServiceMessage();
        }

        public void Create() {
            if (this.CreateSleep > 0) {
                Thread.Sleep(CreateSleep);
            }
        }
    }
}
