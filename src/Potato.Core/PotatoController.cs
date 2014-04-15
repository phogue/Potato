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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Potato.Core.Connections;
using Potato.Core.Database;
using Potato.Core.Events;
using Potato.Core.Packages;
using Potato.Core.Protocols;
using Potato.Core.Remote;
using Potato.Core.Shared;
using Potato.Core.Shared.Events;
using Potato.Core.Shared.Models;
using Potato.Net.Shared;
using Potato.Net.Shared.Utils;
using Potato.Service.Shared;

namespace Potato.Core {
    /// <summary>
    /// The core controller of Potato, an instance of Potato.
    /// </summary>
    public class PotatoController : CoreController, ISharedReferenceAccess, IService {
        /// <summary>
        /// List of game connections
        /// </summary>
        // todo Convert to ICoreController
        public List<IConnectionController> Connections { get; protected set; }

        /// <summary>
        /// The packages that are intalled or can be installed.
        /// </summary>
        public ICoreController Packages { get; protected set; }

        /// <summary>
        /// The command server controller, if active.
        /// </summary>
        public ICoreController CommandServer { get; protected set; }

        /// <summary>
        /// The database controller for interacting with databases.
        /// </summary>
        public ICoreController Database { get; protected set; }

        /// <summary>
        /// Controller to push events to various sources.
        /// </summary>
        public ICoreController PushEvents { get; protected set; }

        /// <summary>
        /// Controller to load library of supported protocols
        /// </summary>
        public ICoreController Protocols { get; protected set; }

        /// <summary>
        /// Tasks to be run by this connection. Primarily used to reattempt connections
        /// or force a synchronization of the game data.
        /// </summary>
        public List<Timer> Tasks { get; set; } 

        /// <summary>
        /// Output to the console for all events processed by this instance.
        /// </summary>
        public ICoreController EventsConsole { get; protected set; }

        /// <summary>
        /// The latest service message to post back to the service controller. If no message exists then a "ok" response will be sent.
        /// </summary>
        public ServiceMessage ServiceMessage { get; set; }

        public SharedReferences Shared { get; private set; }

        /// <summary>
        /// Creates a new instance of Potato, setting up command server, packages and tasks
        /// </summary>
        public PotatoController() : base() {
            this.Shared = new SharedReferences();

            this.Connections = new List<IConnectionController>();

            this.Packages = new PackagesController() {
                BubbleObjects = new List<ICoreController>() {
                    this
                }
            };

            this.CommandServer = new CommandServerController() {
                TunnelObjects = new List<ICoreController>() {
                    this
                }
            };

            this.Database = new DatabaseController() {
                BubbleObjects = new List<ICoreController>() {
                    this
                }
            };

            this.PushEvents = new PushEventsController() {
                BubbleObjects = new List<ICoreController>() {
                    this
                }
            };

            this.Protocols = new ProtocolController() {
                BubbleObjects = new List<ICoreController>() {
                    this
                }
            };

            this.Tasks = new List<Timer>() {
                new Timer(Connection_Tick, this, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(15)),
                new Timer(Events_Tick, this, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60)),
                new Timer(CommandServer_Tick, this, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60)),
                new Timer(Packages_Tick, this, TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(60)),
                new Timer(Security_Tick, this, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60))
            };

            this.EventsConsole = new EventsConsoleController();

            this.CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    CommandType = CommandType.PotatoServiceMergePackage,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "uri",
                            Type = typeof(String)
                        },
                        new CommandParameterType() {
                            Name = "packageId",
                            Type = typeof(String)
                        }
                    },
                    Handler = this.PotatoServiceMergePackage
                },
                new CommandDispatch() {
                    CommandType = CommandType.PotatoServiceUninstallPackage,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "packageId",
                            Type = typeof(String)
                        }
                    },
                    Handler = this.PotatoServiceUninstallPackage
                },
                new CommandDispatch() {
                    CommandType = CommandType.PotatoServiceRestart,
                    Handler = this.PotatoServiceRestart
                },
                new CommandDispatch() {
                    CommandType = CommandType.PotatoAddConnection,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "gameTypeProvider",
                            Type = typeof(String)
                        },
                        new CommandParameterType() {
                            Name = "gameTypeType",
                            Type = typeof(String)
                        },
                        new CommandParameterType() {
                            Name = "hostName",
                            Type = typeof(String)
                        },
                        new CommandParameterType() {
                            Name = "port",
                            Type = typeof(UInt16)
                        },
                        new CommandParameterType() {
                            Name = "password",
                            Type = typeof(String)
                        },
                        new CommandParameterType() {
                            Name = "additional",
                            Type = typeof(String)
                        }
                    },
                    Handler = this.PotatoAddConnection
                },
                new CommandDispatch() {
                    CommandType = CommandType.PotatoRemoveConnection,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "connectionGuid",
                            Type = typeof(String)
                        }
                    },
                    Handler = this.PotatoRemoveConnectionByGuid
                },
                new CommandDispatch() {
                    CommandType = CommandType.PotatoRemoveConnection,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "gameTypeProvider",
                            Type = typeof(String)
                        },
                        new CommandParameterType() {
                            Name = "gameTypeType",
                            Type = typeof(String)
                        },
                        new CommandParameterType() {
                            Name = "hostName",
                            Type = typeof(String)
                        },
                        new CommandParameterType() {
                            Name = "port",
                            Type = typeof(UInt16)
                        }
                    },
                    Handler = this.PotatoRemoveConnectionByDetails
                },
                new CommandDispatch() {
                    CommandType = CommandType.PotatoQuery,
                    Handler = this.PotatoQuery
                }
            });
        }

        /// <summary>
        /// Fires a message across the appdomain back to the instance controller, provided 
        /// a callback has been setup.
        /// </summary>
        /// <remarks>
        /// If the main thread is tied up then "nop" won't be sent back and the service 
        /// will be shut down. We may expand on this in the future to check extra threads, 
        /// polling all the plugin appdomains and such.
        /// </remarks>
        public ServiceMessage PollService() {
            ServiceMessage message = null;
            
            if (this.ServiceMessage != null) {
                message = this.ServiceMessage;

                // clear it for the next poll.
                this.ServiceMessage = null;
            }
            else {
                message = new ServiceMessage() {
                    Name = "nop"
                };
            }
            
            return message;
        }

        /// <summary>
        /// Executes very simple commands originating from the service controller from a local origin.
        /// </summary>
        public ServiceMessage ExecuteMessage(ServiceMessage message) {
            // A request from the service controller to run a command locally.
            ICommandResult result = this.Tunnel(new Command() {
                Name = message.Name,
                Origin = CommandOrigin.Local,
                // This is possible because of the SortedDictionary used in Potato.Service.Shared.ArgumentsHelper
                // This method will always assume that Arguments.Values will result in the order of execution. 
                Parameters = message.Arguments.Values.Select(value => new CommandParameter() {
                    Data = {
                        Content = new List<String>() {
                            value
                        }
                    }
                }).Cast<ICommandParameter>().ToList()
            });

            // Format and return the service message.
            return new ServiceMessage() {
                Name = "result",
                Arguments = new Dictionary<string, string>() {
                    { "Command", message.Name },
                    { "Success", result.Success.ToString() },
                    { "Status", result.CommandResultType.ToString() },
                    { "Message", result.Message }
                }
            };
        }

        /// <summary>
        /// Reconnects or synchronizes data on each connection. A periodic standard
        /// ten second poke of each connection pretty much.
        /// </summary>
        /// <param name="state"></param>
        private void Connection_Tick(Object state) {
            if (this.Connections != null) {
                lock (this.Connections) {
                    foreach (IConnectionController connection in this.Connections) {
                        connection.Poke();
                    }
                }
            }
        }

        /// <summary>
        /// Writes out the events to file.
        /// </summary>
        /// <param name="state"></param>
        private void Events_Tick(object state) {
            this.Shared.Events.WriteEvents();
        }

        /// <summary>
        /// Pokes the command server and all current active clients, ensuring we don't have any stale clients
        /// still held in memory.
        /// </summary>
        /// <param name="state"></param>
        protected void CommandServer_Tick(Object state) {
            if (this.CommandServer != null) {
                this.CommandServer.Poke();
            }
        }

        /// <summary>
        /// Rebuilds the current repositories cache every 60 minutes
        /// </summary>
        /// <param name="state"></param>
        protected void Packages_Tick(Object state) {
            if (this.Packages != null) {
                // Set the root (non-grouped) repository to Myrcon's official repository, or whatever
                // repository the host has setup.
                this.Shared.Variables.Variable(CommonVariableNames.PackagesRepositoryUri).Value = this.Shared.Variables.Get(CommonVariableNames.PackagesDefaultSourceRepositoryUri, Defines.PackagesDefaultSourceRepositoryUri);

                this.Packages.Poke();
            }
        }

        /// <summary>
        /// Removes all expired access tokens every 60 seconds.
        /// </summary>
        /// <param name="state"></param>
        protected void Security_Tick(Object state) {
            if (this.Shared != null && this.Shared.Security != null) {
                this.Shared.Security.Poke();
            }
        }

        /// <summary>
        /// Assigns event handlers to deal with changes within the class.
        /// Starts the execution of this object's security, packages, and variables.
        /// Loads the configuration file.
        /// </summary>
        /// <returns></returns>
        public override ICoreController Execute() {
            this.EventsConsole.Execute();
            this.Packages.Execute();
            this.CommandServer.Execute();
            this.PushEvents.Execute();
            this.Database.Execute();
            this.Protocols.Execute();

            this.Execute(new Command() {
                Origin = CommandOrigin.Local
            }, new Config().Load(new DirectoryInfo(Defines.ConfigsDirectory.FullName)), this.Shared.Variables.Get<String>(CommonVariableNames.PotatoConfigPassword));
            
            this.Shared.Events.Log(new GenericEvent() {
                GenericEventType = GenericEventType.PotatoServiceStarted,
                Message = @"The service has started successfully."
            });

            return base.Execute();
        }

        /// <summary>
        /// IService implementation to call the execute method and ignore the return type.
        /// </summary>
        public void Start() {
            this.Execute();
        }

        /// <summary>
        /// IService proxy for the variables of this object.
        /// </summary>
        /// <param name="arguments"></param>
        public void ParseCommandLineArguments(List<String> arguments) {
            this.Shared.Variables.ParseArguments(arguments);
        }

        /// <summary>
        /// Writes the current object out to a file.
        /// </summary>
        public void WriteConfig() {
            IConfig config = new Config();
            config.Create<PotatoController>();
            this.WriteConfig(config, this.Shared.Variables.Get<String>(CommonVariableNames.PotatoConfigPassword));

            config.Save(new FileInfo(Path.Combine(Defines.ConfigsDirectory.FullName, this.GetType().Namespace + ".json")));
        }

        /// <summary>
        /// Saves all the connections to the config file.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="password"></param>
        public override void WriteConfig(IConfig config, String password = null) {
            this.Shared.Variables.WriteConfig(config, password);

            this.Shared.Events.WriteConfig(config, password);

            this.Packages.WriteConfig(config, password);

            this.Database.WriteConfig(config, password);

            this.Shared.Languages.WriteConfig(config, password);

            this.Shared.Security.WriteConfig(config, password);

            this.CommandServer.WriteConfig(config, password);
            
            this.PushEvents.WriteConfig(config, password);

            lock (this.Connections) {
                foreach (ConnectionController connection in this.Connections) {
                    // This command is executed in the Potato object.
                    // I had to write this comment because I kept moving it to the actual connection and failing oh so hard.
                    config.Append(new Command() {
                        CommandType = CommandType.PotatoAddConnection,
                        Parameters = new List<ICommandParameter>() {
                            new CommandParameter() {
                                Data = {
                                    Content = new List<String>() {
                                        connection.ConnectionModel.ProtocolType.Provider
                                    }
                                }
                            },
                            new CommandParameter() {
                                Data = {
                                    Content = new List<String>() {
                                        connection.ConnectionModel.ProtocolType.Type
                                    }
                                }
                            },
                            new CommandParameter() {
                                Data = {
                                    Content = new List<String>() {
                                        connection.ConnectionModel.Hostname
                                    }
                                }
                            },
                            new CommandParameter() {
                                Data = {
                                    Content = new List<String>() {
                                        connection.ConnectionModel.Port.ToString(CultureInfo.InvariantCulture)
                                    }
                                }
                            },
                            new CommandParameter() {
                                Data = {
                                    Content = new List<String>() {
                                        connection.ConnectionModel.Password
                                    }
                                }
                            },
                            new CommandParameter() {
                                Data = {
                                    Content = new List<String>() {
                                        connection.ConnectionModel.Arguments
                                    }
                                }
                            }
                        }
                    }.ToConfigCommand().Encrypt(password));

                    connection.WriteConfig(config, password);
                }
            }
        }

        /// <summary>
        /// Reverse the direction of the bubble. If we have not found it by now we need to tunnel down to
        /// everything to find where this command needs to be executed. If tunneling finds nothing then
        /// the command is completed.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public override ICommandResult Bubble(ICommand command) {
            return this.Tunnel(command);
        }

        protected override IList<ICoreController> TunnelExecutableObjects(ICommand command) {
            List<ICoreController> list = new List<ICoreController>();

            if (command.Scope != null && command.Scope.ConnectionGuid != Guid.Empty) {
                // Focus only on the connection.
                this.Connections.Where(connection => connection.ConnectionModel.ConnectionGuid == command.Scope.ConnectionGuid).ToList().ForEach(list.Add);
            }
            else {
                // Add all of the connections.
                this.Connections.ForEach(list.Add);

                list.Add(this.PushEvents);
                list.Add(this.Shared.Security);
                list.Add(this.Shared.Languages);
                list.Add(this.Shared.Variables);
                list.Add(this.Shared.Events);
                list.Add(this.Packages);
                list.Add(this.Database);
                list.Add(this.Protocols);
            }
            
            return list;
        }

        /// <summary>
        /// Shuts down the layer.
        /// Disposes of this object's security, packages, and variables.
        /// Calls the base dispose.
        /// </summary>
        public override void Dispose() {

            // 1. Stop all tasks from ticking, removing concurrent executions with this object.
            this.Tasks.ForEach(task => task.Dispose());
            this.Tasks.Clear();
            this.Tasks = null;

            // 2. Events contains varies references to other data through out Potato. This 
            // data should be written out first, writing out the full events data then 
            // moving on to disposing the actual data.
            this.Shared.Events.Dispose();
            this.Shared.Events = null;

            // 3. Stop/disconnect all game server connections, unload any plugins etc.
            lock (this.Connections) {
                foreach (ConnectionController connection in this.Connections) {
                    connection.Dispose();
                }
            }

            this.Connections.Clear();
            this.Connections = null;

            // @todo Does the order matter here? If so, document why.

            this.Shared.Security.Dispose();
            this.Shared.Security = null;

            this.Shared.Languages.Dispose();
            this.Shared.Languages = null;

            this.Packages.Dispose();
            this.Packages = null;

            this.Database.Dispose();
            this.Database = null;

            this.CommandServer.Dispose();
            this.CommandServer = null;

            this.PushEvents.Dispose();
            this.PushEvents = null;

            this.Protocols.Dispose();
            this.Protocols = null;

            this.Shared.Variables.Dispose();
            this.Shared.Variables = null;

            this.EventsConsole.Dispose();
            this.EventsConsole = null;

            base.Dispose();
        }

        /// <summary>
        /// Posts a restart signal for the service controller to poll.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult PotatoServiceRestart(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            // As long as the current account is allowed to execute this command...
            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                this.ServiceMessage = new ServiceMessage() {
                    Name = "restart"
                };

                result = new CommandResult() {
                    Message = String.Format("Successfully posted restart signal."),
                    CommandResultType = CommandResultType.Success,
                    Success = true
                };

                this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.PotatoServiceRestarting));
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Posts a merge signal for the service controller to poll.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult PotatoServiceMergePackage(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            String uri = parameters["uri"].First<String>();
            String packageId = parameters["packageId"].First<String>();

            // As long as the current account is allowed to execute this command...
            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (String.IsNullOrEmpty(uri) == false && String.IsNullOrEmpty(packageId) == false) {
                    this.ServiceMessage = new ServiceMessage() {
                        Name = "merge",
                        Arguments = new Dictionary<String, String>() {
                            { "uri", uri },
                            { "packageid", packageId }
                        }
                    };

                    result = new CommandResult() {
                        Message = String.Format("Successfully posted merge signal."),
                        CommandResultType = CommandResultType.Success,
                        Success = true
                    };

                    this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.PotatoServiceMergePackage));
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Invalid or missing parameter ""uri"" or ""packageId""."),
                        CommandResultType = CommandResultType.InvalidParameter,
                        Success = false
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Posts a uninstall signal for the service controller to poll.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult PotatoServiceUninstallPackage(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            String packageId = parameters["packageId"].First<String>();

            // As long as the current account is allowed to execute this command...
            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (String.IsNullOrEmpty(packageId) == false) {
                    this.ServiceMessage = new ServiceMessage() {
                        Name = "uninstall",
                        Arguments = new Dictionary<String, String>() {
                            { "packageid", packageId }
                        }
                    };

                    result = new CommandResult() {
                        Message = String.Format("Successfully posted uninstall signal."),
                        CommandResultType = CommandResultType.Success,
                        Success = true
                    };

                    this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.PotatoServiceUninstallPackage));
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Invalid or missing parameter ""packageId""."),
                        CommandResultType = CommandResultType.InvalidParameter,
                        Success = false
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Add a connection to this instance.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public ICommandResult PotatoAddConnection(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;
            
            String protocolTypeProvider = parameters["gameTypeProvider"].First<String>() ?? "";
            String protocolTypeType = parameters["gameTypeType"].First<String>() ?? "";
            String hostName = parameters["hostName"].First<String>() ?? "";
            UInt16 port = parameters["port"].First<UInt16>();
            String password = parameters["password"].First<String>() ?? "";
            String additional = parameters["additional"].First<String>() ?? "";

            // As long as the current account is allowed to execute this command...
            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                // As long as we have less than the maximum amount of connections...
                if (this.Connections.Count < this.Shared.Variables.Get(CommonVariableNames.MaximumProtocolConnections, 9000)) {
                    // As long as the connection for that specific game, hostname, and port does not exist...
                    if (this.Connections.FirstOrDefault(c => c.ConnectionModel.ProtocolType.Type == protocolTypeType && c.ConnectionModel.Hostname == hostName && c.ConnectionModel.Port == port) == null) {
                        // As long as the game type is defined...

                        var supportCheckResult = this.Protocols.Tunnel(CommandBuilder.ProtocolsCheckSupportedProtocol(protocolTypeProvider, protocolTypeType).SetOrigin(CommandOrigin.Local));

                        if (supportCheckResult.Success == true) {
                            IProtocolAssemblyMetadata meta = supportCheckResult.Now.ProtocolAssemblyMetadatas.First();
                            
                            ConnectionController connection = new ConnectionController() {
                                Potato = this
                            };

                            connection.SetupProtocol(supportCheckResult.Now.ProtocolAssemblyMetadatas.First(), supportCheckResult.Now.ProtocolTypes.First(), new ProtocolSetup() {
                                Hostname = hostName,
                                Port = port,
                                Password = password,
                                Arguments = ArgumentHelper.ToArguments(additional.Wordify()),
                                ConfigDirectory = meta.Directory.GetDirectories(Defines.ProtocolsDirectoryName, SearchOption.AllDirectories).Select(directory => directory.FullName).FirstOrDefault()
                            });

                            lock (this.Connections) {
                                this.Connections.Add(connection);
                            }

                            connection.Execute();
                            connection.AttemptConnection();

                            result = new CommandResult() {
                                Message = String.Format("Successfully added {0} connection.", protocolTypeType),
                                CommandResultType = CommandResultType.Success,
                                Success = true,
                                Now = {
                                    Connections = new List<ConnectionModel>() {
                                        connection.ConnectionModel
                                    }
                                }
                            };

                            this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.PotatoConnectionAdded));
                        }
                        else {
                            result = new CommandResult() {
                                Message = String.Format(@"Protocol type ""{0}"" is not supported.", protocolTypeType),
                                CommandResultType = CommandResultType.DoesNotExists,
                                Success = false
                            };
                        }
                    }
                    else {
                        result = new CommandResult() {
                            Message = String.Format(@"Game type ""{0}"" with connection to {1}:{2} has already been added.", protocolTypeType, hostName, port),
                            CommandResultType = CommandResultType.AlreadyExists,
                            Success = false
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Maximum number of game connections exceeded."),
                        CommandResultType = CommandResultType.LimitExceeded,
                        Success = false
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        protected ICommandResult PotatoRemoveConnection(ICommand command, IConnectionController connection) {
            ICommandResult result = null;

            // As long as the current account is allowed to execute this command...
            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {

                // As long as the connection for that specific game, hostname, and port exists...
                if (connection != null) {
                    lock (this.Connections) {
                        this.Connections.Remove(connection);
                    }

                    result = new CommandResult() {
                        Message = String.Format(@"Successfully removed connection with connection to {0}:{1} and game type ""{2}"".", connection.ConnectionModel.Hostname, connection.ConnectionModel.Port, connection),
                        CommandResultType = CommandResultType.Success,
                        Success = true,
                        Now = {
                            Connections = new List<ConnectionModel>() {
                                connection.ConnectionModel
                            }
                        }
                    };

                    this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.PotatoConnectionRemoved));

                    connection.Dispose();
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Connection does not exist."),
                        CommandResultType = CommandResultType.DoesNotExists,
                        Success = false
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Proxy to the expanded remove connction method, but only accepts a connectionGuid.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult PotatoRemoveConnectionByGuid(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            String connectionGuid = parameters["connectionGuid"].First<String>();

            IConnectionController connection = this.Connections.FirstOrDefault(x => String.Compare(x.ConnectionModel.ConnectionGuid.ToString(), connectionGuid, StringComparison.OrdinalIgnoreCase) == 0);

            return this.PotatoRemoveConnection(command, connection);
        }

        /// <summary>
        /// Removes a connection from this instance.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public ICommandResult PotatoRemoveConnectionByDetails(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            String gameTypeProvider = parameters["gameTypeProvider"].First<String>();
            String gameTypeType = parameters["gameTypeType"].First<String>();
            String hostName = parameters["hostName"].First<String>();
            UInt16 port = parameters["port"].First<UInt16>();

            IConnectionController connection = this.Connections.FirstOrDefault(c =>
                c.ConnectionModel.ProtocolType.Provider == gameTypeProvider &&
                c.ConnectionModel.ProtocolType.Type == gameTypeType &&
                c.ConnectionModel.Hostname == hostName &&
                c.ConnectionModel.Port == port
            );

            return this.PotatoRemoveConnection(command, connection);
        }

        /// <summary>
        /// Queries this instance for a snapshot as it exists now
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult PotatoQuery(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;
            
            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                ICommandResult packages = this.Packages.Tunnel(new Command(command) {
                    CommandType = CommandType.PackagesFetchPackages
                });

                ICommandResult protocols = this.Protocols.Tunnel(new Command(command) {
                    CommandType = CommandType.ProtocolsFetchSupportedProtocols
                });
                
                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success,
                    Now = new CommandData() {
                        Connections = this.Connections.Select(connection => connection.ConnectionModel).ToList(),
                        ProtocolTypes = new List<ProtocolType>(protocols.Now.ProtocolTypes ?? new List<ProtocolType>()),
                        Repositories = new List<RepositoryModel>(packages.Now.Repositories ?? new List<RepositoryModel>()),
                        Groups = new List<Core.Shared.Models.GroupModel>(this.Shared.Security.Groups),
                        Languages = this.Shared.Languages.LoadedLanguageFiles.Select(language => language.LanguageModel).ToList(),
                        Variables = new List<VariableModel>(this.Shared.Variables.VolatileVariables)
                    }
                };
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }
    }
}
