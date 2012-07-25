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

namespace Procon.Core.Interfaces.Connections.TextCommands {

    [Serializable]
    public class TextCommand {

        /// <summary>
        /// The Uid of the plugin.  This is set by the PluginController automatically
        /// if blank, otherwise left open for the plugin to designate another
        /// plugin as the owner of the command.
        /// </summary>
        public string UidCallback { get; set; }

        /// <summary>
        /// The method to execute within the registered uid's plugin
        /// 
        /// e.g "KickCallback" will execute public void KickCallback(...);
        /// </summary>
        public string MethodCallback { get; set; }

        /// <summary>
        /// What type of matching to use on this command.
        /// 
        /// Note only NLP is available at the moment, backwards
        /// compatible command matching may or may not be added in the future.
        /// 
        /// NLP is set by default.
        /// </summary>
        public ParserType Parser { get; set; }

        /// <summary>
        /// List of commands to find in the string.
        /// 
        /// "Kick", "Get rid of", "gtfo", "cya"
        /// </summary>
        public List<string> Commands { get; set; }

        /// <summary>
        /// The priority of the command to be matched against other commands.
        /// 
        /// Defaults to Medium.  This setting should only be raised if you
        /// are writing a command to influence other commands.
        /// </summary>
        public PriorityType Priority { get; set; }

        /// <summary>
        /// A key to be used in a localization lookup for the registered Uid's
        /// localization file, if available.
        /// 
        /// TODO: Change to "Description" object with a variable designating
        /// this as a look up key or the literal text to output.
        /// </summary>
        public string DescriptionKey { get; set; }

        /// <summary>
        /// Who the command is available to.  Noone, Everyone (default) or
        /// Account (they must at least have an account to execute the function)
        /// 
        /// SecurityChecks on the method are done during execution within the plugin.
        /// </summary>
        public SecurityIntersectionType SecurityIntersection { get; set; }

        public TextCommand() {
            this.UidCallback = String.Empty;
            this.MethodCallback = String.Empty;
            this.DescriptionKey = String.Empty;

            this.SecurityIntersection = TextCommands.SecurityIntersectionType.All;

            this.Parser = TextCommands.ParserType.NLP;

            this.Commands = new List<string>();

            this.Priority = PriorityType.Medium;
        }
    }
}
