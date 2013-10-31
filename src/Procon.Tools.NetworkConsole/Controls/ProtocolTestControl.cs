using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using Procon.Net;
using Procon.Net.Utils;
using ConnectionState = Procon.Net.ConnectionState;
using Procon.Net.Utils.Tests;
using Procon.Core.Utils;

namespace Procon.Tools.NetworkConsole.Controls {
    public partial class ProtocolTestControl : UserControl {

        public bool IsTestRunning { get; set; }

        public Game ActiveGame {
            get {
                return this._activeGame;
            }
            set {
                this._activeGame = value;

                // Assign events.
                if (this._activeGame != null) {
                    this._activeGame.GameEvent += new Game.GameEventHandler(m_activeGame_GameEvent);
                    this._activeGame.ClientEvent += new Game.ClientEventHandler(m_activeGame_ClientEvent);
                }
            }
        }
        private Game _activeGame;

        /// <summary>
        /// 
        /// </summary>
        protected ProtocolTestRun ProtocolTestRun;

        public ProtocolTestControl() {
            InitializeComponent();
        }


        private void m_activeGame_ClientEvent(Game sender, ClientEventArgs e) {
            if (this.InvokeRequired == true) {
                this.Invoke(new Action<Game, ClientEventArgs>(this.m_activeGame_ClientEvent), sender, e);
                return;
            }

            if (e.ConnectionState == ConnectionState.ConnectionConnected) {

            }
        }

        private void m_activeGame_GameEvent(Game sender, GameEventArgs e) {
            if (this.InvokeRequired == true) {
                this.Invoke(new Action<Game, GameEventArgs>(this.m_activeGame_GameEvent), sender, e);
                return;
            }

            if (e.GameEventType == GameEventType.GamePlayerlistUpdated) {

            }

        }

        public void ConsoleAppendLine(string format, params object[] parameters) {
            this.rtbProtocolTestOutput.AppendText(String.Format("[{0}] {1}{2}", DateTime.Now.ToShortTimeString(), String.Format(format, parameters), Environment.NewLine));

            this.rtbProtocolTestOutput.ScrollToCaret();
        }

        protected void OutputLoadedTest() {

            this.ConsoleAppendLine("Loaded {0} tests", this.ProtocolTestRun.Tests.Count);
        }

        protected void LoadProtocolTest() {
            this.rtbProtocolTestOutput.Text = String.Empty;

            // Dispose of the old object, we're just about to replace it.
            if (this.ProtocolTestRun != null) {
                this.ProtocolTestRun.Dispose();
            }

            try {
                XDocument document = XDocument.Load(this.txtProtocolTestFile.Text);

                this.ProtocolTestRun = document.Root.FromXElement<ProtocolTestRun>();

                this.ProtocolTestRun.ConnecionIsolation = this.chkConnectionIsolation.Checked;

                String selectedTestText = String.IsNullOrEmpty(this.cboTests.Text) == true ? "All" : this.cboTests.Text;

                this.cboTests.Items.Clear();
                this.cboTests.Items.Add("All");

                for (int offset = 0; offset < this.ProtocolTestRun.Tests.Count; offset++) {
                    this.ProtocolTestRun.Tests[offset].Name = String.Format("{0} - {1}", offset.ToString("D4"), this.ProtocolTestRun.Tests[offset].Name);

                    this.cboTests.Items.Add(this.ProtocolTestRun.Tests[offset]);
                }

                // Reselect whatever test was selected.
                this.cboTests.Text = selectedTestText;

                this.OutputLoadedTest();

                this.btnRun.Enabled = true;
                this.btnReload.Enabled = true;
            }
            catch (Exception e) {
                this.rtbProtocolTestOutput.AppendText(e.Message);

                this.btnReload.Enabled = false;
            }

        }

        private void btnBrowse_Click(object sender, EventArgs e) {
            OpenFileDialog fileSearch;

            if (String.IsNullOrEmpty(this.txtProtocolTestFile.Text) == true) {
                fileSearch = new OpenFileDialog {
                    InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tests")
                };
            }
            else {
                fileSearch = new OpenFileDialog {
                    FileName = this.txtProtocolTestFile.Text
                };
            }
            
            fileSearch.ShowDialog();

            if (File.Exists(fileSearch.FileName) == true) {
                this.txtProtocolTestFile.Text = fileSearch.FileName;

                this.LoadProtocolTest();
            }
        }

        private void btnRun_Click(object sender, EventArgs e) {

            if (this.ActiveGame != null && this.ProtocolTestRun != null && this.IsTestRunning == false) {
                this.rtbProtocolTestOutput.Text = String.Empty;

                if (this.ProtocolTestRun.ConnecionIsolation == true) {
                    this.ConsoleAppendLine("Connection Isolation: ^2true^0 (tests will reconnect & login prior to initiating test)");
                }
                else {
                    this.ConsoleAppendLine("Connection Isolation: ^1false^0 (tests will run with the same open connection)");
                }

                List<ProtocolUnitTest> tests = this.ProtocolTestRun.Tests.Where(test => test.Name == this.cboTests.Text).ToList();
                
                ThreadPool.QueueUserWorkItem(delegate {
                    this.IsTestRunning = true;

                    this.ProtocolTestRun.TestBegin += new Net.Utils.Tests.ProtocolTestRun.TestEventHandler(ProtocolTestRun_TestBegin);
                    this.ProtocolTestRun.TestSuccess += new Net.Utils.Tests.ProtocolTestRun.TestEventHandler(ProtocolTestRun_TestSuccess);
                    this.ProtocolTestRun.TestFailed += new Net.Utils.Tests.ProtocolTestRun.TestEventHandler(ProtocolTestRun_TestFailed);

                    // No specific test selected, run them all.
                    if (tests.Count == 0) {
                        this.ProtocolTestRun.Execute(this.ActiveGame);
                    }
                    else {
                        this.ProtocolTestRun.Execute(this.ActiveGame, tests);
                    }

                    this.ProtocolTestRun.TestBegin -= new Net.Utils.Tests.ProtocolTestRun.TestEventHandler(ProtocolTestRun_TestBegin);
                    this.ProtocolTestRun.TestSuccess -= new Net.Utils.Tests.ProtocolTestRun.TestEventHandler(ProtocolTestRun_TestSuccess);
                    this.ProtocolTestRun.TestFailed -= new Net.Utils.Tests.ProtocolTestRun.TestEventHandler(ProtocolTestRun_TestFailed);

                    this.IsTestRunning = false;
                });
            }
        }

        protected void OutputTestResult(ProtocolUnitTest test) {
            this.ConsoleAppendLine("TIME ELAPSED: {0}", test.End - test.Start);

            test.TestEvent -= new ProtocolUnitTest.TestEventHandler(Test_TestEvent);
            test.TestSetup -= new ProtocolUnitTest.TestEventHandler(Test_TestSetup);
            this.ActiveGame.ClientEvent -= new Game.ClientEventHandler(ActiveGame_ClientEvent);
        }

        protected void ProtocolTestRun_TestFailed(ProtocolTestRun sender, ProtocolUnitTestEventArgs e) {
            if (this.InvokeRequired == true) {
                this.Invoke(new Action<ProtocolTestRun, ProtocolUnitTestEventArgs>(this.ProtocolTestRun_TestFailed), sender, e);
                return;
            }

            this.OutputTestResult(e.Test);

            this.ConsoleAppendLine("END: [^1FAILED^0]");
        }

        protected void ProtocolTestRun_TestSuccess(ProtocolTestRun sender, ProtocolUnitTestEventArgs e) {
            if (this.InvokeRequired == true) {
                this.Invoke(new Action<ProtocolTestRun, ProtocolUnitTestEventArgs>(this.ProtocolTestRun_TestSuccess), sender, e);
                return;
            }

            this.OutputTestResult(e.Test);

            this.ConsoleAppendLine("END: [^2SUCCESS^0]");
        }

        protected void ProtocolTestRun_TestBegin(ProtocolTestRun sender, ProtocolUnitTestEventArgs e) {
            if (this.InvokeRequired == true) {
                this.Invoke(new Action<ProtocolTestRun, ProtocolUnitTestEventArgs>(this.ProtocolTestRun_TestBegin), sender, e);
                return;
            }

            this.ConsoleAppendLine("===========================================================================================");
            this.ConsoleAppendLine("BEGIN: {0}", e.Test.Name);

            e.Test.TestEvent += new ProtocolUnitTest.TestEventHandler(Test_TestEvent);
            e.Test.TestSetup += new ProtocolUnitTest.TestEventHandler(Test_TestSetup);
        }

        protected void Test_TestSetup(ProtocolUnitTest sender, ProtocolUnitTestEventArgs args) {
            this.ActiveGame.ClientEvent += new Game.ClientEventHandler(ActiveGame_ClientEvent);
        }

        protected void ActiveGame_ClientEvent(Game sender, ClientEventArgs e) {
            if (this.InvokeRequired == true) {
                this.Invoke(new Action<Game, ClientEventArgs>(this.ActiveGame_ClientEvent), sender, e);
                return;
            }

            if (e.EventType == ClientEventType.ClientConnectionStateChange) {
                this.ConsoleAppendLine("State: ^6{0}", e.ConnectionState.ToString());
            }
            else if (e.EventType == ClientEventType.ClientConnectionFailure || e.EventType == ClientEventType.ClientSocketException) {
                this.ConsoleAppendLine("^1Error: {0}", e.ConnectionError.Message);
            }
            else if (e.EventType == ClientEventType.ClientPacketSent) {
                this.ConsoleAppendLine("^2SEND: {0}", e.Packet.ToDebugString());
            }
            else if (e.EventType == ClientEventType.ClientPacketReceived) {
                this.ConsoleAppendLine("^5RECV: {0}", e.Packet.ToDebugString());
            }
        }

        protected void Test_TestEvent(ProtocolUnitTest sender, ProtocolUnitTestEventArgs e) {
            if (this.InvokeRequired == true) {
                this.Invoke(new Action<ProtocolUnitTest, ProtocolUnitTestEventArgs>(this.Test_TestEvent), sender, e);
                return;
            }

            if (String.IsNullOrEmpty(e.Message) == false) {
                this.ConsoleAppendLine("{0}", e.Message);
            }
        }

        private void btnReload_Click(object sender, EventArgs e) {
            this.LoadProtocolTest();
        }

        private void chkConnectionIsolation_CheckedChanged(object sender, EventArgs e) {
            if (this.ProtocolTestRun != null) {
                this.ProtocolTestRun.ConnecionIsolation = this.chkConnectionIsolation.Checked;
            }
        }
    }
}
