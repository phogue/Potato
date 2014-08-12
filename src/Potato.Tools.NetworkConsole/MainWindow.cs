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
using System.Windows.Forms;
using System.Reflection;
using System.Timers;
using Potato.Core.Connections;
using Potato.Core.Protocols;
using Potato.Core.Shared;
using Potato.Core.Variables;
using Potato.Net.Shared;
using Potato.Net.Shared.Actions;

namespace Potato.Tools.NetworkConsole {
    public partial class MainWindow : Form {
        /// <summary>
        ///  The actual game object loaded and operated on within a sandbox
        /// </summary>
        public ConnectionController Connection { get; set; }

        public VariableController Variables { get; set; }

        private ProtocolController ProtocolController { get; set; }

        // Console
        private LinkedList<String> CommandHistory { get; set; }
        private LinkedListNode<String> CommandHistoryCurrentNode { get; set; }

        private System.Timers.Timer Timer { get; set; }

        public MainWindow(IEnumerable<String> args) {
            InitializeComponent();

            this.CommandHistory = new LinkedList<string>();

            this.Variables = new VariableController();
            this.Variables.ParseArguments(new List<String>(args));

            this.ProtocolController = new ProtocolController();
            this.ProtocolController.Execute();

            this.Connection = new ConnectionController();
            this.Connection.Execute();
            this.Connection.ClientEvent += new Action<IClientEventArgs>(Connection_ClientEvent);

            this.Timer = new System.Timers.Timer(10000);
            this.Timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            this.Timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e) {
            this.Connection.Poke();
        }

        private void CreateProtocol() {
            ushort port = 10156;

            if (ushort.TryParse(this.txtPort.Text, out port) == true && String.IsNullOrEmpty(this.txtHostname.Text) == false) {
                var selectedProtocolMetadata = this.ProtocolController.Protocols.FirstOrDefault(protocolAssemblyMetadata => protocolAssemblyMetadata.ProtocolTypes.Select(protocolType => String.Format("{0} - {1} ({2})", protocolType.Provider, protocolType.Name, protocolType.Type)).Contains((string)this.cboGames.SelectedItem));

                if (selectedProtocolMetadata != null) {

                    var selectedProtocolType = selectedProtocolMetadata.ProtocolTypes.FirstOrDefault(protocolType => String.Format("{0} - {1} ({2})", protocolType.Provider, protocolType.Name, protocolType.Type) == (string)this.cboGames.SelectedItem);

                    if (selectedProtocolType != null) {

                        this.Connection.SetupProtocol(selectedProtocolMetadata, selectedProtocolType, new ProtocolSetup() {
                            Hostname = this.txtHostname.Text,
                            Password = this.txtPassword.Text,
                            Port = port
                        });

                        this.Connection.AttemptConnection();
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e) {
            this.lblVersion.Text = @"Version: " + Assembly.GetExecutingAssembly().GetName().Version;

            this.cboGames.Items.AddRange(this.ProtocolController.Protocols.SelectMany(protocolAssemblyMetadata => protocolAssemblyMetadata.ProtocolTypes).Select(protocolMetadataType => String.Format("{0} - {1} ({2})", protocolMetadataType.Provider, protocolMetadataType.Name, protocolMetadataType.Type)).Distinct().Cast<Object>().ToArray());

            this.txtHostname.Text = this.Variables.Get("Hostname", "");
            this.txtPort.Text = this.Variables.Get("Port", "");
            this.txtPassword.Text = this.Variables.Get("Password", "");
            this.txtAdditional.Text = this.Variables.Get("Additional", "");

            var protocolProvider = this.Variables.Get("ProtocolProvider", "");
            var protocolType = this.Variables.Get("ProtocolType", "");

            var result = this.ProtocolController.Tunnel(CommandBuilder.ProtocolsCheckSupportedProtocol(protocolProvider, protocolType).SetOrigin(CommandOrigin.Local));

            if (result.Success == true && result.Now.ProtocolAssemblyMetadatas.Count > 0 && result.Now.ProtocolTypes.Count > 0) {
                var type = result.Now.ProtocolTypes.First();

                var name = String.Format("{0} - {1} ({2})", type.Provider, type.Name, type.Type);

                this.cboGames.SelectedIndex = this.cboGames.Items.Contains(name) == true ? this.cboGames.Items.IndexOf(name) : 0;
            }
            else {
                this.cboGames.SelectedIndex = 0;
            }

            if (this.Variables.Get("Connect", false) == true) {
                this.CreateProtocol();
            }
        }

        private void btnConnect_Click(object sender, EventArgs e) {
            if (this.Connection.ProtocolState.Settings.Current.ConnectionState == ConnectionState.ConnectionDisconnected) {
                this.CreateProtocol();
            }
            else {
                this.Connection.Protocol.Shutdown();
            }
        }

        private void Connection_ClientEvent(IClientEventArgs e) {
            if (this.InvokeRequired == true) {
                this.Invoke(new Action<IClientEventArgs>(this.Connection_ClientEvent), e);
                return;
            }

            if (this.tbcPanels.SelectedIndex == 0 || this.chkVerboseLogging.Checked == true) {
                if (e.EventType == ClientEventType.ClientConnectionStateChange) {
                    this.ConsoleAppendLine("State: ^6{0}", e.ConnectionState.ToString());

                    if (e.ConnectionState == ConnectionState.ConnectionDisconnected) {
                        this.btnConnect.Text = @"Connect";
                        this.pnlConnection.Enabled = true;
                    }
                    else {
                        this.pnlConnection.Enabled = false;
                        this.btnConnect.Text = @"Disconnect";
                    }
                }
                else if (e.EventType == ClientEventType.ClientConnectionFailure || e.EventType == ClientEventType.ClientSocketException) {
                    this.ConsoleAppendLine("^1Error: {0}", e.Now.Exceptions.FirstOrDefault());
                }
                else if (e.EventType == ClientEventType.ClientPacketSent && e.Now.Packets.Count > 0) {
                    this.ConsoleAppendLine("^2SEND: {0}", e.Now.Packets.First().DebugText);
                }
                else if (e.EventType == ClientEventType.ClientPacketReceived && e.Now.Packets.Count > 0) {
                    this.ConsoleAppendLine("^5RECV: {0}", e.Now.Packets.First().DebugText);
                }
            }
        }

        public void ConsoleAppendLine(string format, params object[] parameters) {
            this.rtbConsole.AppendText(String.Format("[{0}] {1}{2}", DateTime.Now.ToShortTimeString(), String.Format(format, parameters), Environment.NewLine));

            this.rtbConsole.ReadOnly = false;
            int consoleBoxLines = this.rtbConsole.Lines.Length;

            if (this.chkAnchorScrollbar.Checked == true) {
                this.rtbConsole.ScrollToCaret();
            }

            if (consoleBoxLines > 100 && this.rtbConsole.Focused == false) {
                for (int i = 0; i < consoleBoxLines - 100; i++) {
                    this.rtbConsole.Select(0, this.rtbConsole.Lines[0].Length + 1);
                    this.rtbConsole.SelectedText = String.Empty;
                }
            }

            this.rtbConsole.ReadOnly = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            this.Connection.Dispose();
        }

        private void Execute(string commandText) {
            this.Connection.Protocol.Action(new NetworkAction() {
                ActionType = NetworkActionType.NetworkPacketSend,
                Now = {
                    Content = new List<String>() {
                        commandText
                    }
                }
            });
        }

        private void txtConsoleText_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyData == Keys.Enter) {
                this.btnSend_Click(null, null);

                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == Keys.Up) {
                e.SuppressKeyPress = true;

                if (this.CommandHistoryCurrentNode == null && this.CommandHistory.First != null) {
                    this.CommandHistoryCurrentNode = this.CommandHistory.First;
                    this.txtConsoleText.Text = this.CommandHistoryCurrentNode.Value;

                    this.txtConsoleText.Select(this.txtConsoleText.Text.Length, 0);
                }
                else if (this.CommandHistoryCurrentNode != null && this.CommandHistoryCurrentNode.Next != null) {
                    this.CommandHistoryCurrentNode = this.CommandHistoryCurrentNode.Next;
                    this.txtConsoleText.Text = this.CommandHistoryCurrentNode.Value;

                    this.txtConsoleText.Select(this.txtConsoleText.Text.Length, 0);
                }
            }
            else if (e.KeyData == Keys.Down) {
                if (this.CommandHistoryCurrentNode != null && this.CommandHistoryCurrentNode.Previous != null) {
                    this.CommandHistoryCurrentNode = this.CommandHistoryCurrentNode.Previous;
                    this.txtConsoleText.Text = this.CommandHistoryCurrentNode.Value;

                    this.txtConsoleText.Select(this.txtConsoleText.Text.Length, 0);
                }

                e.SuppressKeyPress = true;
            }
        }

        private void btnSend_Click(object sender, EventArgs e) {
            this.Execute(this.txtConsoleText.Text);

            this.CommandHistory.AddFirst(this.txtConsoleText.Text);
            if (this.CommandHistory.Count > 20) {
                this.CommandHistory.RemoveLast();
            }
            this.CommandHistoryCurrentNode = null;

            this.txtConsoleText.Clear();
            this.txtConsoleText.Focus();
        }

        private void txtHostname_TextChanged(object sender, EventArgs e) {
        }
    }
}