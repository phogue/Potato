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

namespace Procon.Core.Shared.Models {

    /// <summary>
    /// A text command to find text matches against.
    /// </summary>
    [Serializable]
    public sealed class TextCommandModel : CoreModel, IDisposable {
        /// <summary>
        /// The Guid of the plugin.  This is set by the PluginController automatically
        /// if empty, otherwise left open for the plugin to designate another
        /// plugin as the owner of the command.
        /// </summary>
        public Guid PluginGuid { get; set; }

        /// <summary>
        /// The command to send within the scope of the registered uid's plugin e.g "KickCommand"
        /// </summary>
        public String PluginCommand { get; set; }

        /// <summary>
        /// What type of matching to use on this command.
        /// </summary>
        /// <remarks>
        /// <para>Note only fuzzy is available at the moment, backwards
        /// compatible command matching may or may not be added in the future.</para>
        /// <para>Fuzzy is set by default, but should br specified explicitly.</para>
        /// </remarks>
        public TextCommandParserType Parser { get; set; }

        /// <summary>
        /// The priority of the command to be matched against other commands.
        /// </summary>
        /// <remarks>Defaults to 0.  This setting should only be raised if you
        /// are writing a command to influence other commands.</remarks>
        public int Priority { get; set; }

        /// <summary>
        /// The name of the command e.g "Kill"
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// A short description of the command explaining what it will do.
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// List of commands to find in the string. "Kick", "Get rid of", "gtfo", "cya"
        /// </summary>
        public List<String> Commands { get; set; }

        /// <summary>
        /// sets a default "empty" text command to match against.
        /// </summary>
        public TextCommandModel() {
            this.PluginGuid = Guid.Empty;
            this.PluginCommand = String.Empty;
            this.Description = String.Empty;

            this.Parser = TextCommandParserType.Fuzzy;

            this.Commands = new List<String>();

            this.Priority = 0;
        }

        /// <summary>
        /// This just makes the command inert
        /// </summary>
        public void Dispose() {
            this.PluginGuid = Guid.Empty;
            this.PluginCommand = null;
            this.Description = null;

            this.Parser = TextCommandParserType.Fuzzy;

            this.Commands.Clear();
            this.Commands = null;
        }
    }
}
