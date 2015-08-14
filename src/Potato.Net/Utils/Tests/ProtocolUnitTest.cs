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
using System.Threading;
using System.Xml.Serialization;
using Potato.Net.Shared;
using Potato.Net.Shared.Actions;
using Potato.Net.Shared.Sandbox;

namespace Potato.Net.Utils.Tests {

    [Serializable]
    public class ProtocolUnitTest : IDisposable {

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
        /// How long this test should remain before timing out with an error.
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Friendly name for this test.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List to run.
        /// </summary>
        [XmlArray(ElementName = "TestCommands")]
        [XmlArrayItem(ElementName = "TestCommand")]
        public List<ProtocolUnitTestCommand> TestCommands { get; set; }

        /// <summary>
        /// Fired whenever a log event/action for use in displaying debug output.
        /// </summary>
        public event TestEventHandler TestEvent;

        /// <summary>
        /// Fired when the test is setup (we have disconnected and logged back in, we're ready to go)
        /// </summary>
        public event TestEventHandler TestSetup;

        protected virtual void OnTestSetup(ProtocolUnitTestEventArgs args) {
            var handler = TestSetup;
            if (handler != null) {
                handler(this, args);
            } 
        }

        protected virtual void OnTestEvent(ProtocolUnitTestEventArgs args) {
            var handler = TestEvent;
            if (handler != null) {
                handler(this, args);
            } 
        }

        public delegate void TestEventHandler(ProtocolUnitTest sender, ProtocolUnitTestEventArgs args);

        /// <summary>
        /// Asserts the game client disconnects, or is already disconnected.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        protected bool Disconnect(ISandboxProtocolController game) {
            var disconnected = true;

            if (game.State != null && game.State.Settings.Current.ConnectionState != ConnectionState.ConnectionDisconnected) {
                var disconnectEvent = new AutoResetEvent(false);

                Action<IClientEventArgs> handler = (args) => {
                    if (args.EventType == ClientEventType.ClientConnectionStateChange) {
                        if (args.ConnectionState == ConnectionState.ConnectionDisconnected) {
                            disconnectEvent.Set();
                        }
                    }
                };

                var originalBubbleProxy = game.Bubble;

                game.Bubble = new SandboxProtocolCallbackProxy() {
                    ClientEvent = new Action<IClientEventArgs>(handler)
                };

                game.Shutdown();
                
                if ((disconnected = disconnectEvent.WaitOne(Timeout * 1000)) == false) {
                    OnTestEvent(new ProtocolUnitTestEventArgs() { Message = "Timeout on client disconnection." });
                }

                game.Bubble = originalBubbleProxy;
            }

            return disconnected;
        }

        /// <summary>
        /// Asserts a connection has been established to the server and has properly logged in to begin testing.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        protected bool LoggedIn(ISandboxProtocolController game) {
            var loggedIn = true;

            if (game.State == null || game.State.Settings.Current.ConnectionState != ConnectionState.ConnectionLoggedIn) {
                var loginEvent = new AutoResetEvent(false);

                Action<IClientEventArgs> handler = (args) => {
                    if (args.EventType == ClientEventType.ClientConnectionStateChange) {
                        if (args.ConnectionState == ConnectionState.ConnectionLoggedIn) {
                            loginEvent.Set();
                        }
                    }
                };

                var originalBubbleProxy = game.Bubble;

                game.Bubble = new SandboxProtocolCallbackProxy() {
                    ClientEvent = new Action<IClientEventArgs>(handler)
                };

                game.AttemptConnection();

                if ((loggedIn = loginEvent.WaitOne(Timeout * 1000)) == false) {
                    OnTestEvent(new ProtocolUnitTestEventArgs() { Message = "Timeout on client connection + login. (check end point & credentials)" });
                }

                game.Bubble = originalBubbleProxy;
            }

            return loggedIn;
        }

        public bool Execute(ISandboxProtocolController game, bool connectionIsolation) {
            var success = true;

            if (connectionIsolation == true) {
                // 1. Assert the game is disconnected.
                success = Disconnect(game);
            }

            // 2. Assert the game is connected and logged in, ready to go.
            success = success && LoggedIn(game);

            Start = DateTime.Now;

            if (success == true) {
                OnTestSetup(new ProtocolUnitTestEventArgs() {
                    Message = "Test connection reconnected and logged in."
                });

                // 3. Assert each command is successful in whole from sending a command.
                foreach (var command in TestCommands) {

                    var expectedResults = new AutoResetEvent(false);

                    // List of packets recieved that were not matched.
                    var unmatchedReceived = new List<string>();

                    var localCommand = command;

                    Action<IClientEventArgs> handler = (args) => {
                        if (args.EventType == ClientEventType.ClientPacketReceived) {
                            var matchedPacket = args.Now.Packets.First().Type == PacketType.Response ? localCommand.Responses.FirstOrDefault(response => response.Matches(args.Now.Packets.First().Text)) : localCommand.Requests.FirstOrDefault(request => request.Matches(args.Now.Packets.First().Text));

                            if (matchedPacket != null) {
                                matchedPacket.Found = true;
                            }
                            else {
                                // Add it to the alternative packets received for debug output.
                                unmatchedReceived.Add(args.Now.Packets.First().Text);
                            }

                            // If we don't have any packets remaining that have not been found yet.
                            if (localCommand.Responses.Count(response => response.Found == false) + localCommand.Requests.Count(request => request.Found == false) == 0) {
                                expectedResults.Set();
                            }
                        }
                    };

                    var originalBubbleProxy = game.Bubble;

                    game.Bubble = new SandboxProtocolCallbackProxy() {
                        ClientEvent = new Action<IClientEventArgs>(handler)
                    };

                    // 3 b. Send our packet to initiate this command test
                    game.Action(new NetworkAction() {
                        ActionType = NetworkActionType.NetworkPacketSend,
                        Now = {
                            Content = new List<string>() {
                                command.Send.Text
                            }
                        }
                    });

                    if ((success = success && expectedResults.WaitOne(Timeout * 1000)) == false) {
                        var expecting = command.Responses.Where(response => response.Found == false).Select(response => response.ToString()).Union(command.Requests.Where(request => request.Found == false).Select(request => request.ToString())).ToArray();

                        OnTestEvent(new ProtocolUnitTestEventArgs() { Message = string.Format("Expecting: {0}; Received: {1}", string.Join(", ", expecting), string.Join(", ", unmatchedReceived.ToArray())) });
                    }

                    game.Bubble = originalBubbleProxy;
                }
            }
            End = DateTime.Now;

            return success;
        }

        public override string ToString() {
            return Name;
        }

        public void Dispose() {
            Name = null;
            TestCommands.ForEach(e => e.Dispose());
        }
    }
}
