using System.Collections.Generic;
using System.ServiceProcess;
using Procon.Service.Shared;

namespace Procon.Service {
    public partial class ProconService : ServiceBase {

        /// <summary>
        /// The currently loaded instance of Procon.
        /// </summary>
        public Shared.ServiceController Instance { get; set; }

        public ProconService() {
            InitializeComponent();
        }

        protected override void OnStart(string[] args) {
            this.Instance.Arguments = new List<string>(args);

            this.Instance.SignalMessage(new ServiceMessage() {
                Name = "start"
            });
        }

        protected override void OnStop() {
            this.Instance.SignalMessage(new ServiceMessage() {
                Name = "stop"
            });
        }

        protected override void OnShutdown() {
            this.Instance.SignalMessage(new ServiceMessage() {
                Name = "stop"
            });
        }
    }
}
