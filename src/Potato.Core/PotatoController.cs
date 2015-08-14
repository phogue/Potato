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
        /// A simple time stamp showing how old a Potato is. The time is set in the constructor.
        /// </summary>
        public DateTime InstantiatedStamp { get; set; }

        /// <summary>
        /// Creates a new instance of Potato, setting up command server, packages and tasks
        /// </summary>
        public PotatoController() : base() {
            InstantiatedStamp = DateTime.Now;

            Shared = new SharedReferences();

            Connections = new List<IConnectionController>();

            Packages = new PackagesController() {
                BubbleObjects = new List<ICoreController>() {
                    this
                }
            };

            CommandServer = new CommandServerController() {
                TunnelObjects = new List<ICoreController>() {
                    this
                }
            };

            Database = new DatabaseController() {
                BubbleObjects = new List<ICoreController>() {
                    this
                }
            };

            PushEvents = new PushEventsController() {
                BubbleObjects = new List<ICoreController>() {
                    this
                }
            };

            Protocols = new ProtocolController() {
                BubbleObjects = new List<ICoreController>() {
                    this
                }
            };

            Tasks = new List<Timer>() {
                new Timer(Connection_Tick, this, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(15)),
                new Timer(Events_Tick, this, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60)),
                new Timer(CommandServer_Tick, this, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60)),
                new Timer(Packages_Tick, this, TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(60)),
                new Timer(Security_Tick, this, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60))
            };

            EventsConsole = new EventsConsoleController();

            CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    CommandType = CommandType.PotatoServiceMergePackage,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "uri",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "packageId",
                            Type = typeof(string)
                        }
                    },
                    Handler = PotatoServiceMergePackage
                },
                new CommandDispatch() {
                    CommandType = CommandType.PotatoServiceUninstallPackage,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "packageId",
                            Type = typeof(string)
                        }
                    },
                    Handler = PotatoServiceUninstallPackage
                },
                new CommandDispatch() {
                    CommandType = CommandType.PotatoServiceRestart,
                    Handler = PotatoServiceRestart
                },
                new CommandDispatch() {
                    CommandType = CommandType.PotatoAddConnection,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "gameTypeProvider",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "gameTypeType",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "hostName",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "port",
                            Type = typeof(ushort)
                        },
                        new CommandParameterType() {
                            Name = "password",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "additional",
                            Type = typeof(string)
                        }
                    },
                    Handler = PotatoAddConnection
                },
                new CommandDispatch() {
                    CommandType = CommandType.PotatoRemoveConnection,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "connectionGuid",
                            Type = typeof(string)
                        }
                    },
                    Handler = PotatoRemoveConnectionByGuid
                },
                new CommandDispatch() {
                    CommandType = CommandType.PotatoRemoveConnection,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "gameTypeProvider",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "gameTypeType",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "hostName",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "port",
                            Type = typeof(ushort)
                        }
                    },
                    Handler = PotatoRemoveConnectionByDetails
                },
                new CommandDispatch() {
                    CommandType = CommandType.PotatoQuery,
                    Handler = PotatoQuery
                },
                new CommandDispatch() {
                    CommandType = CommandType.PotatoPing,
                    Handler = PotatoPing
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
            
            if (ServiceMessage != null) {
                message = ServiceMessage;

                // clear it for the next poll.
                ServiceMessage = null;
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
            var result = Tunnel(new Command() {
                Name = message.Name,
                Origin = CommandOrigin.Local,
                // This is possible because of the SortedDictionary used in Potato.Service.Shared.ArgumentsHelper
                // This method will always assume that Arguments.Values will result in the order of execution. 
                Parameters = message.Arguments.Values.Select(value => new CommandParameter() {
                    Data = {
                        Content = new List<string>() {
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
        private void Connection_Tick(object state) {
            if (Connections != null) {
                lock (Connections) {
                    foreach (var connection in Connections) {
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
            Shared.Events.WriteEvents();
        }

        /// <summary>
        /// Pokes the command server and all current active clients, ensuring we don't have any stale clients
        /// still held in memory.
        /// </summary>
        /// <param name="state"></param>
        protected void CommandServer_Tick(object state) {
            if (CommandServer != null) {
                CommandServer.Poke();
            }
        }

        /// <summary>
        /// Rebuilds the current repositories cache every 60 minutes
        /// </summary>
        /// <param name="state"></param>
        protected void Packages_Tick(object state) {
            if (Packages != null) {
                // Set the root (non-grouped) repository to Myrcon's official repository, or whatever
                // repository the host has setup.
                Shared.Variables.Variable(CommonVariableNames.PackagesRepositoryUri).Value = Shared.Variables.Get(CommonVariableNames.PackagesDefaultSourceRepositoryUri, Defines.PackagesDefaultSourceRepositoryUri);

                Packages.Poke();
            }
        }

        /// <summary>
        /// Removes all expired access tokens every 60 seconds.
        /// </summary>
        /// <param name="state"></param>
        protected void Security_Tick(object state) {
            if (Shared != null && Shared.Security != null) {
                Shared.Security.Poke();
            }
        }

        /// <summary>
        /// Assigns event handlers to deal with changes within the class.
        /// Starts the execution of this object's security, packages, and variables.
        /// Loads the configuration file.
        /// </summary>
        /// <returns></returns>
        public override ICoreController Execute() {
            EventsConsole.Execute();
            Packages.Execute();
            CommandServer.Execute();
            PushEvents.Execute();
            Database.Execute();
            Protocols.Execute();

            Execute(new Command() {
                Origin = CommandOrigin.Local
            }, new Config().Load(new DirectoryInfo(Defines.ConfigsDirectory.FullName)), Shared.Variables.Get<string>(CommonVariableNames.PotatoConfigPassword));
            
            Shared.Events.Log(new GenericEvent() {
                GenericEventType = GenericEventType.PotatoServiceStarted,
                Message = @"The service has started successfully."
            });

            return base.Execute();
        }

        /// <summary>
        /// IService implementation to call the execute method and ignore the return type.
        /// </summary>
        public void Start() {
            Execute();
        }

        /// <summary>
        /// IService proxy for the variables of this object.
        /// </summary>
        /// <param name="arguments"></param>
        public void ParseCommandLineArguments(List<string> arguments) {
            Shared.Variables.ParseArguments(arguments);
        }

        /// <summary>
        /// Writes the current object out to a file.
        /// </summary>
        public void WriteConfig() {
            IConfig config = new Config();
            config.Create<PotatoController>();
            WriteConfig(config, Shared.Variables.Get<string>(CommonVariableNames.PotatoConfigPassword));

            config.Save(new FileInfo(Path.Combine(Defines.ConfigsDirectory.FullName, GetType().Namespace + ".json")));
        }

        /// <summary>
        /// Saves all the connections to the config file.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="password"></param>
        public override void WriteConfig(IConfig config, string password = null) {
            Shared.Variables.WriteConfig(config, password);

            Shared.Events.WriteConfig(config, password);

            Packages.WriteConfig(config, password);

            Database.WriteConfig(config, password);

            Shared.Languages.WriteConfig(config, password);

            Shared.Security.WriteConfig(config, password);

            CommandServer.WriteConfig(config, password);
            
            PushEvents.WriteConfig(config, password);

            lock (Connections) {
                foreach (ConnectionController connection in Connections) {
                    // This command is executed in the Potato object.
                    // I had to write this comment because I kept moving it to the actual connection and failing oh so hard.
                    config.Append(new Command() {
                        CommandType = CommandType.PotatoAddConnection,
                        Parameters = new List<ICommandParameter>() {
                            new CommandParameter() {
                                Data = {
                                    Content = new List<string>() {
                                        connection.ConnectionModel.ProtocolType.Provider
                                    }
                                }
                            },
                            new CommandParameter() {
                                Data = {
                                    Content = new List<string>() {
                                        connection.ConnectionModel.ProtocolType.Type
                                    }
                                }
                            },
                            new CommandParameter() {
                                Data = {
                                    Content = new List<string>() {
                                        connection.ConnectionModel.Hostname
                                    }
                                }
                            },
                            new CommandParameter() {
                                Data = {
                                    Content = new List<string>() {
                                        connection.ConnectionModel.Port.ToString(CultureInfo.InvariantCulture)
                                    }
                                }
                            },
                            new CommandParameter() {
                                Data = {
                                    Content = new List<string>() {
                                        connection.ConnectionModel.Password
                                    }
                                }
                            },
                            new CommandParameter() {
                                Data = {
                                    Content = new List<string>() {
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
            return Tunnel(command);
        }

        protected override IList<ICoreController> TunnelExecutableObjects(ICommand command) {
            var list = new List<ICoreController>();

            if (command.Scope != null && command.Scope.ConnectionGuid != Guid.Empty) {
                // Focus only on the connection.
                Connections.Where(connection => connection.ConnectionModel.ConnectionGuid == command.Scope.ConnectionGuid).ToList().ForEach(list.Add);
            }
            else {
                // Add all of the connections.
                Connections.ForEach(list.Add);

                list.Add(PushEvents);
                list.Add(Shared.Security);
                list.Add(Shared.Languages);
                list.Add(Shared.Variables);
                list.Add(Shared.Events);
                list.Add(Packages);
                list.Add(Database);
                list.Add(Protocols);
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
            Tasks.ForEach(task => task.Dispose());
            Tasks.Clear();
            Tasks = null;

            // 2. Events contains varies references to other data through out Potato. This 
            // data should be written out first, writing out the full events data then 
            // moving on to disposing the actual data.
            Shared.Events.Dispose();
            Shared.Events = null;

            // 3. Stop/disconnect all game server connections, unload any plugins etc.
            lock (Connections) {
                foreach (ConnectionController connection in Connections) {
                    connection.Dispose();
                }
            }

            Connections.Clear();
            Connections = null;

            // @todo Does the order matter here? If so, document why.

            Shared.Security.Dispose();
            Shared.Security = null;

            Shared.Languages.Dispose();
            Shared.Languages = null;

            Packages.Dispose();
            Packages = null;

            Database.Dispose();
            Database = null;

            CommandServer.Dispose();
            CommandServer = null;

            PushEvents.Dispose();
            PushEvents = null;

            Protocols.Dispose();
            Protocols = null;

            Shared.Variables.Dispose();
            Shared.Variables = null;

            EventsConsole.Dispose();
            EventsConsole = null;

            base.Dispose();
        }

        /// <summary>
        /// Posts a restart signal for the service controller to poll.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult PotatoServiceRestart(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            // As long as the current account is allowed to execute this command...
            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                ServiceMessage = new ServiceMessage() {
                    Name = "restart"
                };

                result = new CommandResult() {
                    Message = string.Format("Successfully posted restart signal."),
                    CommandResultType = CommandResultType.Success,
                    Success = true
                };

                Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.PotatoServiceRestarting));
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
        public ICommandResult PotatoServiceMergePackage(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var uri = parameters["uri"].First<string>();
            var packageId = parameters["packageId"].First<string>();

            // As long as the current account is allowed to execute this command...
            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (string.IsNullOrEmpty(uri) == false && string.IsNullOrEmpty(packageId) == false) {
                    ServiceMessage = new ServiceMessage() {
                        Name = "merge",
                        Arguments = new Dictionary<string, string>() {
                            { "uri", uri },
                            { "packageid", packageId }
                        }
                    };

                    result = new CommandResult() {
                        Message = string.Format("Successfully posted merge signal."),
                        CommandResultType = CommandResultType.Success,
                        Success = true
                    };

                    Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.PotatoServiceMergePackage));
                }
                else {
                    result = new CommandResult() {
                        Message = string.Format(@"Invalid or missing parameter ""uri"" or ""packageId""."),
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
        public ICommandResult PotatoServiceUninstallPackage(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var packageId = parameters["packageId"].First<string>();

            // As long as the current account is allowed to execute this command...
            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (string.IsNullOrEmpty(packageId) == false) {
                    ServiceMessage = new ServiceMessage() {
                        Name = "uninstall",
                        Arguments = new Dictionary<string, string>() {
                            { "packageid", packageId }
                        }
                    };

                    result = new CommandResult() {
                        Message = string.Format("Successfully posted uninstall signal."),
                        CommandResultType = CommandResultType.Success,
                        Success = true
                    };

                    Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.PotatoServiceUninstallPackage));
                }
                else {
                    result = new CommandResult() {
                        Message = string.Format(@"Invalid or missing parameter ""packageId""."),
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
        public ICommandResult PotatoAddConnection(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;
            
            var protocolTypeProvider = parameters["gameTypeProvider"].First<string>() ?? "";
            var protocolTypeType = parameters["gameTypeType"].First<string>() ?? "";
            var hostName = parameters["hostName"].First<string>() ?? "";
            var port = parameters["port"].First<ushort>();
            var password = parameters["password"].First<string>() ?? "";
            var additional = parameters["additional"].First<string>() ?? "";

            // As long as the current account is allowed to execute this command...
            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                // As long as we have less than the maximum amount of connections...
                if (Connections.Count < Shared.Variables.Get(CommonVariableNames.MaximumProtocolConnections, 9000)) {
                    // As long as the connection for that specific game, hostname, and port does not exist...
                    if (Connections.FirstOrDefault(c => c.ConnectionModel.ProtocolType.Type == protocolTypeType && c.ConnectionModel.Hostname == hostName && c.ConnectionModel.Port == port) == null) {
                        // As long as the game type is defined...

                        var supportCheckResult = Protocols.Tunnel(CommandBuilder.ProtocolsCheckSupportedProtocol(protocolTypeProvider, protocolTypeType).SetOrigin(CommandOrigin.Local));

                        if (supportCheckResult.Success == true) {
                            var meta = supportCheckResult.Now.ProtocolAssemblyMetadatas.First();
                            
                            var connection = new ConnectionController() {
                                Potato = this
                            };

                            connection.SetupProtocol(supportCheckResult.Now.ProtocolAssemblyMetadatas.First(), supportCheckResult.Now.ProtocolTypes.First(), new ProtocolSetup() {
                                Hostname = hostName,
                                Port = port,
                                Password = password,
                                Arguments = ArgumentHelper.ToArguments(additional.Wordify()),
                                ConfigDirectory = meta.Directory.GetDirectories(Defines.ProtocolsDirectoryName, SearchOption.AllDirectories).Select(directory => directory.FullName).FirstOrDefault()
                            });

                            lock (Connections) {
                                Connections.Add(connection);
                            }

                            connection.Execute();
                            connection.AttemptConnection();

                            result = new CommandResult() {
                                Message = string.Format("Successfully added {0} connection.", protocolTypeType),
                                CommandResultType = CommandResultType.Success,
                                Success = true,
                                Now = {
                                    Connections = new List<ConnectionModel>() {
                                        connection.ConnectionModel
                                    }
                                }
                            };

                            Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.PotatoConnectionAdded));
                        }
                        else {
                            result = new CommandResult() {
                                Message = string.Format(@"Protocol type ""{0}"" is not supported.", protocolTypeType),
                                CommandResultType = CommandResultType.DoesNotExists,
                                Success = false
                            };
                        }
                    }
                    else {
                        result = new CommandResult() {
                            Message = string.Format(@"Game type ""{0}"" with connection to {1}:{2} has already been added.", protocolTypeType, hostName, port),
                            CommandResultType = CommandResultType.AlreadyExists,
                            Success = false
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = string.Format(@"Maximum number of game connections exceeded."),
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
            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {

                // As long as the connection for that specific game, hostname, and port exists...
                if (connection != null) {
                    lock (Connections) {
                        Connections.Remove(connection);
                    }

                    result = new CommandResult() {
                        Message = string.Format(@"Successfully removed connection with connection to {0}:{1} and game type ""{2}"".", connection.ConnectionModel.Hostname, connection.ConnectionModel.Port, connection),
                        CommandResultType = CommandResultType.Success,
                        Success = true,
                        Now = {
                            Connections = new List<ConnectionModel>() {
                                connection.ConnectionModel
                            }
                        }
                    };

                    Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.PotatoConnectionRemoved));

                    connection.Dispose();
                }
                else {
                    result = new CommandResult() {
                        Message = string.Format(@"Connection does not exist."),
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
        public ICommandResult PotatoRemoveConnectionByGuid(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var connectionGuid = parameters["connectionGuid"].First<string>();

            var connection = Connections.FirstOrDefault(x => string.Compare(x.ConnectionModel.ConnectionGuid.ToString(), connectionGuid, StringComparison.OrdinalIgnoreCase) == 0);

            return PotatoRemoveConnection(command, connection);
        }

        /// <summary>
        /// Removes a connection from this instance.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public ICommandResult PotatoRemoveConnectionByDetails(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var gameTypeProvider = parameters["gameTypeProvider"].First<string>();
            var gameTypeType = parameters["gameTypeType"].First<string>();
            var hostName = parameters["hostName"].First<string>();
            var port = parameters["port"].First<ushort>();

            var connection = Connections.FirstOrDefault(c =>
                c.ConnectionModel.ProtocolType.Provider == gameTypeProvider &&
                c.ConnectionModel.ProtocolType.Type == gameTypeType &&
                c.ConnectionModel.Hostname == hostName &&
                c.ConnectionModel.Port == port
            );

            return PotatoRemoveConnection(command, connection);
        }

        /// <summary>
        /// Queries this instance for a snapshot as it exists now
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult PotatoQuery(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;
            
            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                var packages = Packages.Tunnel(new Command(command) {
                    CommandType = CommandType.PackagesFetchPackages
                });

                var protocols = Protocols.Tunnel(new Command(command) {
                    CommandType = CommandType.ProtocolsFetchSupportedProtocols
                });
                
                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success,
                    Now = new CommandData() {
                        Connections = Connections.Select(connection => connection.ConnectionModel).ToList(),
                        ProtocolTypes = new List<ProtocolType>(protocols.Now.ProtocolTypes ?? new List<ProtocolType>()),
                        Repositories = new List<RepositoryModel>(packages.Now.Repositories ?? new List<RepositoryModel>()),
                        Groups = new List<Core.Shared.Models.GroupModel>(Shared.Security.Groups),
                        Languages = Shared.Languages.LoadedLanguageFiles.Select(language => language.LanguageModel).ToList(),
                        Variables = new List<VariableModel>(Shared.Variables.VolatileVariables.Values)
                    }
                };
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }
        
        /// <summary>
        /// Queries this instance for a response and uptime
        /// </summary>
        /// <returns></returns>
        public ICommandResult PotatoPing(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;
            
            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success,
                    Now = {
                        Content = new List<string>() {
                            Convert.ToInt32((DateTime.Now - InstantiatedStamp).TotalMilliseconds).ToString(CultureInfo.InvariantCulture)
                        }
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
