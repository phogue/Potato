// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Newtonsoft.Json;

namespace Procon.Core.Interfaces.Connections.Plugins {
    using Procon.Core.Interfaces.Variables;
    using Procon.Core.Interfaces.Connections.Plugins.Variables;
    using Procon.Core.Interfaces.Connections.TextCommands;
    using Procon.Core.Interfaces.Security;
    using Procon.Core.Interfaces.Security.Objects;
    using Procon.Core.Localization;
    using Procon.Net;
    using Procon.Net.Protocols.Objects;
    using Procon.Core.Utils;

    public class LocalPlugin : Plugin {

        [JsonIgnore]
        public string Path { get; set; }

        [JsonIgnore]
        public PluginLoaderFactory PluginFactory { get; set; }

        [JsonIgnore]
        public SecurityController Security { get; set; }

        [JsonIgnore]
        public Connection Connection { get; set; }
        
        protected IPluginAPI AppDomainPlugin { get; set; }

        [JsonIgnore]
        public IVariableController Variables { get; set; }

        private PluginDetails m_pluginDetails;
        public override PluginDetails PluginDetails {
            get {
                PluginDetails details = this.m_pluginDetails;

                if (this.AppDomainPlugin != null) {
                    this.m_pluginDetails = details = this.AppDomainPlugin.PluginDetails;
                }

                return details;
            }
            set {
                // This is only so the plugin details can be serialized
                this.m_pluginDetails = value;
            }
        }

        public Plugin Read() {

            if (File.Exists(this.Path) == true) {
                this.Uid = new FileInfo(this.Path).Name.Replace(".dll", "");

                try {

                    this.AppDomainPlugin = (IPluginAPI)this.PluginFactory.Create(this.Path, this.GetType().Namespace + "." + this.Uid, null);

                    this.Setup();
                }
                catch (Exception) { }
            }

            return this;
        }

        public override void SetPluginVariable(CommandInitiator initiator, Plugins.Variables.Variable variable) {
            this.AppDomainPlugin.SetPluginVariable(variable);
        }

        protected void Setup() {
            if (this.AppDomainPlugin != null) {

                this.AppDomainPlugin.PluginEvent(
                    new PluginEventArgs() {
                        EventType = PluginEventType.RegisteringCallbacks
                    }
                );

                this.AppDomainPlugin.ProtocolActionCallback = new PluginAPI.ProtocolActionHandler(this.ProtocolAction);

                this.AppDomainPlugin.ExecuteCallback = new PluginAPI.ExecuteHandler(this.Execute);
                this.AppDomainPlugin.LocCallback = new PluginAPI.LocHandler(this.Loc);
                this.AppDomainPlugin.PlayerLocCallback = new PluginAPI.PlayerLocHandler(this.PlayerLoc);

                this.AppDomainPlugin.RegisterTextCommandCallback = new PluginAPI.TextCommandHandler(this.RegisterCommand);
                this.AppDomainPlugin.UnregisterTextCommandCallback = new PluginAPI.TextCommandHandler(this.UnregisterCommand);
                
                this.Connection.ClientEvent += new Game.ClientEventHandler(Connection_ClientEvent);
                this.Connection.GameEvent += new Game.GameEventHandler(Connection_GameEvent);
                this.Connection.TextCommand.TextCommandEvent += new TextCommandController.TextCommandEventHandler(StateNLP_TextCommandEvent);

                this.AppDomainPlugin.ProconVersion = Assembly.GetExecutingAssembly().GetName().Version;

                this.AppDomainPlugin.PluginEvent(
                    new PluginEventArgs() {
                        EventType = PluginEventType.CallbacksRegistered
                    }
                );

                // check the plugin's config directory
                this.AppDomainPlugin.ConfigDirectoryInfo = new DirectoryInfo(System.IO.Path.Combine(Defines.CONFIGS_DIRECTORY, System.IO.Path.Combine(PathValidator.Valdiate(String.Format("{0}_{1}", this.Connection.Hostname, this.Connection.Port)), PathValidator.Valdiate(this.Uid))));
                if (!this.AppDomainPlugin.ConfigDirectoryInfo.Exists) {
                    this.AppDomainPlugin.ConfigDirectoryInfo.Create();
                }

                // Phogue - TODO: Add "config.cfg" to Procon.Core.Defines.  All configs in their own dir's
                // should be named config.cfg
                // Phogue - Note the .cfg extension from .txt, this keeps config files consistently named.
                //this.AppDomainPlugin.PluginConfig.Name = "config.cfg";
                /* Phogue - Note that the PluginConfig object will ensure the directories exist before writing.
                 * See Procon.Core.Configs.Config.cs
                this.AppDomainPlugin.ConfigDir = System.IO.Path.Combine(Defines.CONFIGS_DIRECTORY, System.IO.Path.Combine(PathValidator.Valdiate(String.Format("{0}_{1}", this.Connection.Hostname, this.Connection.Port)), PathValidator.Valdiate(this.Uid)));
                DirectoryInfo configDirectory = new DirectoryInfo(this.AppDomainPlugin.ConfigDir);
                if (!configDirectory.Exists)
                {
                    configDirectory.Create();
                }
                */

                // check the plugin's log directory
                this.AppDomainPlugin.LogDirectoryInfo = new DirectoryInfo(System.IO.Path.Combine(Defines.LOGS_DIRECTORY, System.IO.Path.Combine(PathValidator.Valdiate(String.Format("{0}_{1}", this.Connection.Hostname, this.Connection.Port)), PathValidator.Valdiate(this.Uid))));

                if (!this.AppDomainPlugin.LogDirectoryInfo.Exists) {
                    this.AppDomainPlugin.LogDirectoryInfo.Create();
                }

                // create a standard .txt config-file
                /* Phogue - there isn't any need to init a config.  If useful information isn't being saved, there
                 * isn't any point in saving it.
                this.AppDomainPlugin.ConfigFile = System.IO.Path.Combine(this.AppDomainPlugin.ConfigDir, "config.cfg");
                FileInfo configFile = new FileInfo(this.AppDomainPlugin.ConfigFile);
                if (!configFile.Exists)
                {
                    StreamWriter sw = new StreamWriter(this.AppDomainPlugin.ConfigFile, true);
                    sw.WriteLine("/////////////////////////////////////////////");
                    sw.WriteLine("// Config-file for " + this.Uid + " created by " + this.AppDomainPlugin.Author);
                    sw.WriteLine("/////////////////////////////////////////////");
                    sw.Flush();
                    sw.Close();
                }
                */

                this.AppDomainPlugin.Hostname = this.Connection.Hostname;
                this.AppDomainPlugin.Port = this.Connection.Port;

                this.AppDomainPlugin.PluginEvent(
                    new PluginEventArgs() {
                        EventType = PluginEventType.ConfigSetup
                    }
                );

                this.AppDomainPlugin.LoadConfig();
            }
        }

        private void Connection_ClientEvent(Game sender, ClientEventArgs e) {
            if (this.AppDomainPlugin != null && e.EventType != ClientEventType.PacketReceived && e.EventType != ClientEventType.PacketSent) {
                this.AppDomainPlugin.ClientEvent(e);
            }
        }

        private void Connection_GameEvent(Game sender, GameEventArgs e) {
            if (this.AppDomainPlugin != null) {
                // TODO: For some reason, when serializing classes that have been derived outside
                //       of this .dll, this is throwing an error.  For example, Player will not
                //       throw an error when serialzed, but FrostbitePlayer will.
                try { this.AppDomainPlugin.GameEvent(e); }
                catch (Exception) { }
            }
        }
        
        private void StateNLP_TextCommandEvent(TextCommandController sender, TextCommandEventArgs e) {
            if (this.AppDomainPlugin != null) {
                if (e.EventType == TextCommandEventType.Executed && e.Command.UidCallback == this.Uid) {
                    this.AppDomainPlugin.TextCommandEvent(e);
                }
                else if (e.EventType == TextCommandEventType.Registered || e.EventType == TextCommandEventType.Unregistered) {
                    this.AppDomainPlugin.TextCommandEvent(e);
                }
            }
        }

        public Object Execute(List<string> words) {
            // TODO: Allow plugins to execute commands?  Is this needed?
            return null;
        }

        public void RegisterCommand(TextCommand command) {

            if (command.UidCallback == String.Empty) {
                command.UidCallback = this.Uid;
            }

            TextCommand registeredCommand = this.Connection.TextCommand.TextCommands.Find(x => x.UidCallback == this.Uid && x.MethodCallback == command.MethodCallback);

            if (registeredCommand == null) {
                this.Connection.TextCommand.TextCommands.Add(command);
            }
        }

        public void UnregisterCommand(TextCommand command) {
            this.Connection.TextCommand.TextCommands.RemoveAll(x => x.UidCallback == this.Uid && x.MethodCallback == command.MethodCallback);
        }

        protected string Loc(string languageCode, string @namespace, string key, params object[] args) {
            return ExecutableBase.MasterLanguages.Loc(languageCode, @namespace, key, args);
        }

        protected string PlayerLoc(Player player, string @namespace, string key, params object[] args) {
            Account account = this.Security.Account(this.Connection.GameType, player.UID);
            string languageCode = account != null ? account.PreferredLanguageCode : null;

            return ExecutableBase.MasterLanguages.Loc(languageCode, @namespace, key, args);
        }

        /// <summary>
        /// Intermediary method to fill in the current game name and the permission to be checked.
        /// 
        /// This is mostly so plugin authors can ommit this data (or never even know it)
        /// 
        /// </summary>
        /// <param name="check">The security check sent by the plugin</param>
        /// <param name="permission">The permission to be checked against</param>
        /// <returns></returns>
        private bool Can(SecurityCheck check) {

            bool can = false;

            if (check != null) {
                can = this.Security.Can(check, this.Connection.GameType);
            }
            else {
                can = true;
            }

            return can;
        }

        protected void ProtocolAction(ProtocolObject action, SecurityCheck check = null) {
            if (Can(check) == true) { // && Connection is RemoteConnection) {
                // @todo why the check for remoteconnection?
                this.Connection.Action(action);

                //(Connection as RemoteConnection).Action(action);
            }
        }

        public override void Dispose() {
            if (AppDomainPlugin != null) {
                AppDomainPlugin.Dispose();
            }
        }
    }
}
