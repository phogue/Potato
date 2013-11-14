using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using Procon.Net.Protocols.Objects;

namespace Procon.Net.Utils.Tests {

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
        public String Name { get; set; }

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
            TestEventHandler handler = TestSetup;
            if (handler != null) {
                handler(this, args);
            } 
        }

        protected virtual void OnTestEvent(ProtocolUnitTestEventArgs args) {
            TestEventHandler handler = this.TestEvent;
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
        protected bool Disconnect(Game game) {
            bool disconnected = true;

            if (game.ConnectionState != ConnectionState.ConnectionDisconnected) {
                AutoResetEvent disconnectEvent = new AutoResetEvent(false);

                Game.ClientEventHandler handler = (sender, args) => {
                    if (args.EventType == ClientEventType.ClientConnectionStateChange) {
                        if (args.ConnectionState == ConnectionState.ConnectionDisconnected) {
                            disconnectEvent.Set();
                        }
                    }
                };

                game.ClientEvent += handler;

                game.Shutdown();
                
                if ((disconnected = disconnectEvent.WaitOne(this.Timeout * 1000)) == false) {
                    this.OnTestEvent(new ProtocolUnitTestEventArgs() { Message = "Timeout on client disconnection." });
                }

                game.ClientEvent -= handler;
            }

            return disconnected;
        }

        /// <summary>
        /// Asserts a connection has been established to the server and has properly logged in to begin testing.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        protected bool LoggedIn(Game game) {
            bool loggedIn = true;

            if (game.ConnectionState != ConnectionState.ConnectionLoggedIn) {
                AutoResetEvent loginEvent = new AutoResetEvent(false);

                Game.ClientEventHandler handler = (sender, args) => {
                    if (args.EventType == ClientEventType.ClientConnectionStateChange) {
                        if (args.ConnectionState == ConnectionState.ConnectionLoggedIn) {
                            loginEvent.Set();
                        }
                    }
                };

                game.ClientEvent += handler;

                game.AttemptConnection();

                if ((loggedIn = loginEvent.WaitOne(this.Timeout * 1000)) == false) {
                    this.OnTestEvent(new ProtocolUnitTestEventArgs() { Message = "Timeout on client connection + login. (check end point & credentials)" });
                }

                game.ClientEvent -= handler;
            }

            return loggedIn;
        }

        public bool Execute(Game game, bool connectionIsolation) {
            bool success = true;

            if (connectionIsolation == true) {
                // 1. Assert the game is disconnected.
                success = this.Disconnect(game);
            }

            // 2. Assert the game is connected and logged in, ready to go.
            success = success && this.LoggedIn(game);

            this.Start = DateTime.Now;

            if (success == true) {
                this.OnTestSetup(new ProtocolUnitTestEventArgs() {
                    Message = "Test connection reconnected and logged in."
                });

                // 3. Assert each command is successful in whole from sending a command.
                foreach (ProtocolUnitTestCommand command in this.TestCommands) {

                    AutoResetEvent expectedResults = new AutoResetEvent(false);

                    // List of packets recieved that were not matched.
                    List<String> unmatchedReceived = new List<String>();

                    ProtocolUnitTestCommand localCommand = command;

                    Game.ClientEventHandler handler = (sender, args) => {
                        if (args.EventType == ClientEventType.ClientPacketReceived) {
                            ProtocolUnitTestPacket matchedPacket = args.Packet.Type == PacketType.Response ? localCommand.Responses.FirstOrDefault(response => response.Matches(args.Packet.ToString())) : localCommand.Requests.FirstOrDefault(request => request.Matches(args.Packet.ToString()));

                            if (matchedPacket != null) {
                                matchedPacket.Found = true;
                            }
                            else {
                                // Add it to the alternative packets received for debug output.
                                unmatchedReceived.Add(args.Packet.ToString());
                            }

                            // If we don't have any packets remaining that have not been found yet.
                            if (localCommand.Responses.Count(response => response.Found == false) + localCommand.Requests.Count(request => request.Found == false) == 0) {
                                expectedResults.Set();
                            }
                        }
                    };

                    game.ClientEvent += handler;

                    // 3 b. Send our packet to initiate this command test
                    game.Action(new Raw() { PacketText = command.Send.Text, ActionType = NetworkActionType.NetworkSend });

                    if ((success = success && expectedResults.WaitOne(this.Timeout * 1000)) == false) {
                        String[] expecting = command.Responses.Where(response => response.Found == false).Select(response => response.ToString()).Union(command.Requests.Where(request => request.Found == false).Select(request => request.ToString())).ToArray();

                        this.OnTestEvent(new ProtocolUnitTestEventArgs() { Message = String.Format("Expecting: {0}; Recieved: {1}", String.Join(", ", expecting), String.Join(", ", unmatchedReceived.ToArray())) });
                    }

                    game.ClientEvent -= handler;
                }
            }
            this.End = DateTime.Now;

            return success;
        }

        public override string ToString() {
            return this.Name;
        }

        public void Dispose() {
            this.Name = null;
            this.TestCommands.ForEach(e => e.Dispose());
        }
    }
}
