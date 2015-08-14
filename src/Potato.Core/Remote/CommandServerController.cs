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
using System.Net;
using Potato.Core.Interface;
using Potato.Core.Shared;
using Potato.Core.Shared.Events;
using Potato.Core.Shared.Models;
using Potato.Net.Protocols.CommandServer;
using Potato.Net.Shared;
using Potato.Net.Shared.Utils.HTTP;

namespace Potato.Core.Remote {
    /// <summary>
    /// Listens for incoming connections, authenticates and dispatches commands
    /// </summary>
    public class CommandServerController : CoreController, ISharedReferenceAccess {
        /// <summary>
        /// The client to send/recv remote commands.
        /// </summary>
        public CommandServerListener CommandServerListener { get; set; }

        /// <summary>
        /// The certificate controller for managing the command server certificate.
        /// </summary>
        public ICertificateController Certificate { get; set; }

        /// <summary>
        /// A reference to the shared reference of static variables.
        /// </summary>
        public SharedReferences Shared { get; private set; }

        /// <summary>
        /// The basic interface for a local user to interact with
        /// </summary>
        public ICoreController Interface { get; set; }

        /// <summary>
        /// Initalizes the command server controller with default values
        /// </summary>
        public CommandServerController() : base() {
            Shared = new SharedReferences();
            Certificate = new CertificateController();
            Interface = new InterfaceController();
        }

        public override ICoreController Execute() {
            Shared.Variables.Variable(CommonVariableNames.CommandServerEnabled).PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(OnPropertyChanged);
            Shared.Variables.Variable(CommonVariableNames.CommandServerPort).PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(OnPropertyChanged);
            Shared.Variables.Variable(CommonVariableNames.CommandServerCertificatePath).PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(OnPropertyChanged);

            TunnelObjects.Add(Interface);

            // Copy for unit testing purposes if variables is modified.
            Certificate.Shared.Variables = Shared.Variables;
            Certificate.Execute();

            Interface.Execute();

            Configure();

            return base.Execute();
        }

        public override void Dispose() {
            base.Dispose();

            Shared.Variables.Variable(CommonVariableNames.CommandServerEnabled).PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(OnPropertyChanged);
            Shared.Variables.Variable(CommonVariableNames.CommandServerPort).PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(OnPropertyChanged);
            Shared.Variables.Variable(CommonVariableNames.CommandServerCertificatePath).PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(OnPropertyChanged);
            
            if (CommandServerListener != null) CommandServerListener.Dispose();
            CommandServerListener = null;
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            Configure();
        }

        /// <summary>
        /// Pokes the underlying listener, ensuring that all clients held in memory are still
        /// active and not disconnected.
        /// </summary>
        public override void Poke() {
            // Method implemented here instead of calling the public method so we can do
            // additional work during a Poke in the future.

            if (CommandServerListener != null) {
                CommandServerListener.Poke();
            }
        }

        /// <summary>
        /// Configures the command server, listening for any changes to the configuration and altering
        /// accordingly.
        /// 
        /// We should fetch and listen for changes to the CommandServer* variables
        /// </summary>
        public void Configure() {
            if (Shared.Variables.Get<bool>(CommonVariableNames.CommandServerEnabled) == true) {
                if (Certificate.Certificate != null) {
                    CommandServerListener = new CommandServerListener {
                        Certificate = Certificate.Certificate,
                        Port = Shared.Variables.Get<int>(CommonVariableNames.CommandServerPort),
                        PacketReceived = OnPacketReceived,
                        BeginException = OnBeginException,
                        ListenerException = OnListenerException
                    };

                    // Start accepting connections.
                    if (CommandServerListener.BeginListener() == true) {
                        Shared.Events.Log(new GenericEvent() {
                            GenericEventType = GenericEventType.CommandServerStarted,
                            Success = true,
                            CommandResultType = CommandResultType.Success
                        });
                    }
                    else {
                        Shared.Events.Log(new GenericEvent() {
                            GenericEventType = GenericEventType.CommandServerStarted,
                            Success = false,
                            CommandResultType = CommandResultType.Failed
                        });
                    }
                }
            }
            else if (CommandServerListener != null) {
                CommandServerListener.Dispose();
                CommandServerListener = null;

                Shared.Events.Log(new GenericEvent() {
                    GenericEventType = GenericEventType.CommandServerStopped,
                    Success = true,
                    CommandResultType = CommandResultType.Success
                });
            }
        }

        /// <summary>
        /// Called when an exception is caught while setting up the listener, after the listener has been disposed.
        /// </summary>
        /// <param name="exception"></param>
        public void OnBeginException(Exception exception) {
            Shared.Events.Log(new GenericEvent() {
                GenericEventType = GenericEventType.CommandServerStopped,
                Success = false,
                CommandResultType = CommandResultType.Failed
            });

            // Log the exception for debugging purposes.
            Potato.Service.Shared.ServiceControllerHelpers.LogUnhandledException("CommandServerController.OnBeginException", exception);
        }

        /// <summary>
        /// Called when an exception is caught while a client is connecting, after the listener has been disposed.
        /// </summary>
        /// <param name="exception"></param>
        public void OnListenerException(Exception exception) {
            Shared.Events.Log(new GenericEvent() {
                GenericEventType = GenericEventType.CommandServerStopped,
                Success = false,
                CommandResultType = CommandResultType.Failed
            });

            // Log the exception for debugging purposes.
            Potato.Service.Shared.ServiceControllerHelpers.LogUnhandledException("CommandServerController.OnListenerException", exception);

            // Now start the listener back up. It was running successfully, but a problem occured
            // while processing a client. The error has been logged 
            Configure();
        }

        /// <summary>
        /// Pulls the identifer out of the request (endpoint address)
        /// </summary>
        /// <param name="request">The request received from the client</param>
        /// <returns>The extracted identifer or an empty string if none exists</returns>
        protected string ExtractIdentifer(CommandServerPacket request) {
            var identifer = "";

            if (request != null && request.Packet != null && request.Packet.RemoteEndPoint != null && request.Packet.RemoteEndPoint.Address != null) {
                identifer = request.Packet.RemoteEndPoint.Address.ToString();
            }

            return identifer;
        }

        /// <summary>
        /// Authenticate the command given the request information.
        /// </summary>
        /// <remarks>
        ///     <para>This command only checks if the user is authenticated with our system, not if they can execute the command. This is accomplished while executing the command.</para>
        /// </remarks>
        /// <param name="request">The request information</param>
        /// <param name="command">The command to authenticate</param>
        /// <returns>The result of authentication</returns>
        protected ICommandResult Authenticate(CommandServerPacket request, ICommand command) {
            ICommandResult result = null;

            if (string.IsNullOrEmpty(command.Authentication.Username) == false && string.IsNullOrEmpty(command.Authentication.PasswordPlainText) == false) {
                result = Shared.Security.Tunnel(CommandBuilder.SecurityAccountAuthenticate(command.Authentication.Username, command.Authentication.PasswordPlainText, ExtractIdentifer(request)).SetOrigin(CommandOrigin.Remote));
            }
            else if (command.Authentication.TokenId != Guid.Empty && string.IsNullOrEmpty(command.Authentication.Token) == false) {
                result = Shared.Security.Tunnel(CommandBuilder.SecurityAccountAuthenticateToken(command.Authentication.TokenId, command.Authentication.Token, ExtractIdentifer(request)).SetOrigin(CommandOrigin.Remote));
            }
            else {
                result = new CommandResult() {
                    Success = false,
                    CommandResultType = CommandResultType.Failed,
                    Message = "Invalid username or password"
                };
            }

            return result;
        }

        /// <summary>
        /// Called when a packet is recieved from the listening command server.
        /// </summary>
        /// <param name="client">The client to send back the response</param>
        /// <param name="request">The request packet recieved</param>
        public void OnPacketReceived(IClient client, CommandServerPacket request) {
            var response = new CommandServerPacket() {
                Packet = {
                    Type = PacketType.Response,
                    Origin = PacketOrigin.Client,
                },
                ProtocolVersion = request.ProtocolVersion,
                Method = request.Method,
                StatusCode = HttpStatusCode.NotFound,
                Headers = new WebHeaderCollection() {
                    { HttpResponseHeader.Connection, "close" }
                }
            };
            
            var command = CommandServerSerializer.DeserializeCommand(request);

            if (command != null) {
                var authentication = Authenticate(request, command);

                if (authentication.Success == true) {
                    // If all they wanted to do was check the authentication..
                    if (string.CompareOrdinal(command.Name, CommandType.SecurityAccountAuthenticate.ToString()) == 0 || string.CompareOrdinal(command.Name, CommandType.SecurityAccountAuthenticateToken.ToString()) == 0) {
                        // Success
                        response = CommandServerSerializer.CompleteResponsePacket(CommandServerSerializer.ResponseContentType(command, request), response, authentication);
                    }
                    else if (Method.Equals(request.Method, Method.Get)) {
                        // Propagate their command
                        var result = Interface.Tunnel(command);

                        response = CommandServerSerializer.CompleteResponsePacket(CommandServerSerializer.ResponseContentType(command, request), response, result);
                    }
                    else {
                        // Propagate their command
                        var result = Tunnel(command);

                        response = CommandServerSerializer.CompleteResponsePacket(CommandServerSerializer.ResponseContentType(command, request), response, result);
                    }
                }
                else {
                    // They are not authorized to login or issue this command.
                    response = CommandServerSerializer.CompleteResponsePacket(CommandServerSerializer.ResponseContentType(command, request), response, authentication);

                    response.StatusCode = HttpStatusCode.Unauthorized;

                    // Apply return header
                    response.Headers.Add(HttpResponseHeader.WwwAuthenticate, string.Format(@"Basic realm=""{0}""", request.Request.Host));
                }
            }
            else {
                // Something wrong during deserialization, issue a bad request.
                response.StatusCode = HttpStatusCode.BadRequest;
            }

            CommandServerListener.Respond(client, request, response);
        }
    }
}
