using System.Net;
using Procon.Core.Shared;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;
using Procon.Net.Protocols.CommandServer;
using Procon.Net.Shared;
using Procon.Net.Shared.Protocols.CommandServer;

namespace Procon.Core.Remote {
    /// <summary>
    /// Listens for incoming connections, authenticates and dispatches commands
    /// </summary>
    public class CommandServerController : CoreController, ISharedReferenceAccess, ICommandServerController {
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
        public void Poke() {
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
                    this.CommandServerListener = new CommandServerListener() {
                        Certificate = this.Certificate.Certificate,
                        Port = this.Shared.Variables.Get<int>(CommonVariableNames.CommandServerPort)
                    };

                    // Assign events.
                    this.CommandServerListener.PacketReceived += OnPacketReceived;

                    // Start accepting connections.
                    this.CommandServerListener.BeginListener();

                    this.Shared.Events.Log(new GenericEventArgs() {
                        GenericEventType = GenericEventType.CommandServerStarted,
                        Success = true,
                        Status = CommandResultType.Success
                    });
                }
            }
            else if (this.CommandServerListener != null) {
                this.CommandServerListener.Dispose();
                this.CommandServerListener = null;

                this.Shared.Events.Log(new GenericEventArgs() {
                    GenericEventType = GenericEventType.CommandServerStopped,
                    Success = true,
                    Status = CommandResultType.Success
                });
            }
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

            Command command = CommandServerSerializer.DeserializeCommand(request);

            if (command != null) {
                if (this.Shared.Security.Tunnel(CommandBuilder.SecurityAccountAuthenticate(command.Username, command.PasswordPlainText).SetOrigin(CommandOrigin.Remote)).Success == true) {
                    // Now dispatch the command
                    CommandResultArgs result = this.Tunnel(command);

                    response = CommandServerSerializer.CompleteResponsePacket(CommandServerSerializer.ResponseContentType(command), response, result);
                }
                else {
                    // They are not authorized to login or issue this command.
                    response = CommandServerSerializer.CompleteResponsePacket(CommandServerSerializer.ResponseContentType(command), response, new CommandResultArgs() {
                        Success = false,
                        Status = CommandResultType.InsufficientPermissions,
                        Message = "Invalid username or password"
                    });
                }
            }
            else {
                // Something wrong during deserialization, issue a bad request.
                response.StatusCode = HttpStatusCode.BadRequest;
            }

            this.CommandServerListener.Respond(client, request, response);
        }
    }
}
