using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Procon.Core.Connections;
using Procon.Core.Connections.Plugins;
using Procon.Core.Events;
using Procon.Core.Remote;
using Procon.Core.Repositories;
using Procon.Core.Shared;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;
using Procon.Net;
using Procon.Net.Shared;
using Procon.Net.Shared.Protocols;
using Procon.Service.Shared;

namespace Procon.Core {
    /// <summary>
    /// The core controller of Procon, an instance of Procon.
    /// </summary>
    public class Instance : CoreController, ISharedReferenceAccess, IService {
        /// <summary>
        /// List of game connections
        /// </summary>
        public List<ConnectionController> Connections { get; protected set; }

        /// <summary>
        /// The packages that are intalled or can be installed.
        /// </summary>
        public RepositoryController Packages { get; protected set; }

        /// <summary>
        /// The command server controller, if active.
        /// </summary>
        public CommandServerController CommandServer { get; protected set; }

        /// <summary>
        /// Controller to push events to various sources.
        /// </summary>
        public PushEventsController PushEvents { get; protected set; }

        /// <summary>
        /// Tasks to be run by this connection. Primarily used to reattempt connections
        /// or force a synchronization of the game data.
        /// </summary>
        public List<Timer> Tasks { get; set; } 

        /// <summary>
        /// Output to the console for all events processed by this instance.
        /// </summary>
        public EventsConsoleController EventsConsole { get; protected set; }

        /// <summary>
        /// The latest service message to post back to the service controller. If no message exists then a "ok" response will be sent.
        /// </summary>
        protected ServiceMessage ServiceMessage { get; set; }

        [XmlIgnore, JsonIgnore]
        public SharedReferences Shared { get; private set; }

        /// <summary>
        /// Creates a new instance of Procon, setting up command server, packages and tasks
        /// </summary>
        public Instance() : base() {
            this.Shared = new SharedReferences();

            this.Connections = new List<ConnectionController>();

            this.Packages = new RepositoryController();

            this.CommandServer = new CommandServerController() {
                TunnelObjects = new List<ICoreController>() {
                    this
                }
            };

            this.PushEvents = new PushEventsController();

            this.Tasks = new List<Timer>() {
                new Timer(Connection_Tick, this, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(15)),
                new Timer(Events_Tick, this, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(60)),
                new Timer(CommandServer_Tick, this, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(60)),
                new Timer(Plugin_Tick, this, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(60))
            };

            this.EventsConsole = new EventsConsoleController();

            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        CommandType = CommandType.InstanceServiceRestart
                    },
                    new CommandDispatchHandler(this.InstanceServiceRestart)
                }, {
                    new CommandAttribute() {
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
                        }
                    },
                    new CommandDispatchHandler(this.InstanceAddConnection)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.InstanceRemoveConnection,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "connectionGuid",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.InstanceRemoveConnectionByGuid)
                }, {
                    new CommandAttribute() {
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
                        }
                    },
                    new CommandDispatchHandler(this.InstanceRemoveConnectionByDetails)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.InstanceQuery
                    },
                    new CommandDispatchHandler(this.InstanceQuery)
                }
            });
        }

        /// <summary>
        /// Fires a message across the appdomain back to the instance controller, provided 
        /// a callback has been setup.
        /// </summary>
        /// <remarks>
        /// If the main thread is tied up then "ok" won't be sent back and the service 
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
                    Name = "ok"
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
                    foreach (ConnectionController connection in this.Connections.Where(connection => connection.Game != null)) {
                        if (connection.Game.State != null && connection.Game.State.Settings.Current.ConnectionState == ConnectionState.ConnectionDisconnected) {
                            connection.AttemptConnection();
                        }
                        else {
                            connection.Game.Synchronize();
                        }
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
        /// Renews all of the remoting leases on active plugins for each connection.
        /// </summary>
        /// <param name="state"></param>
        protected void Plugin_Tick(Object state) {
            if (this.Connections != null) {
                foreach (CorePluginController plugins in this.Connections.Select(connection => connection.Plugins)) {
                    plugins.RenewLease();
                }
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

            this.Execute(new Command() {
                Origin = CommandOrigin.Local
            }, new Config().Load(new DirectoryInfo(Defines.ConfigsDirectory)));
            
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
            Config config = new Config();
            config.Create(this.GetType());
            this.WriteConfig(config);

            config.Save(new FileInfo(Path.Combine(Defines.ConfigsDirectory, this.GetType().Namespace + ".xml")));
        }

        /// <summary>
        /// Saves all the connections to the config file.
        /// </summary>
        /// <param name="config"></param>
        public override void WriteConfig(Config config) {
            this.Shared.Variables.WriteConfig(config);

            this.Shared.Events.WriteConfig(config);

            this.Packages.WriteConfig(config);

            this.Shared.Languages.WriteConfig(config);

            this.Shared.Security.WriteConfig(config);

            this.CommandServer.WriteConfig(config);
            
            this.PushEvents.WriteConfig(config);

            lock (this.Connections) {
                foreach (ConnectionController connection in this.Connections) {
                    // This command is executed in the Instance object.
                    // I had to write this comment because I kept moving it to the actual connection and failing oh so hard.
                    config.Root.Add(new Command() {
                        CommandType = CommandType.InstanceAddConnection,
                        Parameters = new List<CommandParameter>() {
                            new CommandParameter() {
                                Data = {
                                    Content = new List<String>() {
                                        connection.ConnectionModel.GameType.Provider
                                    }
                                }
                            },
                            new CommandParameter() {
                                Data = {
                                    Content = new List<String>() {
                                        connection.ConnectionModel.GameType.Type
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
                                        connection.Password
                                    }
                                }
                            },
                            new CommandParameter() {
                                Data = {
                                    Content = new List<String>() {
                                        connection.Additional
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
        public override CommandResultArgs Bubble(Command command) {
            return this.Tunnel(command);
        }

        protected override IList<ICoreController> TunnelExecutableObjects(Command command) {
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

            this.CommandServer.Dispose();
            this.CommandServer = null;

            this.PushEvents.Dispose();
            this.PushEvents = null;

            this.Shared.Variables.Dispose();
            this.Shared.Variables = null;

            base.Dispose();
        }

        /// <summary>
        /// Posts a restart signal for the service controller to poll.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CommandResultArgs InstanceServiceRestart(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            // As long as the current account is allowed to execute this command...
            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                this.ServiceMessage = new ServiceMessage() {
                    Name = "restart"
                };

                result = new CommandResultArgs() {
                    Message = String.Format("Successfully posted restart signal."),
                    Status = CommandResultType.Success,
                    Success = true
                };

                this.Shared.Events.Log(GenericEventArgs.ConvertToGenericEvent(result, GenericEventType.InstanceServiceRestarting));
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Add a connection to this instance.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public CommandResultArgs InstanceAddConnection(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            String gameTypeProvider = parameters["gameTypeProvider"].First<String>();
            String gameTypeType = parameters["gameTypeType"].First<String>();
            String hostName = parameters["hostName"].First<String>();
            UInt16 port = parameters["port"].First<UInt16>();
            String password = parameters["password"].First<String>();
            String additional = parameters["additional"].First<String>();

            // As long as the current account is allowed to execute this command...
            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                // As long as we have less than the maximum amount of connections...
                if (this.Connections.Count < this.Shared.Variables.Get(CommonVariableNames.MaximumGameConnections, 9000)) {
                    // As long as the connection for that specific game, hostname, and port does not exist...
                    if (this.Connections.FirstOrDefault(c => c.ConnectionModel.GameType.Type == gameTypeType && c.ConnectionModel.Hostname == hostName && c.ConnectionModel.Port == port) == null) {
                        // As long as the game type is defined...

                        Type gameType = SupportedGameTypes.GetSupportedGames().Where(g => g.Key.Provider == gameTypeProvider && g.Key.Type == gameTypeType).Select(g => g.Value).FirstOrDefault();

                        // As long as the game type selected is supported...
                        if (gameType != null) {
                            Game game = (Game) Activator.CreateInstance(gameType, hostName, port);
                            game.Additional = additional;
                            game.Password = password;
                            game.GameConfigPath = Defines.ConfigsGamesDirectory;

                            ConnectionController connection = new ConnectionController() {
                                Game = game,
                                Instance = this
                            };

                            lock (this.Connections) {
                                this.Connections.Add(connection);
                            }

                            connection.Execute();
                            connection.AttemptConnection();

                            result = new CommandResultArgs() {
                                Message = String.Format("Successfully added {0} connection.", gameTypeType),
                                Status = CommandResultType.Success,
                                Success = true,
                                Now = {
                                    Connections = new List<ConnectionModel>() {
                                        connection.ConnectionModel
                                    }
                                }
                            };

                            this.Shared.Events.Log(GenericEventArgs.ConvertToGenericEvent(result, GenericEventType.InstanceConnectionAdded));
                        }
                        else {
                            result = new CommandResultArgs() {
                                Message = String.Format(@"Game type ""{0}"" is not supported.", gameTypeType),
                                Status = CommandResultType.DoesNotExists,
                                Success = false
                            };
                        }
                    }
                    else {
                        result = new CommandResultArgs() {
                            Message = String.Format(@"Game type ""{0}"" with connection to {1}:{2} has already been added.", gameTypeType, hostName, port),
                            Status = CommandResultType.AlreadyExists,
                            Success = false
                        };
                    }
                }
                else {
                    result = new CommandResultArgs() {
                        Message = String.Format(@"Maximum number of game connections exceeded."),
                        Status = CommandResultType.LimitExceeded,
                        Success = false
                    };
                }
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        protected CommandResultArgs InstanceRemoveConnection(Command command, ConnectionController connection) {
            CommandResultArgs result = null;

            // As long as the current account is allowed to execute this command...
            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {

                // As long as the connection for that specific game, hostname, and port exists...
                if (connection != null) {
                    lock (this.Connections) {
                        this.Connections.Remove(connection);
                    }

                    result = new CommandResultArgs() {
                        Message = String.Format(@"Successfully removed connection with connection to {0}:{1} and game type ""{2}"".", connection.ConnectionModel.Hostname, connection.ConnectionModel.Port, connection),
                        Status = CommandResultType.Success,
                        Success = true,
                        Now = {
                            Connections = new List<ConnectionModel>() {
                                connection.ConnectionModel
                            }
                        }
                    };

                    this.Shared.Events.Log(GenericEventArgs.ConvertToGenericEvent(result, GenericEventType.InstanceConnectionRemoved));

                    connection.Dispose();
                }
                else {
                    result = new CommandResultArgs() {
                        Message = String.Format(@"Connection does not exist."),
                        Status = CommandResultType.DoesNotExists,
                        Success = false
                    };
                }
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Proxy to the expanded remove connction method, but only accepts a connectionGuid.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CommandResultArgs InstanceRemoveConnectionByGuid(Command command, Dictionary<String, CommandParameter> parameters) {
            String connectionGuid = parameters["connectionGuid"].First<String>();

            ConnectionController connection = this.Connections.FirstOrDefault(x => String.Compare(x.ConnectionModel.ConnectionGuid.ToString(), connectionGuid, StringComparison.OrdinalIgnoreCase) == 0);

            return this.InstanceRemoveConnection(command, connection);
        }

        /// <summary>
        /// Removes a connection from this instance.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public CommandResultArgs InstanceRemoveConnectionByDetails(Command command, Dictionary<String, CommandParameter> parameters) {
            String gameTypeProvider = parameters["gameTypeProvider"].First<String>();
            String gameTypeType = parameters["gameTypeType"].First<String>();
            String hostName = parameters["hostName"].First<String>();
            UInt16 port = parameters["port"].First<UInt16>();

            ConnectionController connection = this.Connections.FirstOrDefault(c =>
                c.ConnectionModel.GameType.Provider == gameTypeProvider &&
                c.ConnectionModel.GameType.Type == gameTypeType &&
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
        public CommandResultArgs InstanceQuery(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Now = new CommandData() {
                        Connections = this.Connections.Select(connection => connection.ConnectionModel).ToList(),
                        GameTypes = new List<GameType>(SupportedGameTypes.GetSupportedGames().Select(k => k.Key as GameType)),
                        //Repositories = new List<RepositoryModel>(this.Packages.RemoteRepositories),
                        //Packages = new List<PackageModel>(this.Packages.Packages),
                        Groups = new List<GroupModel>(this.Shared.Security.Groups),
                        Languages = this.Shared.Languages.LoadedLanguageFiles.Select(language => language.LanguageModel).ToList(),
                        Variables = new List<VariableModel>(this.Shared.Variables.VolatileVariables)
                    }
                };
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }
    }
}
