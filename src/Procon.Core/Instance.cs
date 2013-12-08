using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Procon.Core.Connections.Plugins;
using Procon.Core.Events;
using Procon.Core.Localization;
using Procon.Core.Remote;
using Procon.Core.Scheduler;
using Procon.Core.Security;
using Procon.Net.Protocols;

namespace Procon.Core {
    using Procon.Core.Connections;
    using Procon.Core.Repositories;
    using Procon.Core.Variables;
    using Procon.Net;
    using Procon.Service.Shared;
    
    /// <summary>
    /// The core controller of Procon, an instance of Procon.
    /// </summary>
    public class Instance : Executable, IService {

        /// <summary>
        /// List of game connections
        /// </summary>
        public List<Connection> Connections { get; protected set; }

        /// <summary>
        /// The packages that are intalled or can be installed.
        /// </summary>
        public RepositoryController Packages { get; protected set; }

        /// <summary>
        /// The daemon controller, if active.
        /// </summary>
        public DaemonController Daemon { get; protected set; }

        /// <summary>
        /// Controller to push events to various sources.
        /// </summary>
        public PushEventsController PushEvents { get; protected set; }

        /// <summary>
        /// Tasks to be run by this connection. Primarily used to reattempt connections
        /// or force a synchronization of the game data.
        /// </summary>
        public TaskController Tasks { get; protected set; }

        /// <summary>
        /// Output to the console for all events processed by this instance.
        /// </summary>
        public EventsConsoleController EventsConsole { get; protected set; }

        /// <summary>
        /// The latest service message to post back to the service controller. If no message exists then a "ok" response will be sent.
        /// </summary>
        protected ServiceMessage ServiceMessage { get; set; }

        /// <summary>
        /// Creates a new instance of Procon, setting up daemon, packages and tasks
        /// </summary>
        public Instance() : base() {
            this.Connections = new List<Connection>();

            this.Packages = new RepositoryController();

            this.Daemon = new DaemonController() {
                TunnelObjects = new List<IExecutableBase>() {
                    this
                }
            };

            this.PushEvents = new PushEventsController();

            this.Tasks = new TaskController();

            this.EventsConsole = new EventsConsoleController();

            // Tick every 15 seconds.
            this.Tasks.Add(
                new Task() {
                    Condition = new Temporal() {
                        (date, task) => date.Second % 15 == 0
                    }
                }
            ).Tick += new Task.TickHandler(Connection_Tick);

            // Tick every minute.
            this.Tasks.Add(
                new Task() {
                    Condition = new Temporal() {
                        (date, task) => date.Minute % 1 == 0 && date.Second == 0
                    }
                }
            ).Tick += new Task.TickHandler(Events_Tick);

            // Tick every minute.
            this.Tasks.Add(
                new Task() {
                    Condition = new Temporal() {
                        (date, task) => date.Minute % 1 == 0 && date.Second == 0
                    }
                }
            ).Tick += new Task.TickHandler(Daemon_Tick);

            this.Tasks.Add(
                new Task() {
                    Condition = new Temporal() {
                        (date, task) => date.Minute % 1 == 0 && date.Second == 0
                    }
                }
            ).Tick += new Task.TickHandler(Plugin_Tick);

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
        /// Writes out the events to file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Events_Tick(object sender, TickEventArgs e) {
            this.Events.WriteEvents();
        }

        /// <summary>
        /// Reconnects or synchronizes data on each connection. A periodic standard
        /// ten second poke of each connection pretty much.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connection_Tick(Object sender, TickEventArgs e) {
            if (this.Connections != null) {
                lock (this.Connections) {
                    foreach (Connection connection in this.Connections.Where(connection => connection.Game != null)) {
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
        /// Pokes the daemon and all current active clients, ensuring we don't have any stale clients
        /// still held in memory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Daemon_Tick(Object sender, TickEventArgs e) {
            if (this.Daemon != null) {
                this.Daemon.Poke();
            }
        }

        /// <summary>
        /// Renews all of the remoting leases on active plugins for each connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Plugin_Tick(Object sender, TickEventArgs e) {
            if (this.Connections != null) {
                foreach (PluginController plugins in this.Connections.Select(connection => connection.Plugins)) {
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
        public override ExecutableBase Execute() {
            this.EventsConsole.Execute();
            this.Packages.Execute();
            this.Daemon.Execute();
            this.PushEvents.Execute();

            this.Tasks.Start();

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
            this.Variables.ParseArguments(arguments);
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
            Config variablesConfig = new Config().Create(this.Variables.GetType());
            this.Variables.WriteConfig(variablesConfig);
            config.Combine(variablesConfig);

            Config eventsConfig = new Config().Create(this.Events.GetType());
            this.Events.WriteConfig(eventsConfig);
            config.Combine(eventsConfig);

            Config packagesConfig = new Config().Create(this.Packages.GetType());
            this.Packages.WriteConfig(packagesConfig);
            config.Combine(packagesConfig);

            Config languagesConfig = new Config().Create(this.Languages.GetType());
            this.Languages.WriteConfig(languagesConfig);
            config.Combine(languagesConfig);

            Config securityConfig = new Config().Create(this.Security.GetType());
            this.Security.WriteConfig(securityConfig);
            config.Combine(securityConfig);

            Config daemonConfig = new Config().Create(this.Daemon.GetType());
            this.Daemon.WriteConfig(daemonConfig);
            config.Combine(daemonConfig);

            Config pushEventsConfig = new Config().Create(this.PushEvents.GetType());
            this.PushEvents.WriteConfig(pushEventsConfig);
            config.Combine(pushEventsConfig);

            lock (this.Connections) {
                foreach (Connection connection in this.Connections) {
                    // This command is executed in the Instance object.
                    // I had to write this comment because I kept moving it to the actual connection and failing oh so hard.
                    config.Root.Add(new Command() {
                        CommandType = CommandType.InstanceAddConnection,
                        Parameters = new List<CommandParameter>() {
                            new CommandParameter() {
                                Data = {
                                    Content = new List<String>() {
                                        connection.GameType.Provider
                                    }
                                }
                            },
                            new CommandParameter() {
                                Data = {
                                    Content = new List<String>() {
                                        connection.GameType.Type
                                    }
                                }
                            },
                            new CommandParameter() {
                                Data = {
                                    Content = new List<String>() {
                                        connection.Hostname
                                    }
                                }
                            },
                            new CommandParameter() {
                                Data = {
                                    Content = new List<String>() {
                                        connection.Port.ToString(CultureInfo.InvariantCulture)
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

                    Config connectionConfig = new Config().Create(connection.GetType());
                    connection.WriteConfig(connectionConfig);
                    config.Combine(connectionConfig);
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

        protected override IList<IExecutableBase> TunnelExecutableObjects(Command command) {
            List<IExecutableBase> list = new List<IExecutableBase>();

            if (command.Scope != null && command.Scope.ConnectionGuid != Guid.Empty) {
                // Focus only on the connection.
                this.Connections.Where(connection => connection.ConnectionGuid == command.Scope.ConnectionGuid).ToList().ForEach(list.Add);
            }
            else {
                // Add all of the connections.
                this.Connections.ForEach(list.Add);

                list.Add(this.PushEvents);
                list.Add(this.Security);
                list.Add(this.Languages);
                list.Add(this.Variables);
                list.Add(this.Events);
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
            foreach (Task task in this.Tasks) {
                task.Tick -= new Task.TickHandler(Connection_Tick);
                task.Tick -= new Task.TickHandler(Events_Tick);
            }

            this.Tasks.Dispose();
            this.Tasks = null;

            // 2. Events contains varies references to other data through out Procon. This 
            // data should be written out first, writing out the full events data then 
            // moving on to disposing the actual data.
            this.Events.Dispose();
            this.Events = null;

            // 3. Stop/disconnect all game server connections, unload any plugins etc.
            lock (this.Connections) {
                foreach (Connection connection in this.Connections) {
                    connection.Dispose();
                }
            }

            this.Connections.Clear();
            this.Connections = null;

            // @todo Does the order matter here? If so, document why.

            this.Security.Dispose();
            this.Security = null;

            this.Languages.Dispose();
            this.Languages = null;

            this.Packages.Dispose();
            this.Packages = null;

            this.Daemon.Dispose();
            this.Daemon = null;

            this.PushEvents.Dispose();
            this.PushEvents = null;

            this.Variables.Dispose();
            this.Variables = null;

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
            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                this.ServiceMessage = new ServiceMessage() {
                    Name = "restart"
                };

                result = new CommandResultArgs() {
                    Message = String.Format("Successfully posted restart signal."),
                    Status = CommandResultType.Success,
                    Success = true
                };

                this.Events.Log(GenericEventArgs.ConvertToGenericEvent(result, GenericEventType.InstanceServiceRestarting));
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
            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                // As long as we have less than the maximum amount of connections...
                if (this.Connections.Count < this.Variables.Get(CommonVariableNames.MaximumGameConnections, 9000)) {
                    // As long as the connection for that specific game, hostname, and port does not exist...
                    if (this.Connections.FirstOrDefault(c => c.GameType.Type == gameTypeType && c.Hostname == hostName && c.Port == port) == null) {
                        // As long as the game type is defined...

                        Type gameType = SupportedGameTypes.GetSupportedGames().Where(g => g.Key.Provider == gameTypeProvider && g.Key.Type == gameTypeType).Select(g => g.Value).FirstOrDefault();

                        // As long as the game type selected is supported...
                        if (gameType != null) {
                            Game game = (Game) Activator.CreateInstance(gameType, hostName, port);
                            game.Additional = additional;
                            game.Password = password;
                            game.GameConfigPath = Defines.ConfigsGamesDirectory;

                            Connection connection = new Connection() {
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
                                    Connections = new List<Connection>() {
                                        connection
                                    }
                                }
                            };

                            this.Events.Log(GenericEventArgs.ConvertToGenericEvent(result, GenericEventType.InstanceConnectionAdded));
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

        protected CommandResultArgs InstanceRemoveConnection(Command command, Connection connection) {
            CommandResultArgs result = null;

            // As long as the current account is allowed to execute this command...
            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {

                // As long as the connection for that specific game, hostname, and port exists...
                if (connection != null) {
                    lock (this.Connections) {
                        this.Connections.Remove(connection);
                    }

                    result = new CommandResultArgs() {
                        Message = String.Format(@"Successfully removed connection with connection to {0}:{1} and game type ""{2}"".", connection.Hostname, connection.Port, connection),
                        Status = CommandResultType.Success,
                        Success = true,
                        Now = {
                            Connections = new List<Connection>() {
                                // Clone it, cause we're about to properly dispose the connection.
                                connection.Clone() as Connection
                            }
                        }
                    };

                    this.Events.Log(GenericEventArgs.ConvertToGenericEvent(result, GenericEventType.InstanceConnectionRemoved));

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

            Connection connection = this.Connections.FirstOrDefault(x => String.Compare(x.ConnectionGuid.ToString(), connectionGuid, StringComparison.OrdinalIgnoreCase) == 0);

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

            Connection connection = this.Connections.FirstOrDefault(c => 
                c.GameType.Provider == gameTypeProvider && 
                c.GameType.Type == gameTypeType && 
                c.Hostname == hostName && 
                c.Port == port
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

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Now = new CommandData() {
                        Connections = new List<Connection>(this.Connections),
                        GameTypes = new List<GameType>(SupportedGameTypes.GetSupportedGames().Select(k => k.Key as GameType)),
                        Repositories = new List<Repository>(this.Packages.RemoteRepositories),
                        Packages = new List<FlatPackedPackage>(this.Packages.Packages),
                        Groups = new List<Group>(this.Security.Groups),
                        Languages = new List<Language>(this.Languages.LoadedLanguageFiles),
                        Variables = new List<Variable>(this.Variables.VolatileVariables)
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
