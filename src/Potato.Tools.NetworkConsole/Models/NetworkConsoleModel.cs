using System;
using System.Collections.Generic;
using System.Timers;
using Potato.Core.Connections;
using Potato.Core.Protocols;
using Potato.Core.Variables;

namespace Potato.Tools.NetworkConsole.Models {
    public class NetworkConsoleModel {
        /// <summary>
        ///  The actual game object loaded and operated on within a sandbox
        /// </summary>
        public ConnectionController Connection { get; set; }

        /// <summary>
        /// Controller to pass command line arguments
        /// </summary>
        public VariableController Variables { get; set; }

        /// <summary>
        /// The controller to load up all available protocols
        /// </summary>
        public ProtocolController ProtocolController { get; set; }

        /// <summary>
        /// The history of commands sent to the server
        /// </summary>
        public LinkedList<String> CommandHistory { get; set; }

        public LinkedListNode<String> CommandHistoryCurrentNode { get; set; }

        /// <summary>
        /// Simple boolean for executing synchronization
        /// </summary>
        public bool SynchornizationEnabled { get; set; }

        /// <summary>
        /// Executes the synchronization 
        /// </summary>
        protected System.Timers.Timer SynchornizationTimer { get; set; }

        public NetworkConsoleModel() {
            this.CommandHistory = new LinkedList<string>();

            this.Variables = new VariableController();

            this.ProtocolController = new ProtocolController();
            this.ProtocolController.Execute();

            this.Connection = new ConnectionController();
            this.Connection.Execute();

            this.SynchornizationEnabled = true;

            this.SynchornizationTimer = new System.Timers.Timer(10000);
            this.SynchornizationTimer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            this.SynchornizationTimer.Start();
        }

        public void ParseArguments(IEnumerable<String> args) {
            this.Variables.ParseArguments(new List<String>(args));
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e) {
            if (this.SynchornizationEnabled == true) {
                this.Connection.Poke();
            }
        }

    }
}
