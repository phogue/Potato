// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Reflection;
using System.Timers;

namespace Procon.Net.Console {
    using Procon.Net;
    using Procon.Net.Protocols;
    using Procon.Net.Console.Utils;

    public partial class MainWindow : Form {

        private Dictionary<GameType, Type> Games { get; set; }

        // Console
        private LinkedList<string> m_commandHistory { get; set; }
        private LinkedListNode<string> m_commandHistoryCurrentNode { get; set; }

        private Game ActiveGame { get; set; }

        private System.Timers.Timer Timer { get; set; }

        public MainWindow() {
            InitializeComponent();

            this.m_commandHistory = new LinkedList<string>();

            this.Timer = new System.Timers.Timer(10000);
            this.Timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            this.Timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e) {
            if (this.ActiveGame != null) {
                this.ActiveGame.Synchronize();
            }
        }

        private void Form1_Load(object sender, EventArgs e) {

            this.lblVersion.Text = "Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            this.Games = Game.GetSupportedGames();
            /*
            Regex supportedGamesNamespame = new Regex(@"^Procon\.Net\.Protocols.*");

            this.Games = (from gameType in Assembly.GetAssembly(typeof(Game)).GetTypes()
                          where gameType != null &&
                          gameType.IsClass == true &&
                          gameType.IsAbstract == false &&
                          gameType.Namespace != null &&
                          supportedGamesNamespame.IsMatch(gameType.Namespace) == true &&
                          typeof(Game).IsAssignableFrom(gameType) == true &&
                          String.Compare(gameType.Name, typeof(Game).Name) != 0
                          select gameType).ToDictionary(x => x.Name);
            */
            this.cboGames.Items.AddRange(this.Games.Select(x=>x.Key.ToString()).ToArray());

            ConnectionDetails cd = new ConnectionDetails().Read();
            this.txtHostname.Text = cd.Hostname;
            this.txtPort.Text = cd.Port > 0 ? cd.Port.ToString() : String.Empty;
            this.txtPassword.Text = cd.Password;
            this.txtAdditional.Text = cd.Additional;

            if (this.cboGames.Items.Contains(cd.GameName) == true) {
                this.cboGames.SelectedIndex = this.cboGames.Items.IndexOf(cd.GameName);
            }
            else {
                this.cboGames.SelectedIndex = 0;
            }

            if (cd.IsLoaded == false) {
                MessageBox.Show("This tool is designed to aid developers with expanding procon's protocol repertoire, but can also be used for basic administration.  It's not pretty and it's not supposed to be." + Environment.NewLine + Environment.NewLine + "This message will not appear after you have sucessfully connected to a server.", "Procon 2 - Protocol Test Console", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e) {

            if (this.ActiveGame == null || (this.ActiveGame != null && this.ActiveGame.State != null && this.ActiveGame.State.Variables.ConnectionState == ConnectionState.Disconnected))
            {

                ushort port = 10156;

                if (ushort.TryParse(this.txtPort.Text, out port) == true && this.Games.ContainsKey((GameType)Enum.Parse(typeof(GameType), (string)this.cboGames.SelectedItem)) == true) {
                    this.txtPort.BackColor = SystemColors.Window;

                    this.ActiveGame = (Game)Activator.CreateInstance(this.Games[(GameType)Enum.Parse(typeof(GameType), (string)this.cboGames.SelectedItem)], this.txtHostname.Text, port);
                    this.ActiveGame.Password = this.txtPassword.Text;
                    this.ActiveGame.GameConfigPath = System.IO.Path.Combine(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs"), "Games");
                    this.ActiveGame.Additional = this.txtAdditional.Text;
                    this.chat1.ActiveGame = this.ActiveGame;
                    this.playerPanel1.ActiveGame = this.ActiveGame;
                    this.banPanel1.ActiveGame = this.ActiveGame;
                    this.mapPanel1.ActiveGame = this.ActiveGame;
                    this.eventPanel1.ActiveGame = this.ActiveGame;

                    //this.ActiveGame.ConnectionStateChanged += new Game.ConnectionStateChangedHandler(ActiveGame_ConnectionStateChanged);
                    //this.ActiveGame.ConnectionFailure += new Game.FailureHandler(ActiveGame_ConnectionFailure);
                    //this.ActiveGame.PacketReceived += new Game.PacketDispatchHandler(ActiveGame_PacketReceived);
                    //this.ActiveGame.PacketSent += new Game.PacketDispatchHandler(ActiveGame_PacketSent);

                    this.gameStatePropertyGrid.SelectedObject = this.ActiveGame;

                    this.ActiveGame.GameEvent += new Game.GameEventHandler(ActiveGame_GameEvent);
                    this.ActiveGame.ClientEvent += new Game.ClientEventHandler(ActiveGame_ClientEvent);

                    this.ActiveGame.AttemptConnection();
                }
                else {
                    this.txtPort.BackColor = ControlPaint.LightLight(Color.Maroon);
                }
            }
            else if (this.ActiveGame != null) {
                this.ActiveGame.Shutdown();
            }
        }

        void ActiveGame_GameEvent(Game sender, GameEventArgs e) {
            if (this.InvokeRequired == true) {
                this.Invoke(new Action<Game, GameEventArgs>(this.ActiveGame_GameEvent), sender, e);
                return;
            }

            this.gameStatePropertyGrid.Refresh();
        }

        void ActiveGame_ClientEvent(Game sender, ClientEventArgs e) {
            if (this.InvokeRequired == true) {
                this.Invoke(new Action<Game, ClientEventArgs>(this.ActiveGame_ClientEvent), sender, e);
                return;
            }

            if (this.tbcPanels.SelectedIndex == 0 || this.chkVerboseLogging.Checked == true) {
                if (e.EventType == ClientEventType.ConnectionStateChange) {
                    this.ConsoleAppendLine("State: ^6{0}", e.ConnectionState.ToString());

                    if (e.ConnectionState == Net.ConnectionState.Disconnected) {
                        this.btnConnect.Text = "Connect";
                        this.pnlConnection.Enabled = true;
                    }
                    else {
                        this.pnlConnection.Enabled = false;
                        this.btnConnect.Text = "Disconnect";
                    }
                }
                else if (e.EventType == ClientEventType.ConnectionFailure || e.EventType == ClientEventType.SocketException) {
                    this.ConsoleAppendLine("^1Error: {0}", e.ConnectionError.Message);
                }
                else if (e.EventType == ClientEventType.PacketSent) {
                    this.ConsoleAppendLine("^2SEND: {0}", e.Packet.ToDebugString());
                }
                else if (e.EventType == ClientEventType.PacketReceived) {
                    this.ConsoleAppendLine("^5RECV: {0}", e.Packet.ToDebugString());
                }
            }

            this.gameStatePropertyGrid.Refresh();
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
            if (this.ActiveGame != null) {
                new ConnectionDetails() {
                    GameName = this.cboGames.SelectedItem.ToString(),
                    Hostname = this.ActiveGame.Hostname,
                    Port = this.ActiveGame.Port,
                    Password = this.ActiveGame.Password,
                    Additional = this.ActiveGame.Additional
                }.Write();

                this.ActiveGame.Shutdown();
            }
        }

        private void Execute(string commandText) {

            if (this.ActiveGame != null) {
                this.ActiveGame.Raw(commandText);
            }
        }

        private void txtConsoleText_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyData == Keys.Enter) {

                this.btnSend_Click(null, null);

                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == Keys.Up) {
                e.SuppressKeyPress = true;

                if (this.m_commandHistoryCurrentNode == null && this.m_commandHistory.First != null) {
                    this.m_commandHistoryCurrentNode = this.m_commandHistory.First;
                    this.txtConsoleText.Text = this.m_commandHistoryCurrentNode.Value;

                    this.txtConsoleText.Select(this.txtConsoleText.Text.Length, 0);
                }
                else if (this.m_commandHistoryCurrentNode != null && this.m_commandHistoryCurrentNode.Next != null) {
                    this.m_commandHistoryCurrentNode = this.m_commandHistoryCurrentNode.Next;
                    this.txtConsoleText.Text = this.m_commandHistoryCurrentNode.Value;

                    this.txtConsoleText.Select(this.txtConsoleText.Text.Length, 0);
                }
            }
            else if (e.KeyData == Keys.Down) {

                if (this.m_commandHistoryCurrentNode != null && this.m_commandHistoryCurrentNode.Previous != null) {
                    this.m_commandHistoryCurrentNode = this.m_commandHistoryCurrentNode.Previous;
                    this.txtConsoleText.Text = this.m_commandHistoryCurrentNode.Value;

                    this.txtConsoleText.Select(this.txtConsoleText.Text.Length, 0);
                }

                e.SuppressKeyPress = true;
            }
        }

        private void btnSend_Click(object sender, EventArgs e) {
            this.Execute(this.txtConsoleText.Text);

            this.m_commandHistory.AddFirst(this.txtConsoleText.Text);
            if (this.m_commandHistory.Count > 20) {
                this.m_commandHistory.RemoveLast();
            }
            this.m_commandHistoryCurrentNode = null;

            this.txtConsoleText.Clear();
            this.txtConsoleText.Focus();
        }

        private void txtHostname_TextChanged(object sender, EventArgs e) {

        }
        
    }
}
