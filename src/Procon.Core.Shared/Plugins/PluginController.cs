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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;
using Procon.Net.Shared;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Actions.Deferred;

namespace Procon.Core.Shared.Plugins {
    /// <summary>
    /// The class in which all plugins should inherit from. This class handles all remoting
    /// to Procon and other standard tasks 
    /// </summary>
    public abstract class PluginController : AsynchronousCoreController, IPluginController {
        /// <summary>
        /// A human readable name/title of this plugin.
        /// </summary>
        public String Title { get; set; }

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
        public IProtocolState ProtocolState { get; set; }

        /// <summary>
        /// All actions awaiting responses from the game networking layer
        /// </summary>
        protected ConcurrentDictionary<Guid, IDeferredAction> DeferredActions { get; set; } 

        protected PluginController() : base() {
            this.PluginGuid = this.GetAssemblyGuid();
            this.Title = this.PluginGuid.ToString();

            this.DeferredActions = new ConcurrentDictionary<Guid, IDeferredAction>();

            this.ProtocolState = new ProtocolState();

            this.CommandDispatchers.Add(new CommandDispatch() {
                CommandType = CommandType.TextCommandsExecute,
                CommandAttributeType = CommandAttributeType.Executed,
                ParameterTypes = new List<CommandParameterType>() {
                    new CommandParameterType() {
                        Name = "text",
                        Type = typeof(String)
                    }
                },
                Handler = this.TextCommandExecuted
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

        public override void BeginBubble(ICommand command, Action<ICommandResult> completed = null) {
            // There isn't much point in bubbling up if we just need to come back down here.
            if (command.ScopeModel != null && command.ScopeModel.PluginGuid == this.PluginGuid) {
                base.BeginTunnel(command, completed);
            }
            else if (this.BubbleObjects != null) {
                base.BeginBubble(command, completed);
            }
        }

        public override ICommandResult Bubble(ICommand command) {
            // There isn't much point in bubbling up if we just need to come back down here.
            if (command.ScopeModel != null && command.ScopeModel.PluginGuid == this.PluginGuid) {
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
        public virtual ICommandResult Action(INetworkAction action) {
            return this.Bubble(new Command() {
                Name = action.ActionType.ToString(),
                ScopeModel = new CommandScopeModel() {
                    ConnectionGuid = this.ConnectionGuid
                },
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            NetworkActions = new List<INetworkAction>() {
                                action
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Sets up and sends a deferred action to the server.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public virtual ICommandResult Action(IDeferredAction action) {
            ICommandResult result = null;

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

                config.Save(new FileInfo(Path.Combine(this.ConfigDirectoryInfo.FullName, this.GetType().Namespace + ".json")));
            }
        }

        public override void WriteConfig(IConfig config, String password = null) {
            // Overwrite this method to write out your config
        }

        /// <summary>
        /// Loads the config from this plugins config directory.
        /// </summary>
        /// <remarks>Configs are simply saved commands</remarks>
        public virtual void LoadConfig() {
            this.GenericEvent(new GenericEvent() {
                GenericEventType = GenericEventType.ConfigLoading
            });

            this.Execute(new Config().Load(ConfigDirectoryInfo));

            this.GenericEvent(new GenericEvent() {
                GenericEventType = GenericEventType.ConfigLoaded
            });
        }

        /// <summary>
        /// Fetches a list of available commands handled by this controller and any controllers found in TunnelObjects.
        /// </summary>
        public virtual List<String> HandledCommandNames() {
            var items = new List<ICoreController>() { this };
            var tunneled = items.SelectMany(item => item.TunnelObjects).ToList();

            while (tunneled.Count > 0) {
                items.AddRange(tunneled);

                tunneled = tunneled.SelectMany(item => item.TunnelObjects).ToList();
            }

            return items.SelectMany(item => item.CommandDispatchers).Select(item => item.Name).Distinct().OrderBy(item => item).ToList();
        }

        public IPluginSetupResult Setup(IPluginSetup setup) {
            if (setup.ConnectionGuid != null) {
                this.ConnectionGuid = Guid.Parse(setup.ConnectionGuid);
            }

            if (setup.ConfigDirectoryPath != null) {
                this.ConfigDirectoryInfo = new DirectoryInfo(setup.ConfigDirectoryPath);
                this.ConfigDirectoryInfo.Create();
            }

            if (setup.LogDirectoryPath != null) {
                this.LogDirectoryInfo = new DirectoryInfo(setup.LogDirectoryPath);
                this.LogDirectoryInfo.Create();
            }

            return new PluginSetupResult() {
                Commands = this.HandledCommandNames(),
                Title = this.Title
            };
        }

        public virtual void GameEvent(IProtocolEventArgs e) {
            // Apply any changes to the protocol state for this plugin
            if (e.StateDifference != null) {
                this.ProtocolState.Apply(e.StateDifference);
            }
        }

        public virtual void ClientEvent(IClientEventArgs e) {
            if (e.EventType == ClientEventType.ClientActionDone) {
                INetworkAction doneAction = e.Then.Actions.FirstOrDefault();

                if (doneAction != null) {
                    IDeferredAction deferredAction;

                    if (this.DeferredActions.TryRemove(doneAction.Uid, out deferredAction) == true && deferredAction.TryInsertDone(doneAction, e.Then.Packets, e.Now.Packets) == true) {
                        deferredAction.TryInsertAlways(doneAction);

                        deferredAction.Release();
                    }
                }
            }
            else if (e.EventType == ClientEventType.ClientActionExpired) {
                INetworkAction doneAction = e.Then.Actions.FirstOrDefault();

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
        public virtual ICommandResult TextCommandExecuted(ICommand command, Dictionary<String, ICommandParameter> parameters) {

            // Not used.
            // String text = parameters["text"].First<String>();

            if (command.Result.CommandResultType == CommandResultType.Success && command.Result.Now.TextCommands.First().PluginGuid == this.PluginGuid) {
                this.Tunnel(new Command() {
                    Origin = CommandOrigin.Local,
                    Name = command.Result.Now.TextCommands.First().PluginCommand,
                    Parameters = new List<ICommandParameter>() {
                        new CommandParameter() {
                            Data = {
                                CommandResults = new List<ICommandResult>() {
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
        protected virtual void GenericEventTypePluginLoaded(GenericEvent e) {
            this.LoadConfig();
        }

        /// <summary>
        /// Consider this the plugin destructor
        /// </summary>
        /// <param name="e"></param>
        protected virtual void GenericEventTypePluginUnloading(GenericEvent e) {
            this.WriteConfig();
        }

        /// <summary>
        /// A text command has been executed and it's callback directs to this plugin.
        /// </summary>
        /// <remarks>We convert to a new command and push it through to this plugin,
        /// which just cleans up the plugin implementation a little bit but converting
        /// a plugin executed command to a local command. Snazzified.</remarks>
        /// <param name="e"></param>
        protected virtual void GenericEventTypeTextCommandExecuted(GenericEvent e) {
            this.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                Name = e.Now.TextCommands.First().PluginCommand,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Events = new List<IGenericEvent>() {
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
        public virtual void GenericEvent(GenericEvent e) {
            if (e.GenericEventType == GenericEventType.PluginsLoaded) {
                this.GenericEventTypePluginLoaded(e);
            }
            else if (e.GenericEventType == GenericEventType.PluginsUnloading) {
                this.GenericEventTypePluginUnloading(e);
            }
            else if (e.GenericEventType == GenericEventType.TextCommandExecuted) {
                this.GenericEventTypeTextCommandExecuted(e);
            }
        }
    }
}