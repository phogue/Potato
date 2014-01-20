using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Procon.Core.Connections;
using Procon.Core.Database;
using Procon.Core.Events;
using Procon.Core.Packages;
using Procon.Core.Remote;
using Procon.Core.Shared;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;
using Procon.Net.Shared;
using Procon.Net.Shared.Protocols;
using Procon.Service.Shared;

namespace Procon.Core {
    /// <summary>
    /// The core controller of Procon, an instance of Procon.
    /// </summary>
    public class InstanceController : CoreController, ISharedReferenceAccess, IService {
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
        /// Creates a new instance of Procon, setting up command server, packages and tasks
        /// </summary>
        public InstanceController() : base() {
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

            this.PushEvents = new PushEventsController();

            this.Tasks = new List<Timer>() {
                new Timer(Connection_Tick, this, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(15)),
                new Timer(Events_Tick, this, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60)),
                new Timer(CommandServer_Tick, this, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60)),
                new Timer(Packages_Tick, this, TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(60))
            };

            this.EventsConsole = new EventsConsoleController();

            this.CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    CommandType = CommandType.InstanceServiceMergePackage,
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
                    Handler = this.InstanceServiceMergePackage
                },
                new CommandDispatch() {
                    CommandType = CommandType.InstanceServiceUninstallPackage,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "packageId",
                            Type = typeof(String)
                        }
                    },
                    Handler = this.InstanceServiceUninstallPackage
                },
                new CommandDispatch() {
                    CommandType = CommandType.InstanceServiceRestart,
                    Handler = this.InstanceServiceRestart
                },
                new CommandDispatch() {
                    CommandType = CommandType.InstanceAddConnection,
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
                    Handler = this.InstanceAddConnection
                },
                new CommandDispatch() {
                    CommandType = CommandType.InstanceRemoveConnection,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "connectionGuid",
                            Type = typeof(String)
                        }
                    },
                    Handler = this.InstanceRemoveConnectionByGuid
                },
                new CommandDispatch() {
                    CommandType = CommandType.InstanceRemoveConnection,
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
                    Handler = this.InstanceRemoveConnectionByDetails
                },
                new CommandDispatch() {
                    CommandType = CommandType.InstanceQuery,
                    Handler = this.InstanceQuery
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

                // clear it for the net poll.
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

            this.Execute(new Command() {
                Origin = CommandOrigin.Local
            }, new Config().Load(new DirectoryInfo(Defines.ConfigsDirectory.FullName)));
            
            this.Shared.Events.Log(new GenericEvent() {
                GenericEventType = GenericEventType.InstanceServiceStarted,
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
            config.Create<InstanceController>();
            this.WriteConfig(config);

            config.Save(new FileInfo(Path.Combine(Defines.ConfigsDirectory.FullName, this.GetType().Namespace + ".json")));
        }

        /// <summary>
        /// Saves all the connections to the config file.
        /// </summary>
        /// <param name="config"></param>
        public override void WriteConfig(IConfig config) {
            this.Shared.Variables.WriteConfig(config);

            this.Shared.Events.WriteConfig(config);

            this.Packages.WriteConfig(config);

            this.Database.WriteConfig(config);

            this.Shared.Languages.WriteConfig(config);

            this.Shared.Security.WriteConfig(config);

            this.CommandServer.WriteConfig(config);
            
            this.PushEvents.WriteConfig(config);

            lock (this.Connections) {
                foreach (ConnectionController connection in this.Connections) {
                    // This command is executed in the Instance object.
                    // I had to write this comment because I kept moving it to the actual connection and failing oh so hard.
                    config.Append(new Command() {
                        CommandType = CommandType.InstanceAddConnection,
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
                                        connection.ConnectionModel.Additional
                                    }
                                }
                            }
                        }
                    }.ToConfigCommand());

                    connection.WriteConfig(config);
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

            if (command.ScopeModel != null && command.ScopeModel.ConnectionGuid != Guid.Empty) {
                // Focus only on the connection.
                this.Connections.Where(connection => connection.ConnectionModel.ConnectionGuid == command.ScopeModel.ConnectionGuid).ToList().ForEach(list.Add);
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

            // 2. Events contains varies references to other data through out Procon. This 
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
        public ICommandResult InstanceServiceRestart(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            // As long as the current account is allowed to execute this command...
            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                this.ServiceMessage = new ServiceMessage() {
                    Name = "restart"
                };

                result = new CommandResult() {
                    Message = String.Format("Successfully posted restart signal."),
                    Status = CommandResultType.Success,
                    Success = true
                };

                this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.InstanceServiceRestarting));
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
        public ICommandResult InstanceServiceMergePackage(ICommand command, Dictionary<String, ICommandParameter> parameters) {
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
                        Status = CommandResultType.Success,
                        Success = true
                    };

                    this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.InstanceServiceMergePackage));
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Invalid or missing parameter ""uri"" or ""packageId""."),
                        Status = CommandResultType.InvalidParameter,
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
        public ICommandResult InstanceServiceUninstallPackage(ICommand command, Dictionary<String, ICommandParameter> parameters) {
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
                        Status = CommandResultType.Success,
                        Success = true
                    };

                    this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.InstanceServiceUninstallPackage));
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Invalid or missing parameter ""packageId""."),
                        Status = CommandResultType.InvalidParameter,
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
        public ICommandResult InstanceAddConnection(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            String gameTypeProvider = parameters["gameTypeProvider"].First<String>();
            String gameTypeType = parameters["gameTypeType"].First<String>();
            String hostName = parameters["hostName"].First<String>();
            UInt16 port = parameters["port"].First<UInt16>();
            String password = parameters["password"].First<String>();
            String additional = parameters["additional"].First<String>();

            // As long as the current account is allowed to execute this command...
            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                // As long as we have less than the maximum amount of connections...
                if (this.Connections.Count < this.Shared.Variables.Get(CommonVariableNames.MaximumProtocolConnections, 9000)) {
                    // As long as the connection for that specific game, hostname, and port does not exist...
                    if (this.Connections.FirstOrDefault(c => c.ConnectionModel.ProtocolType.Type == gameTypeType && c.ConnectionModel.Hostname == hostName && c.ConnectionModel.Port == port) == null) {
                        // As long as the game type is defined...

                        Type gameType = SupportedGameTypes.GetSupportedGames().Where(g => g.Key.Provider == gameTypeProvider && g.Key.Type == gameTypeType).Select(g => g.Value).FirstOrDefault();

                        // As long as the game type selected is supported...
                        if (gameType != null) {
                            IProtocol game = (IProtocol) Activator.CreateInstance(gameType, hostName, port);
                            game.Additional = additional;
                            game.Password = password;

                            DirectoryInfo packagePath = Defines.PackageContainingPath(gameType.Assembly.Location);

                            if (packagePath != null) {
                                game.ProtocolsConfigDirectory = packagePath.GetDirectories(Defines.ProtocolsDirectoryName, SearchOption.AllDirectories).Select(directory => directory.FullName).FirstOrDefault();
                            }

                            ConnectionController connection = new ConnectionController() {
                                Protocol = game,
                                Instance = this
                            };

                            lock (this.Connections) {
                                this.Connections.Add(connection);
                            }

                            connection.Execute();
                            connection.AttemptConnection();

                            result = new CommandResult() {
                                Message = String.Format("Successfully added {0} connection.", gameTypeType),
                                Status = CommandResultType.Success,
                                Success = true,
                                Now = {
                                    Connections = new List<ConnectionModel>() {
                                        connection.ConnectionModel
                                    }
                                }
                            };

                            this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.InstanceConnectionAdded));
                        }
                        else {
                            result = new CommandResult() {
                                Message = String.Format(@"Game type ""{0}"" is not supported.", gameTypeType),
                                Status = CommandResultType.DoesNotExists,
                                Success = false
                            };
                        }
                    }
                    else {
                        result = new CommandResult() {
                            Message = String.Format(@"Game type ""{0}"" with connection to {1}:{2} has already been added.", gameTypeType, hostName, port),
                            Status = CommandResultType.AlreadyExists,
                            Success = false
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Maximum number of game connections exceeded."),
                        Status = CommandResultType.LimitExceeded,
                        Success = false
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        protected ICommandResult InstanceRemoveConnection(ICommand command, IConnectionController connection) {
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
                        Status = CommandResultType.Success,
                        Success = true,
                        Now = {
                            Connections = new List<ConnectionModel>() {
                                connection.ConnectionModel
                            }
                        }
                    };

                    this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.InstanceConnectionRemoved));

                    connection.Dispose();
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Connection does not exist."),
                        Status = CommandResultType.DoesNotExists,
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
        public ICommandResult InstanceRemoveConnectionByGuid(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            String connectionGuid = parameters["connectionGuid"].First<String>();

            IConnectionController connection = this.Connections.FirstOrDefault(x => String.Compare(x.ConnectionModel.ConnectionGuid.ToString(), connectionGuid, StringComparison.OrdinalIgnoreCase) == 0);

            return this.InstanceRemoveConnection(command, connection);
        }

        /// <summary>
        /// Removes a connection from this instance.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public ICommandResult InstanceRemoveConnectionByDetails(ICommand command, Dictionary<String, ICommandParameter> parameters) {
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

            return this.InstanceRemoveConnection(command, connection);
        }

        /// <summary>
        /// Queries this instance for a snapshot as it exists now
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult InstanceQuery(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                ICommandResult packages = this.Packages.Tunnel(new Command(command) {
                    CommandType = CommandType.PackagesFetchPackages
                });
                
                result = new CommandResult() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Now = new CommandData() {
                        Connections = this.Connections.Select(connection => connection.ConnectionModel).ToList(),
                        GameTypes = new List<ProtocolType>(SupportedGameTypes.GetSupportedGames().Select(k => k.Key as ProtocolType)),
                        Repositories = new List<RepositoryModel>(packages.Now.Repositories ?? new List<RepositoryModel>()),
                        Groups = new List<GroupModel>(this.Shared.Security.Groups),
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
