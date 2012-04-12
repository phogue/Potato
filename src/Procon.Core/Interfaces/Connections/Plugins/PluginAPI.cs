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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Procon.Core.Interfaces.Connections.Plugins {
    using Procon.Core.Interfaces.Connections.NLP;
    using Procon.Core.Interfaces.Connections.Plugins.Variables;
    using Procon.Core.Interfaces.Security;
    using Procon.Core.Interfaces.Security.Objects;
    using Procon.Core.Scheduler;
    using Procon.Core.Utils;
    using Procon.Net;
    using Procon.Net.Protocols.Objects;
    using Procon.Net.Utils.PunkBuster;
    using Procon.Net.Utils.PunkBuster.Objects;

    public abstract class PluginAPI : Executable<PluginAPI>, IPluginAPI {

        public Config PluginConfig { get; protected set; }

        /// <summary>
        /// Tasks running on a seperate thread for this instance of the plugin.
        /// </summary>
        public TaskController Tasks { get; protected set; }

        /// <summary>
        /// The hostname of the current game server
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// The port of the current game server
        /// </summary>
        public ushort Port { get; set; }

        /// <summary>
        /// The version of Procon.Core running
        /// </summary>
        public System.Version ProconVersion { get; set; }

        /// <summary>
        /// The friendly name of the plugin
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The name of the author(s) of the plugin
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The main website of the plugin or author
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// A detailed description of the plugin
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// List of variables with this plugin.
        /// 
        /// Note that procon willgo through SetPluginVariable
        /// and GetPluginVariables and not directly edit this list.
        /// </summary>
        protected List<Variable> Variables { get; set; }

        /// <summary>
        /// Path to the config-file directory of the plugin
        /// </summary>
        //public string ConfigDir { get; set; }

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

        public PluginAPI()
            : base() {
            this.Tasks = new TaskController().Start();
            this.Variables = new List<Variable>();
        }

        ~PluginAPI() {
            this.Tasks.Stop();

            //throw new NotImplementedException();
            //this.SaveConfig();
        }

        #region Callbacks

        public delegate void ProtocolActionHandler(ProtocolObject action, SecurityCheck check);
        public ProtocolActionHandler ProtocolActionCallback { get; set; }

        public delegate Object ExecuteHandler(List<string> words);
        public ExecuteHandler ExecuteCallback { get; set; }

        public delegate void TextCommandHandler(TextCommand command);
        public TextCommandHandler RegisterTextCommandCallback { get; set; }
        public TextCommandHandler UnregisterTextCommandCallback { get; set; }

        public delegate string LocHandler(string languageCode, string @namespace, string key, params object[] args);
        public LocHandler LocCallback { get; set; }

        public delegate string PlayerLocHandler(Player player, string @namespace, string key, params object[] args);
        public PlayerLocHandler PlayerLocCallback { get; set; }
        
        // This may change to an event flag to filter what the plugin gets
        //public delegate void RegisterEventHandler(string eventName);
        //public RegisterEventHandler RegisterGameEvent { get; set; }

        public void Action(ProtocolObject action, SecurityCheck check = null) {
            if (action != null) {
                this.ProtocolActionCallback(action, check);
            }
        }

        /*
        public Object Execute(List<string> words) {
            Object result = null;

            if (words.Count > 0) {
                result = this.ExecuteCallback(words);
            }

            return result;
        }

        public Object Execute(params string[] words) {
            return this.Execute(words.ToList());
        }
        */
        public void RegisterTextCommand(TextCommand command) {
            if (command != null) {
                this.RegisterTextCommandCallback(command);
            }
        }

        public void UnregisterTextCommand(TextCommand command) {
            if (command != null) {
                this.UnregisterTextCommandCallback(command);
            }
        }

        public string Loc(string languageCode, string key, params object[] args) {
            return this.LocCallback(languageCode, this.GetType().FullName, key, args);
        }

        public string PlayerLoc(Player player, string key, params object[] args) {
            return this.PlayerLocCallback(player, this.GetType().FullName, key, args);
        }

        public string NamespaceLoc(string languageCode, string @namespace, string key, params object[] args) {
            return this.LocCallback(languageCode, @namespace, key, args);
        }

        public string NamespacePlayerLoc(Player player, string @namespace, string key, params object[] args) {
            return this.PlayerLocCallback(player, @namespace, key, args);
        }

        #endregion

        #region Core

        /// <summary>
        /// This is so only one call needs to be made across the AppDomain
        /// to collect all the information about this plugin.
        /// </summary>
        public PluginDetails PluginDetails {
            get {
                return new PluginDetails() {
                    ClassName = this.GetType().Name,
                    Name = this.Name,
                    //Version = System.Reflection.Assembly.GetAssembly(this.GetType()).GetName().Version,
                    Author = this.Author,
                    Website = this.Website,
                    Description = this.Description,
                    PluginVariables = this.GetPluginVariables()
                };
            }
        }

        public virtual void SetPluginVariable(Variable variable) {

            throw new NotImplementedException();
            //this.SaveConfig();
        }

        protected virtual List<Variable> GetPluginVariables() {
            return this.Variables;
        }

        #endregion

        #region Events

        public virtual void PluginEvent(PluginEventArgs e) {

            if (e.EventType == PluginEventType.ConfigSetup) {
                this.PluginEvent(
                    new PluginEventArgs() {
                        EventType = PluginEventType.ConfigLoading
                    }
                );

                this.Execute();

                this.PluginEvent(
                    new PluginEventArgs() {
                        EventType = PluginEventType.ConfigLoaded
                    }
                );
            }

        }

        public override PluginAPI Execute() {

            return base.Execute();
        }

        protected override void WriteConfig(XElement xNamespace, ref FileInfo xFile)
        {
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

            xFile = new FileInfo(Path.Combine(ConfigDirectoryInfo.FullName, String.Format("{0}.xml", GetType().Name)));
        }

        public void LoadConfig() {

            PluginConfig = new Config().LoadDirectory(ConfigDirectoryInfo);
            Execute(PluginConfig);
        }

        public virtual void GameEvent(GameEventArgs e) {
            this.GameState = e.GameState;
        }

        public virtual void ClientEvent(ClientEventArgs e) {

        }

        public virtual void TextCommandEvent(TextCommandEventArgs e) {

        }

        #endregion

        #region Config Loaded/Saving
        /*
        /// <summary>
        /// Saves the config if the outerlock is not set.
        /// 
        /// If the outerlock is set to true then procon or the plugin api has marked
        /// the config as busy and should not be saved (it might be in the middle of loading
        /// for instance - so saving it would be a little confusing!)
        /// </summary>
        protected void SaveConfig() {
            // It won't be written in Procon.Core.Configs.Config.cs because the outer
            // lock is set to true anyway, but this just prevents the execution of
            // ToWords.. just a minor optimization.
            if (this.PluginConfig.OuterLock == false) {
                this.PluginConfig.Set(this.ToWords());
                this.PluginConfig.Write<PluginConfig>();
            }
        }

        private void PluginConfig_ConfigLoaded(Configs.WordedConfig sender) {
            this.PluginConfig.OuterLock = true;

            this.Variables.Clear();

            foreach (List<string> line in sender.Words) {
                this.Execute(new CommandInitiator() { CommandOrigin = CommandOrigin.Local }, line);
            }

            this.PluginConfig.OuterLock = false;
        }
        */
        [Command(Command = CommandName.PluginSetVariable)]
        protected void SetConfigPluginVariable(CommandInitiator initiator, string uid, string jsonVariable) {
            // It should always be local, just sanity checking.
            if (initiator.CommandOrigin == CommandOrigin.Local) {
                this.SetPluginVariable(Connections.Plugins.Variables.Variable.FromJson(jsonVariable));
            }
        }
        
        /// <summary>
        /// You can override this to save additional config settings not covered by CommandName
        /// 
        /// public override List<List<object>> ToWords() {
        ///     List<List<object>> words = base.ToWords();
        ///     
        ///        words.Add(
        ///            new List<object>() {
        ///                "MyConfigSetting",
        ///                "My Setting",
        ///                12345
        ///            }
        ///        );
        /// 
        ///     return words;
        /// }
        /// </summary>
        /// <returns></returns>
        /// 
        /*
        public override List<List<object>> ToWords() {
            List<List<object>> words = new List<List<object>>();

            foreach (Variables.Variable variable in this.PluginDetails.PluginVariables) {
                words.Add(
                    // The plugin is saved in the same format that would be sent via a layer
                    // or if the plugin setting was saved in a connection or instance config.
                    new List<object>() {
                        CommandName.PluginSetVariable,
                        this.GetType().Name,
                        variable.ToJson()
                    }
                );
            }

            return words;
        }
        */
        #endregion

        public void Shutdown() {
            throw new NotImplementedException();
            //this.SaveConfig();
        }
    }
}
