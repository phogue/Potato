using System;
using System.Collections.Generic;
using System.ServiceProcess;
using Procon.Service.Shared;
using ServiceController = Procon.Service.Shared.ServiceController;

namespace Procon.Service {
    public partial class ProconService : ServiceBase {

        /// <summary>
        /// The currently loaded instance of Procon.
        /// </summary>
        public ServiceController Instance { get; set; }

        public ProconService() {
            InitializeComponent();
        }

        protected override void OnStart(string[] args) {
            this.Instance = new ServiceController() {
                Arguments = new List<String>(args),
                Settings = new ServiceSettings(new List<String>(args))
            };

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
