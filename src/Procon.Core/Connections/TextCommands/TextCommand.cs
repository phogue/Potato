using System;
using System.Collections.Generic;

namespace Procon.Core.Connections.TextCommands {

    [Serializable]
    public sealed class TextCommand : IDisposable {

        /// <summary>
        /// The Uid of the plugin.  This is set by the PluginController automatically
        /// if blank, otherwise left open for the plugin to designate another
        /// plugin as the owner of the command.
        /// </summary>
        public String PluginUid { get; set; }

        /// <summary>
        /// The method to execute within the registered uid's plugin
        /// 
        /// e.g "KickCallback" will execute public void KickCallback(...);
        /// </summary>
        public String PluginCommand { get; set; }

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
        public List<String> Commands { get; set; }

        /// <summary>
        /// The priority of the command to be matched against other commands.
        /// 
        /// Defaults to 0.  This setting should only be raised if you
        /// are writing a command to influence other commands.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// A key to be used in a localization lookup for the registered Uid's
        /// localization file, if available.
        /// 
        /// TODO: Change to "Description" object with a variable designating
        /// this as a look up key or the literal text to output.
        /// </summary>
        public String DescriptionKey { get; set; }

        public TextCommand() {
            this.PluginUid = String.Empty;
            this.PluginCommand = String.Empty;
            this.DescriptionKey = String.Empty;

            this.Parser = ParserType.Fuzzy;

            this.Commands = new List<String>();

            this.Priority = 0;
        }

        /// <summary>
        /// This just makes the command inert
        /// </summary>
        public void Dispose() {
            this.PluginUid = null;
            this.PluginCommand = null;
            this.DescriptionKey = null;

            this.Parser = ParserType.Fuzzy;

            this.Commands.Clear();
            this.Commands = null;
        }
    }
}
