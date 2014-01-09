using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Procon.Core.Shared.Events;
using Procon.Net.Shared;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Actions.Deferred;
using Procon.Net.Shared.Models;

namespace Procon.Core.Shared.Plugins {

    /// <summary>
    /// The class in which all plugins should inherit from. This class handles all remoting
    /// to Procon and other standard tasks 
    /// </summary>
    public abstract class PluginController : AsynchronousCoreController, IPluginController {
        /// <summary>
        /// The Guid of the executing assembly. Used to uniquely identify this plugin. 
        /// </summary>
        public Guid PluginGuid { get; private set; }

        /// <summary>
        /// The connection that owns this plugin instance.
        /// </summary>
        public Guid ConnectionGuid { get; set; }

        /// <summary>
        /// Path to the log-file directory of the plugin
        /// </summary>
        public DirectoryInfo LogDirectoryInfo { get; set; }

        /// <summary>
        /// Path to the default config-file of the plugin
        /// </summary>
        public DirectoryInfo ConfigDirectoryInfo { get; set; }

        /// <summary>
        /// The latest GameState that was passed across the AppDomain.
        /// </summary>
        public ProtocolState GameState { get; set; }

        /// <summary>
        /// The interface to callback from the plugin side to Procon.
        /// </summary>
        //public IList<IExecutableBase> PluginCallback { private get; set; }

        /// <summary>
        /// All actions awaiting responses from the game networking layer
        /// </summary>
        protected ConcurrentDictionary<Guid, IDeferredAction> DeferredActions { get; set; } 

        protected PluginController() : base() {
            this.PluginGuid = this.GetAssemblyGuid();

            this.DeferredActions = new ConcurrentDictionary<Guid, IDeferredAction>();

            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        CommandType = CommandType.TextCommandsExecute,
                        CommandAttributeType = CommandAttributeType.Executed,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "text",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.TextCommandExecuted)
                }
            });
        }
        
        /// <summary>
        /// Finds the executing assemblies guid 
        /// </summary>
        /// <returns></returns>
        protected Guid GetAssemblyGuid() {
            Guid guid = Guid.Empty;

            GuidAttribute attribute = Assembly.GetAssembly(this.GetType()).GetCustomAttributes(typeof(GuidAttribute), true).Cast<GuidAttribute>().FirstOrDefault();

            if (attribute == null || Guid.TryParse(attribute.Value, out guid) == false) {
                throw new Exception("Missing assembly GuidAttribute or incorrect guid format");
            }

            return guid;
        }

        public override void BeginBubble(Command command, Action<CommandResultArgs> completed = null) {
            // There isn't much point in bubbling up if we just need to come back down here.
            if (command.Scope != null && command.Scope.PluginGuid == this.PluginGuid) {
                base.BeginTunnel(command, completed);
            }
            else if (this.BubbleObjects != null) {
                base.BeginBubble(command, completed);
            }
        }

        public override CommandResultArgs Bubble(Command command) {
            // There isn't much point in bubbling up if we just need to come back down here.
            if (command.Scope != null && command.Scope.PluginGuid == this.PluginGuid) {
                command.Result = this.Tunnel(command);
            }
            else if (this.BubbleObjects != null) {
                command.Result = base.Bubble(command);
            }

            return command.Result;
        }

        /// <summary>
        /// A helper function for sending proxy network actions to the server.
        /// </summary>
        /// <remarks>
        ///     <para>The action will be executed with no user specified, so it should always be successful, at least from a permissions point of view.</para>
        /// </remarks>
        /// <param name="action">The action to send to the server.</param>
        /// <returns>The result of the command, check the status for a success message.</returns>
        public virtual CommandResultArgs Action(NetworkAction action) {
            // Splitting the commands up is very deliberate, used to divide the permissions a user
            // requires to initiate a particular command. We do this here instead of later for.. well..
            // I don't know. A plugin might have a use to ignore this action at some point?

            CommandResultArgs result = null;
            CommandType command = CommandType.None;
            CommandParameter parameter = new CommandParameter();

            if (action is Chat) {
                command = CommandType.NetworkProtocolActionChat;
                parameter.Data.Chats = new List<Chat>() { action as Chat };
            }
            else if (action is Kill) {
                command = CommandType.NetworkProtocolActionKill;
                parameter.Data.Kills = new List<Kill>() { action as Kill };
            }
            else if (action is Kick) {
                command = CommandType.NetworkProtocolActionKick;
                parameter.Data.Kicks = new List<Kick>() { action as Kick };
            }
            else if (action is Ban) {
                command = CommandType.NetworkProtocolActionBan;
                parameter.Data.Bans = new List<Ban>() { action as Ban };
            }
            else if (action is Move) {
                command = CommandType.NetworkProtocolActionMove;
                parameter.Data.Moves = new List<Move>() { action as Move };
            }
            else if (action is Map) {
                command = CommandType.NetworkProtocolActionMap;
                parameter.Data.Maps = new List<Map>() { action as Map };
            }
            else if (action is Raw) {
                command = CommandType.NetworkProtocolActionRaw;
                parameter.Data.Raws = new List<Raw>() { action as Raw };
            }

            // Provided we have worked out what they wanted to send..
            if (command != CommandType.None) {
                result = this.Bubble(new Command() {
                    CommandType = command,
                    Scope = new CommandScope() {
                        ConnectionGuid = this.ConnectionGuid
                    },
                    Parameters = new List<CommandParameter>() {
                        parameter
                    }
                });
            }

            return result;
        }

        /// <summary>
        /// Sets up and sends a deferred action to the server.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public virtual CommandResultArgs Action(IDeferredAction action) {
            CommandResultArgs result = null;

            if (action.GetAction() != null) {
                // Wait for responses from this action
                this.DeferredActions.TryAdd(action.GetAction().Uid, action);

                // Send the action for processing
                result = this.Action(action.GetAction());

                // Alert the deferred action of packets that have been sent
                action.TryInsertSent(action.GetAction(), result.Now.Packets);
            }

            // Now return the result 
            return result;
        }

        /// <summary>
        /// Preps the config, then passes it to the executable object's WriteConfig(Config)
        /// </summary>
        /// <remarks>You can see a similar implementation of this in Instance, which is treated
        /// as the base class (therefore it's not passing a config to a child class) but instead
        /// needs to make the initial config object before writing to it.</remarks>
        public virtual void WriteConfig() {
            if (this.ConfigDirectoryInfo != null) {
                Config config = new Config();
                config.Create(this.GetType());
                this.WriteConfig(config);

                config.Save(new FileInfo(Path.Combine(this.ConfigDirectoryInfo.FullName, this.GetType().Namespace + ".xml")));
            }
        }

        public override void WriteConfig(Config config) {
            // Overwrite this method to write out your config
        }

        /// <summary>
        /// Loads the config from this plugins config directory.
        /// </summary>
        /// <remarks>Configs are simply saved commands</remarks>
        public virtual void LoadConfig() {
            this.GenericEvent(new GenericEventArgs() {
                GenericEventType = GenericEventType.ConfigLoading
            });

            this.Execute(new Config().Load(ConfigDirectoryInfo));

            this.GenericEvent(new GenericEventArgs() {
                GenericEventType = GenericEventType.ConfigLoaded
            });
        }

        public virtual void GameEvent(ProtocolEventArgs e) {
            this.GameState = e.ProtocolState;
        }

        public virtual void ClientEvent(ClientEventArgs e) {
            if (e.EventType == ClientEventType.ClientActionDone) {
                NetworkAction doneAction = e.Then.Actions.FirstOrDefault();

                if (doneAction != null) {
                    IDeferredAction deferredAction;

                    if (this.DeferredActions.TryRemove(doneAction.Uid, out deferredAction) == true && deferredAction.TryInsertDone(doneAction, e.Then.Packets, e.Now.Packets) == true) {
                        deferredAction.TryInsertAlways(doneAction);

                        deferredAction.Release();
                    }
                }
            }
            else if (e.EventType == ClientEventType.ClientActionExpired) {
                NetworkAction doneAction = e.Then.Actions.FirstOrDefault();

                if (doneAction != null) {
                    IDeferredAction deferredAction;

                    if (this.DeferredActions.TryRemove(doneAction.Uid, out deferredAction) == true && deferredAction.TryInsertExpired(doneAction, e.Then.Packets, e.Now.Packets) == true) {
                        deferredAction.TryInsertAlways(doneAction);

                        deferredAction.Release();
                    }
                }
            }
        }

        /// <summary>
        /// This method will be called when a text command has been successfully executed.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual CommandResultArgs TextCommandExecuted(Command command, Dictionary<String, CommandParameter> parameters) {

            // Not used.
            // String text = parameters["text"].First<String>();

            if (command.Result.Status == CommandResultType.Success && command.Result.Now.TextCommands.First().PluginUid == this.PluginGuid.ToString()) {
                this.Tunnel(new Command() {
                    Origin = CommandOrigin.Local,
                    Name = command.Result.Now.TextCommands.First().PluginCommand,
                    Parameters = new List<CommandParameter>() {
                        new CommandParameter() {
                            Data = {
                                CommandResults = new List<CommandResultArgs>() {
                                    command.Result
                                }
                            }
                        }
                    }
                });
            }

            return command.Result;
        }

        /// <summary>
        /// Consider this your constructor.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void GenericEventTypePluginLoaded(GenericEventArgs e) {
            this.LoadConfig();
        }

        /// <summary>
        /// Consider this the plugin destructor
        /// </summary>
        /// <param name="e"></param>
        protected virtual void GenericEventTypePluginUnloading(GenericEventArgs e) {
            this.WriteConfig();
        }

        /// <summary>
        /// A text command has been executed and it's callback directs to this plugin.
        /// </summary>
        /// <remarks>We convert to a new command and push it through to this plugin,
        /// which just cleans up the plugin implementation a little bit but converting
        /// a plugin executed command to a local command. Snazzified.</remarks>
        /// <param name="e"></param>
        protected virtual void GenericEventTypeTextCommandExecuted(GenericEventArgs e) {
            this.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                Name = e.Now.TextCommands.First().PluginCommand,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Events = new List<GenericEventArgs>() {
                                e
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// If you override this method then you should call the base, unless you want to implement your
        /// own functionality. All actions here are dispatched to other virtual methods that you can override in
        /// your plugin if you just want to prevent/alter some default functionality.
        /// </summary>
        /// <param name="e"></param>
        public virtual void GenericEvent(GenericEventArgs e) {
            if (e.GenericEventType == GenericEventType.PluginsPluginLoaded) {
                this.GenericEventTypePluginLoaded(e);
            }
            else if (e.GenericEventType == GenericEventType.PluginsPluginUnloading) {
                this.GenericEventTypePluginUnloading(e);
            }
            else if (e.GenericEventType == GenericEventType.TextCommandExecuted) {
                this.GenericEventTypeTextCommandExecuted(e);
            }
        }
    }
}