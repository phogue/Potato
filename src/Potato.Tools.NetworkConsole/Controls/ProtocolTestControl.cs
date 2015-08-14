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
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using Potato.Net.Shared;
using Potato.Tools.NetworkConsole.Models;
using Potato.Tools.NetworkConsole.Utils;
using Potato.Net.Utils.Tests;

namespace Potato.Tools.NetworkConsole.Controls {
    public partial class ProtocolTestControl : UserControl {

        public bool IsTestRunning { get; set; }

        public NetworkConsoleModel NetworkConsoleModel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected ProtocolTestRun ProtocolTestRun;

        public ProtocolTestControl() {
            InitializeComponent();
        }

        public void ConsoleAppendLine(string format, params object[] parameters) {
            rtbProtocolTestOutput.AppendText(string.Format("[{0}] {1}{2}", DateTime.Now.ToShortTimeString(), string.Format(format, parameters), Environment.NewLine));

            rtbProtocolTestOutput.ScrollToCaret();
        }

        protected void OutputLoadedTest() {

            ConsoleAppendLine("Loaded {0} tests", ProtocolTestRun.Tests.Count);
        }

        protected void LoadProtocolTest() {
            rtbProtocolTestOutput.Text = string.Empty;

            // Dispose of the old object, we're just about to replace it.
            if (ProtocolTestRun != null) {
                ProtocolTestRun.Dispose();
            }

            try {
                var document = XDocument.Load(txtProtocolTestFile.Text);

                ProtocolTestRun = document.Root.FromXElement<ProtocolTestRun>();

                ProtocolTestRun.ConnecionIsolation = chkConnectionIsolation.Checked;

                var selectedTestText = string.IsNullOrEmpty(cboTests.Text) == true ? "All" : cboTests.Text;

                cboTests.Items.Clear();
                cboTests.Items.Add("All");

                for (var offset = 0; offset < ProtocolTestRun.Tests.Count; offset++) {
                    ProtocolTestRun.Tests[offset].Name = string.Format("{0} - {1}", offset.ToString("D4"), ProtocolTestRun.Tests[offset].Name);

                    cboTests.Items.Add(ProtocolTestRun.Tests[offset]);
                }

                // Reselect whatever test was selected.
                cboTests.Text = selectedTestText;

                OutputLoadedTest();

                btnRun.Enabled = true;
                btnReload.Enabled = true;
            }
            catch (Exception e) {
                rtbProtocolTestOutput.AppendText(e.Message);

                btnReload.Enabled = false;
            }

        }

        private void btnBrowse_Click(object sender, EventArgs e) {
            OpenFileDialog fileSearch;

            if (string.IsNullOrEmpty(txtProtocolTestFile.Text) == true) {
                fileSearch = new OpenFileDialog {
                    InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tests")
                };
            }
            else {
                fileSearch = new OpenFileDialog {
                    FileName = txtProtocolTestFile.Text
                };
            }
            
            fileSearch.ShowDialog();

            if (File.Exists(fileSearch.FileName) == true) {
                txtProtocolTestFile.Text = fileSearch.FileName;

                LoadProtocolTest();
            }
        }

        private void btnRun_Click(object sender, EventArgs e) {

            if (NetworkConsoleModel.Connection != null && ProtocolTestRun != null && IsTestRunning == false) {
                rtbProtocolTestOutput.Text = string.Empty;

                if (ProtocolTestRun.ConnecionIsolation == true) {
                    ConsoleAppendLine("Connection Isolation: ^2true^0 (tests will reconnect & login prior to initiating test)");
                }
                else {
                    ConsoleAppendLine("Connection Isolation: ^1false^0 (tests will run with the same open connection)");
                }

                var tests = ProtocolTestRun.Tests.Where(test => test.Name == cboTests.Text).ToList();
                
                ThreadPool.QueueUserWorkItem(delegate {
                    this.IsTestRunning = true;
                    this.NetworkConsoleModel.SynchornizationEnabled = false;

                    this.ProtocolTestRun.TestBegin += new Net.Utils.Tests.ProtocolTestRun.TestEventHandler(ProtocolTestRun_TestBegin);
                    this.ProtocolTestRun.TestSuccess += new Net.Utils.Tests.ProtocolTestRun.TestEventHandler(ProtocolTestRun_TestSuccess);
                    this.ProtocolTestRun.TestFailed += new Net.Utils.Tests.ProtocolTestRun.TestEventHandler(ProtocolTestRun_TestFailed);

                    // No specific test selected, run them all.
                    if (tests.Count == 0) {
                        this.ProtocolTestRun.Execute(this.NetworkConsoleModel.Connection.Protocol);
                    }
                    else {
                        this.ProtocolTestRun.Execute(this.NetworkConsoleModel.Connection.Protocol, tests);
                    }

                    this.ProtocolTestRun.TestBegin -= new Net.Utils.Tests.ProtocolTestRun.TestEventHandler(ProtocolTestRun_TestBegin);
                    this.ProtocolTestRun.TestSuccess -= new Net.Utils.Tests.ProtocolTestRun.TestEventHandler(ProtocolTestRun_TestSuccess);
                    this.ProtocolTestRun.TestFailed -= new Net.Utils.Tests.ProtocolTestRun.TestEventHandler(ProtocolTestRun_TestFailed);

                    this.IsTestRunning = false;
                    this.NetworkConsoleModel.SynchornizationEnabled = true;
                });
            }
        }

        protected void OutputTestResult(ProtocolUnitTest test) {
            ConsoleAppendLine("TIME ELAPSED: {0}", test.End - test.Start);

            test.TestEvent -= new ProtocolUnitTest.TestEventHandler(Test_TestEvent);
            test.TestSetup -= new ProtocolUnitTest.TestEventHandler(Test_TestSetup);
            NetworkConsoleModel.Connection.ClientEvent -= new Action<IClientEventArgs>(Connection_ClientEvent);
        }

        protected void ProtocolTestRun_TestFailed(ProtocolTestRun sender, ProtocolUnitTestEventArgs e) {
            if (InvokeRequired == true) {
                Invoke(new Action<ProtocolTestRun, ProtocolUnitTestEventArgs>(ProtocolTestRun_TestFailed), sender, e);
                return;
            }

            OutputTestResult(e.Test);

            ConsoleAppendLine("END: [^1FAILED^0]");
        }

        protected void ProtocolTestRun_TestSuccess(ProtocolTestRun sender, ProtocolUnitTestEventArgs e) {
            if (InvokeRequired == true) {
                Invoke(new Action<ProtocolTestRun, ProtocolUnitTestEventArgs>(ProtocolTestRun_TestSuccess), sender, e);
                return;
            }

            OutputTestResult(e.Test);

            ConsoleAppendLine("END: [^2SUCCESS^0]");
        }

        protected void ProtocolTestRun_TestBegin(ProtocolTestRun sender, ProtocolUnitTestEventArgs e) {
            if (InvokeRequired == true) {
                Invoke(new Action<ProtocolTestRun, ProtocolUnitTestEventArgs>(ProtocolTestRun_TestBegin), sender, e);
                return;
            }

            ConsoleAppendLine("===========================================================================================");
            ConsoleAppendLine("BEGIN: {0}", e.Test.Name);

            e.Test.TestEvent += new ProtocolUnitTest.TestEventHandler(Test_TestEvent);
            e.Test.TestSetup += new ProtocolUnitTest.TestEventHandler(Test_TestSetup);
        }

        protected void Test_TestSetup(ProtocolUnitTest sender, ProtocolUnitTestEventArgs args) {
            NetworkConsoleModel.Connection.ClientEvent += new Action<IClientEventArgs>(Connection_ClientEvent);
        }

        protected void Connection_ClientEvent(IClientEventArgs e) {
            if (InvokeRequired == true) {
                Invoke(new Action<ClientEventArgs>(Connection_ClientEvent), e);
                return;
            }

            if (e.EventType == ClientEventType.ClientConnectionStateChange) {
                ConsoleAppendLine("State: ^6{0}", e.ConnectionState.ToString());
            }
            else if (e.EventType == ClientEventType.ClientConnectionFailure || e.EventType == ClientEventType.ClientSocketException) {
                ConsoleAppendLine("^1Error: {0}", e.Now.Exceptions.FirstOrDefault());
            }
            else if (e.EventType == ClientEventType.ClientPacketSent) {
                ConsoleAppendLine("^2SEND: {0}", e.Now.Packets.First().DebugText);
            }
            else if (e.EventType == ClientEventType.ClientPacketReceived) {
                ConsoleAppendLine("^5RECV: {0}", e.Now.Packets.First().DebugText);
            }
        }

        protected void Test_TestEvent(ProtocolUnitTest sender, ProtocolUnitTestEventArgs e) {
            if (InvokeRequired == true) {
                Invoke(new Action<ProtocolUnitTest, ProtocolUnitTestEventArgs>(Test_TestEvent), sender, e);
                return;
            }

            if (string.IsNullOrEmpty(e.Message) == false) {
                ConsoleAppendLine("{0}", e.Message);
            }
        }

        private void btnReload_Click(object sender, EventArgs e) {
            LoadProtocolTest();
        }

        private void chkConnectionIsolation_CheckedChanged(object sender, EventArgs e) {
            if (ProtocolTestRun != null) {
                ProtocolTestRun.ConnecionIsolation = chkConnectionIsolation.Checked;
            }
        }
    }
}
