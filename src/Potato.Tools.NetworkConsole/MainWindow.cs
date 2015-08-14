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
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using Potato.Core.Shared;
using Potato.Net.Shared;
using Potato.Net.Shared.Actions;
using Potato.Tools.NetworkConsole.Models;

namespace Potato.Tools.NetworkConsole {
    public partial class MainWindow : Form {

        public NetworkConsoleModel NetworkConsoleModel { get; set; }

        public MainWindow(IEnumerable<string> args) {
            InitializeComponent();

            NetworkConsoleModel = new NetworkConsoleModel();
            NetworkConsoleModel.ParseArguments(args);

            NetworkConsoleModel.Connection.ClientEvent += new Action<IClientEventArgs>(Connection_ClientEvent);

            protocolTestControl1.NetworkConsoleModel = NetworkConsoleModel;
        }

        private void CreateProtocol() {
            ushort port = 10156;

            if (ushort.TryParse(txtPort.Text, out port) == true && string.IsNullOrEmpty(txtHostname.Text) == false) {
                var selectedProtocolMetadata = NetworkConsoleModel.ProtocolController.Protocols.FirstOrDefault(protocolAssemblyMetadata => protocolAssemblyMetadata.ProtocolTypes.Select(protocolType => string.Format("{0} - {1} ({2})", protocolType.Provider, protocolType.Name, protocolType.Type)).Contains((string)cboGames.SelectedItem));

                if (selectedProtocolMetadata != null) {

                    var selectedProtocolType = selectedProtocolMetadata.ProtocolTypes.FirstOrDefault(protocolType => string.Format("{0} - {1} ({2})", protocolType.Provider, protocolType.Name, protocolType.Type) == (string)cboGames.SelectedItem);

                    if (selectedProtocolType != null) {

                        NetworkConsoleModel.Connection.SetupProtocol(selectedProtocolMetadata, selectedProtocolType, new ProtocolSetup() {
                            Hostname = txtHostname.Text,
                            Password = txtPassword.Text,
                            Port = port
                        });

                        NetworkConsoleModel.Connection.AttemptConnection();
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e) {
            lblVersion.Text = @"Version: " + Assembly.GetExecutingAssembly().GetName().Version;

            cboGames.Items.AddRange(NetworkConsoleModel.ProtocolController.Protocols.SelectMany(protocolAssemblyMetadata => protocolAssemblyMetadata.ProtocolTypes).Select(protocolMetadataType => string.Format("{0} - {1} ({2})", protocolMetadataType.Provider, protocolMetadataType.Name, protocolMetadataType.Type)).Distinct().Cast<object>().ToArray());

            txtHostname.Text = NetworkConsoleModel.Variables.Get("Hostname", "");
            txtPort.Text = NetworkConsoleModel.Variables.Get("Port", "");
            txtPassword.Text = NetworkConsoleModel.Variables.Get("Password", "");
            txtAdditional.Text = NetworkConsoleModel.Variables.Get("Additional", "");

            var protocolProvider = NetworkConsoleModel.Variables.Get("ProtocolProvider", "");
            var protocolType = NetworkConsoleModel.Variables.Get("ProtocolType", "");

            var result = NetworkConsoleModel.ProtocolController.Tunnel(CommandBuilder.ProtocolsCheckSupportedProtocol(protocolProvider, protocolType).SetOrigin(CommandOrigin.Local));

            if (result.Success == true && result.Now.ProtocolAssemblyMetadatas.Count > 0 && result.Now.ProtocolTypes.Count > 0) {
                var type = result.Now.ProtocolTypes.First();

                var name = string.Format("{0} - {1} ({2})", type.Provider, type.Name, type.Type);

                cboGames.SelectedIndex = cboGames.Items.Contains(name) == true ? cboGames.Items.IndexOf(name) : 0;
            }
            else {
                cboGames.SelectedIndex = 0;
            }

            if (NetworkConsoleModel.Variables.Get("Connect", false) == true) {
                CreateProtocol();
            }
        }

        private void btnConnect_Click(object sender, EventArgs e) {
            if (NetworkConsoleModel.Connection.ProtocolState.Settings.Current.ConnectionState == ConnectionState.ConnectionDisconnected) {
                CreateProtocol();
            }
            else {
                NetworkConsoleModel.Connection.Protocol.Shutdown();
            }
        }

        private void Connection_ClientEvent(IClientEventArgs e) {
            if (InvokeRequired == true) {
                Invoke(new Action<IClientEventArgs>(Connection_ClientEvent), e);
                return;
            }

            if (tbcPanels.SelectedIndex == 0 || chkVerboseLogging.Checked == true) {
                if (e.EventType == ClientEventType.ClientConnectionStateChange) {
                    ConsoleAppendLine("State: ^6{0}", e.ConnectionState.ToString());

                    if (e.ConnectionState == ConnectionState.ConnectionDisconnected) {
                        btnConnect.Text = @"Connect";
                        pnlConnection.Enabled = true;
                    }
                    else {
                        pnlConnection.Enabled = false;
                        btnConnect.Text = @"Disconnect";
                    }
                }
                else if (e.EventType == ClientEventType.ClientConnectionFailure || e.EventType == ClientEventType.ClientSocketException) {
                    ConsoleAppendLine("^1Error: {0}", e.Now.Exceptions.FirstOrDefault());
                }
                else if (e.EventType == ClientEventType.ClientPacketSent && e.Now.Packets.Count > 0) {
                    ConsoleAppendLine("^2SEND: {0}", e.Now.Packets.First().DebugText);
                }
                else if (e.EventType == ClientEventType.ClientPacketReceived && e.Now.Packets.Count > 0) {
                    ConsoleAppendLine("^5RECV: {0}", e.Now.Packets.First().DebugText);
                }
            }
        }

        public void ConsoleAppendLine(string format, params object[] parameters) {
            rtbConsole.AppendText(string.Format("[{0}] {1}{2}", DateTime.Now.ToShortTimeString(), string.Format(format, parameters), Environment.NewLine));

            rtbConsole.ReadOnly = false;
            var consoleBoxLines = rtbConsole.Lines.Length;

            if (chkAnchorScrollbar.Checked == true) {
                rtbConsole.ScrollToCaret();
            }

            if (consoleBoxLines > 100 && rtbConsole.Focused == false) {
                for (var i = 0; i < consoleBoxLines - 100; i++) {
                    rtbConsole.Select(0, rtbConsole.Lines[0].Length + 1);
                    rtbConsole.SelectedText = string.Empty;
                }
            }

            rtbConsole.ReadOnly = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            NetworkConsoleModel.Connection.Dispose();
        }

        private void Execute(string commandText) {
            NetworkConsoleModel.Connection.Protocol.Action(new NetworkAction() {
                ActionType = NetworkActionType.NetworkPacketSend,
                Now = {
                    Content = new List<string>() {
                        commandText
                    }
                }
            });
        }

        private void txtConsoleText_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyData == Keys.Enter) {
                btnSend_Click(null, null);

                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == Keys.Up) {
                e.SuppressKeyPress = true;

                if (NetworkConsoleModel.CommandHistoryCurrentNode == null && NetworkConsoleModel.CommandHistory.First != null) {
                    NetworkConsoleModel.CommandHistoryCurrentNode = NetworkConsoleModel.CommandHistory.First;
                    txtConsoleText.Text = NetworkConsoleModel.CommandHistoryCurrentNode.Value;

                    txtConsoleText.Select(txtConsoleText.Text.Length, 0);
                }
                else if (NetworkConsoleModel.CommandHistoryCurrentNode != null && NetworkConsoleModel.CommandHistoryCurrentNode.Next != null) {
                    NetworkConsoleModel.CommandHistoryCurrentNode = NetworkConsoleModel.CommandHistoryCurrentNode.Next;
                    txtConsoleText.Text = NetworkConsoleModel.CommandHistoryCurrentNode.Value;

                    txtConsoleText.Select(txtConsoleText.Text.Length, 0);
                }
            }
            else if (e.KeyData == Keys.Down) {
                if (NetworkConsoleModel.CommandHistoryCurrentNode != null && NetworkConsoleModel.CommandHistoryCurrentNode.Previous != null) {
                    NetworkConsoleModel.CommandHistoryCurrentNode = NetworkConsoleModel.CommandHistoryCurrentNode.Previous;
                    txtConsoleText.Text = NetworkConsoleModel.CommandHistoryCurrentNode.Value;

                    txtConsoleText.Select(txtConsoleText.Text.Length, 0);
                }

                e.SuppressKeyPress = true;
            }
        }

        private void btnSend_Click(object sender, EventArgs e) {
            Execute(txtConsoleText.Text);

            NetworkConsoleModel.CommandHistory.AddFirst(txtConsoleText.Text);
            if (NetworkConsoleModel.CommandHistory.Count > 20) {
                NetworkConsoleModel.CommandHistory.RemoveLast();
            }
            NetworkConsoleModel.CommandHistoryCurrentNode = null;

            txtConsoleText.Clear();
            txtConsoleText.Focus();
        }

        private void txtHostname_TextChanged(object sender, EventArgs e) {
        }
    }
}