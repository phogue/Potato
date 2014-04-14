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
using System.Net;
using Procon.Core.Shared;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;
using Procon.Net.Protocols.CommandServer;
using Procon.Net.Shared;

namespace Procon.Core.Remote {
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

        public SharedReferences Shared { get; private set; }

        /// <summary>
        /// Initalizes the command server controller with default values
        /// </summary>
        public CommandServerController() : base() {
            this.Shared = new SharedReferences();
            this.Certificate = new CertificateController();
        }

        public override ICoreController Execute() {
            this.Shared.Variables.Variable(CommonVariableNames.CommandServerEnabled).PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(OnPropertyChanged);
            this.Shared.Variables.Variable(CommonVariableNames.CommandServerPort).PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(OnPropertyChanged);
            this.Shared.Variables.Variable(CommonVariableNames.CommandServerCertificatePath).PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(OnPropertyChanged);

            // Copy for unit testing purposes if variables is modified.
            this.Certificate.Shared.Variables = this.Shared.Variables;
            this.Certificate.Execute();

            this.Configure();

            return base.Execute();
        }

        public override void Dispose() {
            base.Dispose();

            this.Shared.Variables.Variable(CommonVariableNames.CommandServerEnabled).PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(OnPropertyChanged);
            this.Shared.Variables.Variable(CommonVariableNames.CommandServerPort).PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(OnPropertyChanged);
            this.Shared.Variables.Variable(CommonVariableNames.CommandServerCertificatePath).PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(OnPropertyChanged);
            
            if (this.CommandServerListener != null) this.CommandServerListener.Dispose();
            this.CommandServerListener = null;
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            this.Configure();
        }

        /// <summary>
        /// Pokes the underlying listener, ensuring that all clients held in memory are still
        /// active and not disconnected.
        /// </summary>
        public override void Poke() {
            // Method implemented here instead of calling the public method so we can do
            // additional work during a Poke in the future.

            if (this.CommandServerListener != null) {
                this.CommandServerListener.Poke();
            }
        }

        /// <summary>
        /// Configures the command server, listening for any changes to the configuration and altering
        /// accordingly.
        /// 
        /// We should fetch and listen for changes to the CommandServer* variables
        /// </summary>
        public void Configure() {
            if (this.Shared.Variables.Get<bool>(CommonVariableNames.CommandServerEnabled) == true) {
                if (this.Certificate.Certificate != null) {
                    this.CommandServerListener = new CommandServerListener {
                        Certificate = this.Certificate.Certificate,
                        Port = this.Shared.Variables.Get<int>(CommonVariableNames.CommandServerPort),
                        PacketReceived = OnPacketReceived,
                        BeginException = OnBeginException,
                        ListenerException = OnListenerException
                    };

                    // Start accepting connections.
                    if (this.CommandServerListener.BeginListener() == true) {
                        this.Shared.Events.Log(new GenericEvent() {
                            GenericEventType = GenericEventType.CommandServerStarted,
                            Success = true,
                            CommandResultType = CommandResultType.Success
                        });
                    }
                    else {
                        this.Shared.Events.Log(new GenericEvent() {
                            GenericEventType = GenericEventType.CommandServerStarted,
                            Success = false,
                            CommandResultType = CommandResultType.Failed
                        });
                    }
                }
            }
            else if (this.CommandServerListener != null) {
                this.CommandServerListener.Dispose();
                this.CommandServerListener = null;

                this.Shared.Events.Log(new GenericEvent() {
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
            this.Shared.Events.Log(new GenericEvent() {
                GenericEventType = GenericEventType.CommandServerStopped,
                Success = false,
                CommandResultType = CommandResultType.Failed
            });

            // Log the exception for debugging purposes.
            Procon.Service.Shared.ServiceControllerHelpers.LogUnhandledException("CommandServerController.OnBeginException", exception);
        }

        /// <summary>
        /// Called when an exception is caught while a client is connecting, after the listener has been disposed.
        /// </summary>
        /// <param name="exception"></param>
        public void OnListenerException(Exception exception) {
            this.Shared.Events.Log(new GenericEvent() {
                GenericEventType = GenericEventType.CommandServerStopped,
                Success = false,
                CommandResultType = CommandResultType.Failed
            });

            // Log the exception for debugging purposes.
            Procon.Service.Shared.ServiceControllerHelpers.LogUnhandledException("CommandServerController.OnListenerException", exception);

            // Now start the listener back up. It was running successfully, but a problem occured
            // while processing a client. The error has been logged 
            this.Configure();
        }

        /// <summary>
        /// Pulls the identifer out of the request (endpoint address)
        /// </summary>
        /// <param name="request">The request received from the client</param>
        /// <returns>The extracted identifer or an empty string if none exists</returns>
        protected String ExtractIdentifer(CommandServerPacket request) {
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

            if (String.IsNullOrEmpty(command.Authentication.Username) == false && String.IsNullOrEmpty(command.Authentication.PasswordPlainText) == false) {
                result = this.Shared.Security.Tunnel(CommandBuilder.SecurityAccountAuthenticate(command.Authentication.Username, command.Authentication.PasswordPlainText, this.ExtractIdentifer(request)).SetOrigin(CommandOrigin.Remote));
            }
            else if (command.Authentication.TokenId != Guid.Empty && String.IsNullOrEmpty(command.Authentication.Token) == false) {
                result = this.Shared.Security.Tunnel(CommandBuilder.SecurityAccountAuthenticateToken(command.Authentication.TokenId, command.Authentication.Token, this.ExtractIdentifer(request)).SetOrigin(CommandOrigin.Remote));
            }

            return result;
        }

        /// <summary>
        /// Called when a packet is recieved from the listening command server.
        /// </summary>
        /// <param name="client">The client to send back the response</param>
        /// <param name="request">The request packet recieved</param>
        public void OnPacketReceived(IClient client, CommandServerPacket request) {
            CommandServerPacket response = new CommandServerPacket() {
                Packet = {
                    Type = PacketType.Response,
                    Origin = PacketOrigin.Client,
                },
                ProtocolVersion = request.ProtocolVersion,
                Method = request.Method,
                StatusCode = HttpStatusCode.NotFound,
                Headers = new WebHeaderCollection() {
                    { HttpRequestHeader.Connection, "close" }
                }
            };
            
            ICommand command = CommandServerSerializer.DeserializeCommand(request);

            if (command != null) {
                ICommandResult authentication = this.Authenticate(request, command);

                if (authentication.Success == true) {
                    // If all they wanted to do was check the authentication..
                    if (String.CompareOrdinal(command.Name, CommandType.SecurityAccountAuthenticate.ToString()) == 0) {
                        // Success
                        response = CommandServerSerializer.CompleteResponsePacket(CommandServerSerializer.ResponseContentType(command), response, authentication);
                    }
                    else {
                        // Propagate their command
                        ICommandResult result = this.Tunnel(command);

                        response = CommandServerSerializer.CompleteResponsePacket(CommandServerSerializer.ResponseContentType(command), response, result);
                    }
                }
                else {
                    // They are not authorized to login or issue this command.
                    response = CommandServerSerializer.CompleteResponsePacket(CommandServerSerializer.ResponseContentType(command), response, new CommandResult() {
                        Success = false,
                        CommandResultType = CommandResultType.InsufficientPermissions,
                        Message = "Invalid username or password"
                    });
                }
                /*
                if (this.Shared.Security.Tunnel(CommandBuilder.SecurityAccountAuthenticate(command.Authentication.Username, command.Authentication.PasswordPlainText, this.ExtractIdentifer(request)).SetOrigin(CommandOrigin.Remote)).Success == true) {
                    // Now dispatch the command
                    ICommandResult result = this.Tunnel(command);

                    response = CommandServerSerializer.CompleteResponsePacket(CommandServerSerializer.ResponseContentType(command), response, result);
                }
                else if (this.Shared.Security.Tunnel(CommandBuilder.SecurityAccountAuthenticateToken(command.Authentication.TokenId, command.Authentication.Token, this.ExtractIdentifer(request)).SetOrigin(CommandOrigin.Remote)).Success == true) {
                    // Now dispatch the command
                    ICommandResult result = this.Tunnel(command);

                    response = CommandServerSerializer.CompleteResponsePacket(CommandServerSerializer.ResponseContentType(command), response, result);
                }
                else {
                    // They are not authorized to login or issue this command.
                    response = CommandServerSerializer.CompleteResponsePacket(CommandServerSerializer.ResponseContentType(command), response, new CommandResult() {
                        Success = false,
                        CommandResultType = CommandResultType.InsufficientPermissions,
                        Message = "Invalid username or password"
                    });
                }
                */
            }
            else {
                // Something wrong during deserialization, issue a bad request.
                response.StatusCode = HttpStatusCode.BadRequest;
            }

            this.CommandServerListener.Respond(client, request, response);
        }
    }
}
