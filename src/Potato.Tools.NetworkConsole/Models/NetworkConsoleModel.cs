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
        public LinkedList<string> CommandHistory { get; set; }

        public LinkedListNode<string> CommandHistoryCurrentNode { get; set; }

        /// <summary>
        /// Simple boolean for executing synchronization
        /// </summary>
        public bool SynchornizationEnabled { get; set; }

        /// <summary>
        /// Executes the synchronization 
        /// </summary>
        protected System.Timers.Timer SynchornizationTimer { get; set; }

        public NetworkConsoleModel() {
            CommandHistory = new LinkedList<string>();

            Variables = new VariableController();

            ProtocolController = new ProtocolController();
            ProtocolController.Execute();

            Connection = new ConnectionController();
            Connection.Execute();

            SynchornizationEnabled = true;

            SynchornizationTimer = new System.Timers.Timer(10000);
            SynchornizationTimer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            SynchornizationTimer.Start();
        }

        public void ParseArguments(IEnumerable<string> args) {
            Variables.ParseArguments(new List<string>(args));
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e) {
            if (SynchornizationEnabled == true) {
                Connection.Poke();
            }
        }

    }
}
