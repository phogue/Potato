﻿#region Copyright
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
using System.Xml.Serialization;
using Potato.Net.Shared.Sandbox;

namespace Potato.Net.Utils.Tests {

    [Serializable]
    public class ProtocolTestRun : IDisposable {

        /// <summary>
        /// When the run was started
        /// </summary>
        [XmlIgnore]
        public DateTime Start { get; set; }

        /// <summary>
        /// When the test run finished.
        /// </summary>
        [XmlIgnore]
        public DateTime End { get; set; }

        /// <summary>
        /// List of replacements to look for in each command sent.
        /// </summary>
        [XmlArray(ElementName = "Replacements")]
        [XmlArrayItem(ElementName = "Replacement")]
        public List<ProtocolUnitTestObject> Replacements { get; set; }

        /// <summary>
        /// List to tests to run.
        /// </summary>
        [XmlArray(ElementName = "Tests")]
        [XmlArrayItem(ElementName = "Test")]
        public List<ProtocolUnitTest> Tests { get; set; }

        /// <summary>
        /// Flag specifying if tests should first disconnect, then reconnect so they establish an isolated
        /// connection state for a test.
        /// </summary>
        public bool ConnecionIsolation { get; set; }

        /// <summary>
        /// Fired whenever a test has started executing
        /// </summary>
        public event TestEventHandler TestBegin;

        /// <summary>
        /// Fired whenever a test is successfully completed.
        /// </summary>
        public event TestEventHandler TestSuccess;

        /// <summary>
        /// Fired whenever a test failed.
        /// </summary>
        public event TestEventHandler TestFailed;

        public delegate void TestEventHandler(ProtocolTestRun sender, ProtocolUnitTestEventArgs args);

        public ProtocolTestRun() {
            this.ConnecionIsolation = true;
        }

        protected virtual void OnTestBegin(ProtocolUnitTestEventArgs args) {
            TestEventHandler handler = TestBegin;
            if (handler != null) {
                handler(this, args);
            }
        }

        protected virtual void OnTestSuccess(ProtocolUnitTestEventArgs args) {
            TestEventHandler handler = TestSuccess;
            if (handler != null) {
                handler(this, args);
            }
        }

        protected virtual void OnTestFailed(ProtocolUnitTestEventArgs args) {
            TestEventHandler handler = TestFailed;
            if (handler != null) {
                handler(this, args);
            }
        }

        protected void ReplaceText() {

            List<ProtocolUnitTestCommand> commands = this.Tests.SelectMany(test => test.TestCommands).ToList();

            List<ProtocolUnitTestPacket> packets = new List<ProtocolUnitTestPacket>();
            packets.AddRange(commands.SelectMany(command => command.Requests).ToList());
            packets.AddRange(commands.SelectMany(command => command.Responses).ToList());
            packets.AddRange(commands.Select(command => command.Send).ToList());

            foreach (ProtocolUnitTestObject replacement in this.Replacements) {
                foreach (ProtocolUnitTestPacket packet in packets) {
                    packet.ReplaceWith(replacement);
                }
            }
        }

        /// <summary>
        /// Kind of late to the game, decided to change how the UI allows for tests to be re-run and such.
        /// </summary>
        protected void FalseAllFoundFlags() {
            List<ProtocolUnitTestCommand> commands = this.Tests.SelectMany(test => test.TestCommands).ToList();

            List<ProtocolUnitTestPacket> packets = new List<ProtocolUnitTestPacket>();
            packets.AddRange(commands.SelectMany(command => command.Requests).ToList());
            packets.AddRange(commands.SelectMany(command => command.Responses).ToList());
            packets.AddRange(commands.Select(command => command.Send).ToList());

            packets.ForEach(packet => packet.Found = false);
        }

        /// <summary>
        /// Executes only the tests passed in
        /// </summary>
        /// <param name="game"></param>
        /// <param name="tests"></param>
        public void Execute(ISandboxProtocolController game, List<ProtocolUnitTest> tests) {
            this.Start = DateTime.Now;

            this.ReplaceText();

            this.FalseAllFoundFlags();

            foreach (ProtocolUnitTest test in tests) {

                this.OnTestBegin(new ProtocolUnitTestEventArgs() {
                    Test = test
                });

                if (test.Execute(game, this.ConnecionIsolation) == true) {
                    this.OnTestSuccess(new ProtocolUnitTestEventArgs() {
                        Test = test
                    });
                }
                else {
                    this.OnTestFailed(new ProtocolUnitTestEventArgs() {
                        Test = test
                    });
                }
            }

            this.End = DateTime.Now;
        }

        /// <summary>
        /// Executes all of the tests.
        /// </summary>
        /// <param name="game"></param>
        public void Execute(ISandboxProtocolController game) {
            this.Execute(game, this.Tests);
        }

        public void Dispose() {
            this.Replacements.ForEach(e => e.Dispose());
            this.Tests.ForEach(e => e.Dispose());
        }
    }
}
