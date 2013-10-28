using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Procon.Core.Connections.Plugins {
    using Procon.Core.Events;
    using Procon.Core.Scheduler;
    using Procon.Net;
    using Procon.Net.Protocols.Objects;

    public abstract class PluginBase : ExecutableBase, IPluginBase {
        /// <summary>
        /// The Guid of the executing assembly. Used to uniquely identify this plugin. 
        /// </summary>
        public Guid PluginGuid { get; private set; }

        /// <summary>
        /// The connection that owns this plugin instance.
        /// </summary>
        public Guid ConnectionGuid { get; set; }

        protected Config PluginConfig { get; set; }

        /// <summary>
        /// Tasks running on a seperate thread for this instance of the plugin.
        /// </summary>
        public TaskController Tasks { get; protected set; }

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
        public GameState GameState { get; set; }

        public override object InitializeLifetimeService() {
            return null;
        }
        
        protected PluginBase() : base() {
            this.Tasks = new TaskController().Start();

            this.PluginGuid = this.GetType().GUID;
        }
        
        public override void Dispose() {
            base.Dispose();

            this.Tasks.Dispose();
            this.Tasks = null;
        }

        public delegate CommandResultArgs CommandHandler(Command command);
        public CommandHandler ProxyExecuteCallback { get; set; }

        /// <summary>
        /// Executes a command across the appdomain
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public CommandResultArgs ProxyExecute(Command command) {
            return (command.Scope != null && command.Scope.PluginGuid != Guid.Empty) ? this.Execute(command) : this.ProxyExecuteCallback(command);
        }

        /// <summary>
        /// A helper function for sending proxy network actions to the server.
        /// </summary>
        /// <remarks>
        ///     <para>The action will be executed with no user specified, so it should always be successful, at least from a permissions point of view.</para>
        /// </remarks>
        /// <param name="action">The action to send to the server.</param>
        /// <returns>The result of the command, check the status for a success message.</returns>
        public CommandResultArgs ProxyNetworkAction(NetworkAction action) {
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

            // Provided we have worked out what they wanted to send..
            if (command != CommandType.None) {
                result = this.ProxyExecute(new Command() {
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

        public override void WriteConfig(Config config) {
            // TODO: Do Config Writing here.
            // Should look something like this (stolen from LocalInterface):
            //
            // foreach (Connection connection in this.Connections)
            //  xNamespace.Add(new XElement("command",
            //      new XAttribute("name", CommandName.ConnectionsAddConnection), // gametype, hostname, port, password, additional
            //      new XElement("gametype",   connection.GameType),
            //      new XElement("hostname",   connection.Hostname),
            //      new XElement("port",       connection.Port),
            //      new XElement("password",   connection.Password),
            //      new XElement("additional", connection.Additional)
            //  ));
        }

        public void LoadConfig() {
            this.GenericEvent(new GenericEventArgs() {
                GenericEventType = GenericEventType.ConfigLoading
            });

            this.PluginConfig = new Config().LoadDirectory(ConfigDirectoryInfo);
            this.Execute(PluginConfig);

            this.GenericEvent(new GenericEventArgs() {
                GenericEventType = GenericEventType.ConfigLoaded
            });
        }

        public virtual void GameEvent(GameEventArgs e) {
            this.GameState = e.GameState;
        }

        public virtual void ClientEvent(ClientEventArgs e) {
        }

        /// <summary>
        /// This method will be called when a text command has been successfully executed.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        [CommandAttribute(CommandType = CommandType.TextCommandsExecute, CommandAttributeType = CommandAttributeType.Executed)]
        public CommandResultArgs TextCommandExecuted(Command command, String text) {

            if (command.Result.Status == CommandResultType.Success && command.Result.Now.TextCommands.First().PluginUid == this.PluginGuid.ToString()) {
                this.Execute(new Command() {
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

        protected virtual void GenericEventTypeConfigSetup(GenericEventArgs e) {
            this.LoadConfig();
        }

        protected virtual void GenericEventTypeTextCommandExecuted(GenericEventArgs e) {
            this.Execute(new Command() {
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
            if (e.GenericEventType == GenericEventType.ConfigSetup) {
                this.GenericEventTypeConfigSetup(e);
            }
            else if (e.GenericEventType == GenericEventType.TextCommandExecuted) {
                this.GenericEventTypeTextCommandExecuted(e);
            }
        }
    }
}