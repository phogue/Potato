using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Newtonsoft.Json;
using Procon.Core.Variables;
using Procon.Net;
using Procon.Net.Utils;
using Procon.Net.Utils.HTTP;

namespace Procon.Core.Remote {
    using Procon.Net.Protocols.Daemon;

    public class DaemonController : Executable {

        /// <summary>
        /// The owner of this daemon controller
        /// </summary>
        public List<IExecutableBase> ExecutableObjects { get; set; }

        /// <summary>
        /// The client to send/recv remote daemon messages.
        /// </summary>
        public DaemonListener DaemonListener { get; set; }

        public DaemonController() {
            this.ExecutableObjects = new List<IExecutableBase>();
        }

        public override ExecutableBase Execute() {
            this.Variables.Variable(CommonVariableNames.DaemonEnabled).PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(DaemonController_PropertyChanged);
            this.Variables.Variable(CommonVariableNames.DaemonListenerPort).PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(DaemonController_PropertyChanged);

            this.ConfigureDaemon();

            return base.Execute();
        }

        public override void Dispose() {
            base.Dispose();

            this.Variables.Variable(CommonVariableNames.DaemonEnabled).PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(DaemonController_PropertyChanged);
            this.Variables.Variable(CommonVariableNames.DaemonListenerPort).PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(DaemonController_PropertyChanged);

            this.ExecutableObjects.Clear();
            this.ExecutableObjects = null;

            if (this.DaemonListener != null) this.DaemonListener.Dispose();
            this.DaemonListener = null;
        }

        protected override IList<IExecutableBase> TunnelExecutableObjects(Command command) {
            return this.ExecutableObjects;
        }

        private void DaemonController_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            this.ConfigureDaemon();
        }

        /// <summary>
        /// Pokes the underlying listener, ensuring that all clients held in memory are still
        /// active and not disconnected.
        /// </summary>
        public void Poke() {
            // Method implemented here instead of calling the public method so we can do
            // additional work during a Poke in the future.

            if (this.DaemonListener != null) {
                this.DaemonListener.Poke();
            }
        }

        /// <summary>
        /// Configures the daemon, listening for any changes to the configuration and altering
        /// accordingly.
        /// 
        /// We should fetch and listen for changes to the Daemon* variables
        /// </summary>
        protected void ConfigureDaemon() {
            if (this.Variables.Get<bool>(CommonVariableNames.DaemonEnabled) == true) {
                this.DaemonListener = new DaemonListener() {
                    Port = this.Variables.Get<int>(CommonVariableNames.DaemonListenerPort)
                };

                // Assign events.
                this.DaemonListener.PacketReceived += new Net.Protocols.Daemon.DaemonListener.PacketReceivedHandler(DaemonListener_PacketReceived);

                // Start accepting connections.
                this.DaemonListener.BeginListener();
            }
            else if (this.DaemonListener != null) {
                this.DaemonListener.Dispose();
                this.DaemonListener = null;

                // Disable listener
                // Dispose listener
            }
        }

        /// <summary>
        /// Takes in a request and does it's best to convert it to a command within Procon for execution.
        /// </summary>
        /// <param name="request">The http request for this command</param>
        /// <returns>The deserialized command or null if an error occured during deserialization</returns>
        protected Command DeserializeCommand(DaemonPacket request) {
            Command command = null;

            try {
                switch (request.Headers[HttpRequestHeader.ContentType].ToLower()) {
                    case Mime.ApplicationJson:
                        command = JsonConvert.DeserializeObject<Command>(request.Content);

                        // Now we convert all remaining array/containers from the deserialization into basic list of strings
                        // We do this because JsonConvert.DeserializeObject has no reference that we in fact
                        // want basic lists of strings

                        // Should I look into custom converters here? I'd be concerned about using them as the type I want
                        // to convert to is "Object", so it might just be a catch all anyway?
                        //for (int offset = 0; command.Parameters != null && offset < command.Parameters.Count; offset++) {
                        //    if (command.Parameters[offset] is Newtonsoft.Json.Linq.JToken) {
                        //        command.Parameters[offset] = (command.Parameters[offset] as Newtonsoft.Json.Linq.JToken).ToObject<List<String>>();
                        //    }
                        //}

                        break;
                    default:
                        XDocument xCommand = XDocument.Parse(request.Content);
                        command = xCommand.Root.FromXElement<Command>();

                        break;
                }

                if (command != null) {
                    command.Origin = CommandOrigin.Remote;
                    command.RemoteRequest = request;
                }
            }
            catch {
                command = null;
            }

            return command;
        }

        /// <summary>
        /// Serializes the result of the issued command back into the http response for the user.
        /// </summary>
        /// <param name="response">The existing response packet to be modified with additional data/changes</param>
        /// <param name="command">The command that has been dispatched for execution</param>
        /// <param name="result">The result of the command (also found in the command object, but placed here for readability)</param>
        /// <returns>The existing response packet, modified with the result of the command execution.</returns>
        protected DaemonPacket SerializeResponse(DaemonPacket response, Command command, CommandResultArgs result) {

            String responseContentType = result.ContentType;

            // If no response type was specified elsewhere
            if (String.IsNullOrEmpty(responseContentType) == true) {
                // Then assume they want whatever data serialized and returned in whatever the request format was.
                responseContentType = command.RemoteRequest.Headers[HttpRequestHeader.ContentType].ToLower();
            }

            if (responseContentType == Mime.TextHtml && result.Now.Content != null && result.Now.Content.Count > 0) {
                response.Headers.Add(HttpRequestHeader.ContentType, Mime.TextHtml);
                response.Content = result.Now.Content.First();
                response.StatusCode = HttpStatusCode.OK;
            }
            else if (responseContentType == Mime.ApplicationJson) {
                response.Headers.Add(HttpRequestHeader.ContentType, Mime.ApplicationJson);

                response.Content = JsonConvert.SerializeObject(result, new JsonSerializerSettings() {
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                response.StatusCode = HttpStatusCode.OK;
            }
            // Else, nothing or some other odd format. Return xml until we can support this file type.
            else {
                // Nothing specified (or xml specified), return xml by default.
                response.Headers.Add(HttpRequestHeader.ContentType, Mime.ApplicationXml);

                XDocument document = new XDocument();
                document.Add(result.ToXElement());

                // Standalone = yes for now. We should remove all external dtd's
                // as we're using xml as a way of serialization, but not validating against
                // anything. Perhaps we should?
                response.Content = new XDeclaration("1.0", "utf-8", "yes").ToString() + document;
                response.StatusCode = HttpStatusCode.OK;
            }

            return response;
        }

        protected void DaemonListener_PacketReceived(IClient client, DaemonPacket request) {
            DaemonPacket response = new DaemonPacket() {
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

            Command command = this.DeserializeCommand(request);

            if (command != null) {
                // If the command was generated successfully with no parsing errors.
                if (response.StatusCode == HttpStatusCode.NotFound) {
                    if (this.Authenticate(command.Username, command.PasswordPlainText) == true) {
                        // Now dispatch the command
                        CommandResultArgs result = this.Tunnel(command);

                        response = this.SerializeResponse(response, command, result);
                    }
                    else {
                        // They are not authorized to login or issue this command.
                        response = this.SerializeResponse(response, command, new CommandResultArgs() {
                            Success = false,
                            Status = CommandResultType.InsufficientPermissions,
                            Message = "Invalid username or password"
                        });
                    }
                }
            }
            else {
                // Something wrong during deserialization, issue a bad request.
                response.StatusCode = HttpStatusCode.BadRequest;
            }

            this.DaemonListener.Respond(client, request, response);
        }

        protected bool Authenticate(String username, String passwordPlainText) {
            // @todo while this is passing plain text passwords we restrict it to the security controller
            CommandResultArgs result = this.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Remote,
                Username =  username,
                CommandType = CommandType.SecurityAccountAuthenticate,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                username
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                passwordPlainText
                            }
                        }
                    }
                }
            });

            return result.Success == true && result.Status == CommandResultType.Success;
        }
    }
}
