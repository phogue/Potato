using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.Timers;
using Procon.Net.Protocols;
using Procon.Net.Shared;
using Procon.Net.Shared.Actions;

namespace Procon.Tools.NetworkConsole {
    using Procon.Net;
    using Procon.Tools.NetworkConsole.Utils;

    public partial class MainWindow : Form {

        private Dictionary<IGameType, Type> Games { get; set; }

        // Console
        private LinkedList<string> CommandHistory { get; set; }
        private LinkedListNode<string> CommandHistoryCurrentNode { get; set; }

        private Game ActiveGame { get; set; }

        private System.Timers.Timer Timer { get; set; }

        public MainWindow() {
            InitializeComponent();

            this.CommandHistory = new LinkedList<string>();

            this.Timer = new System.Timers.Timer(10000);
            this.Timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            this.Timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e) {
            if (this.ActiveGame != null && this.protocolTest1.IsTestRunning == false) {
                this.ActiveGame.Synchronize();
            }
        }

        private void Form1_Load(object sender, EventArgs e) {

            this.lblVersion.Text = "Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            this.Games = SupportedGameTypes.GetSupportedGames();
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
            this.cboGames.Items.AddRange(this.Games.Select(game => game.Key.Type).ToArray());

            ConnectionDetails cd = new ConnectionDetails().Read();
            this.txtHostname.Text = cd.Hostname;
            this.txtPort.Text = cd.Port > 0 ? cd.Port.ToString(CultureInfo.InvariantCulture) : String.Empty;
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

            if (this.ActiveGame == null || (this.ActiveGame != null && this.ActiveGame.State != null && this.ActiveGame.State.Settings.Current.ConnectionState == ConnectionState.ConnectionDisconnected))
            {

                ushort port = 10156;

                if (ushort.TryParse(this.txtPort.Text, out port) == true && this.Games.Any(game => game.Key.Type == (string)this.cboGames.SelectedItem) == true) {
                    this.txtPort.BackColor = SystemColors.Window;

                    this.ActiveGame = (Game)Activator.CreateInstance(this.Games.First(game => game.Key.Type == (string)this.cboGames.SelectedItem).Value, this.txtHostname.Text, port);
                    this.ActiveGame.Password = this.txtPassword.Text;
                    this.ActiveGame.GameConfigPath = System.IO.Path.Combine(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs"), "Games");
                    this.ActiveGame.Additional = this.txtAdditional.Text;
                    this.chat1.ActiveGame = this.ActiveGame;
                    this.playerPanel1.ActiveGame = this.ActiveGame;
                    this.banPanel1.ActiveGame = this.ActiveGame;
                    this.mapPanel1.ActiveGame = this.ActiveGame;
                    this.eventPanel1.ActiveGame = this.ActiveGame;
                    this.protocolTest1.ActiveGame = this.ActiveGame;

                    //this.ActiveGame.ConnectionStateChanged += new Game.ConnectionStateChangedHandler(ActiveGame_ConnectionStateChanged);
                    //this.ActiveGame.ConnectionFailure += new Game.FailureHandler(ActiveGame_ConnectionFailure);
                    //this.ActiveGame.PacketReceived += new Game.PacketDispatchHandler(ActiveGame_PacketReceived);
                    //this.ActiveGame.PacketSent += new Game.PacketDispatchHandler(ActiveGame_PacketSent);

                    this.gameStatePropertyGrid.SelectedObject = this.ActiveGame;

                    this.ActiveGame.GameEvent += ActiveGame_GameEvent;
                    this.ActiveGame.ClientEvent += ActiveGame_ClientEvent;

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

        void ActiveGame_GameEvent(IGame sender, GameEventArgs e) {
            if (this.InvokeRequired == true) {
                this.Invoke(new Action<Game, GameEventArgs>(this.ActiveGame_GameEvent), sender, e);
                return;
            }

            this.gameStatePropertyGrid.Refresh();
        }

        void ActiveGame_ClientEvent(IGame sender, ClientEventArgs e) {
            if (this.InvokeRequired == true) {
                this.Invoke(new Action<Game, ClientEventArgs>(this.ActiveGame_ClientEvent), sender, e);
                return;
            }

            if (this.tbcPanels.SelectedIndex == 0 || this.chkVerboseLogging.Checked == true) {
                if (e.EventType == ClientEventType.ClientConnectionStateChange) {
                    this.ConsoleAppendLine("State: ^6{0}", e.ConnectionState.ToString());

                    if (e.ConnectionState == ConnectionState.ConnectionDisconnected) {
                        this.btnConnect.Text = "Connect";
                        this.pnlConnection.Enabled = true;
                    }
                    else {
                        this.pnlConnection.Enabled = false;
                        this.btnConnect.Text = "Disconnect";
                    }
                }
                else if (e.EventType == ClientEventType.ClientConnectionFailure || e.EventType == ClientEventType.ClientSocketException) {
                    this.ConsoleAppendLine("^1Error: {0}", e.Now.Exceptions.FirstOrDefault().Message);
                }
                else if (e.EventType == ClientEventType.ClientPacketSent) {
                    this.ConsoleAppendLine("^2SEND: {0}", e.Now.Packets.FirstOrDefault().DebugText);
                }
                else if (e.EventType == ClientEventType.ClientPacketReceived) {
                    this.ConsoleAppendLine("^5RECV: {0}", e.Now.Packets.FirstOrDefault().DebugText);
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
                    Hostname = this.ActiveGame.Client.Hostname,
                    Port = this.ActiveGame.Client.Port,
                    Password = this.ActiveGame.Password,
                    Additional = this.ActiveGame.Additional
                }.Write();

                this.ActiveGame.Shutdown();
            }
        }

        private void Execute(string commandText) {

            if (this.ActiveGame != null) {
                this.ActiveGame.Action(new Raw() {
                    ActionType = NetworkActionType.NetworkSend,
                    Now = {
                        Content = {
                            commandText
                        }
                    }
                });
            }
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
