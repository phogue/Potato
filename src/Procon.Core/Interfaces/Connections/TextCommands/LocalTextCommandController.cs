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
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Text;
using System.Reflection;

namespace Procon.Core.Interfaces.Connections.TextCommands {
    using Procon.Core.Interfaces.Connections.TextCommands.Parsers;
    using Procon.NLP;
    using Procon.NLP.Utils;
    using Procon.NLP.Tokens.Object;
    using Procon.NLP.Tokens.Object.Sets;
    using Procon.NLP.Tokens.Primitive;
    using Procon.NLP.Tokens.Primitive.Numeric;
    using Procon.NLP.Tokens.Primitive.Temporal;
    using Procon.NLP.Tokens.Reduction;
    using Procon.Core.Localization;
    using Procon.Core.Interfaces.Variables;
    using Procon.Core.Interfaces.Connections;
    using Procon.Core.Interfaces.Security.Objects;
    using Procon.Net;
    using Procon.Net.Protocols;
    using Procon.Net.Protocols.Objects;

    public class LocalTextCommandController : TextCommandController {

        /// <summary>
        /// All linq commands require the exact same object in order to compile
        /// </summary>
        public Dictionary<Type, ParameterExpression> LinqParameterExpressions { get; set; }

        /// <summary>
        /// All of the available language files to pick from. Used to lookup the users
        /// language and pass on that.
        /// </summary>
        public LanguageController Languages { get; set; }

        public LocalTextCommandController() {
            this.LinqParameterExpressions = new Dictionary<Type, ParameterExpression>() {
                { typeof(Player), Expression.Parameter(typeof(Player), "p") },
                { typeof(Map), Expression.Parameter(typeof(Map), "m") }
            };
        }

        #region Execution

        /// <summary>
        /// Checks if a prefix is an allowed prefix
        /// </summary>
        /// <param name="prefix">The prefix to check (e.g !, @ etc.)</param>
        /// <returns>The parameter prefix, or null if the prefix is invalid</returns>
        private string GetValidPrefix(string prefix) {

            string result = null;

            if (prefix == this.Connection.Variables.Get<String>(CommandInitiator.Local, CommonVariableNames.TextCommandPublicPrefix) ||
                prefix == this.Connection.Variables.Get<String>(CommandInitiator.Local, CommonVariableNames.TextCommandProtectedPrefix) ||
                prefix == this.Connection.Variables.Get<String>(CommandInitiator.Local, CommonVariableNames.TextCommandPrivatePrefix)) {
                result = prefix;
            }

            return result;
        }

        protected override void AssignEvents() {
            this.Connection.GameEvent += new Game.GameEventHandler(Connection_GameEvent);
        }

        private void Connection_GameEvent(Game sender, GameEventArgs e) {
            if (e.EventType == GameEventType.Chat) {

                // At least has the first prefix character 
                // and a little something-something to pass to
                // the parser.
                if (e.Chat.Text.Length >= 2) {

                    String prefix = e.Chat.Text.First().ToString();
                    String text = e.Chat.Text.Remove(0, 1);

                    if ((prefix = this.GetValidPrefix(prefix)) != null) {
                        this.Parse(
                            e.Chat.Author,
                            this.Connection.Security.Account(this.Connection.GameType, e.Chat.Author.UID),
                            prefix,
                            text
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Executes a text command using the NLP parser.
        /// </summary>
        /// <param name="speaker">The player executing the command</param>
        /// <param name="speakerAccount">The account executing the command</param>
        /// <param name="commands">A list of commands to check against </param>
        /// <param name="prefix">The first valid character of the command being executed</param>
        /// <param name="text">The next, minus the first character</param>
        protected void ParseNLP(Player speaker, Account speakerAccount, List<TextCommand> commands, String prefix, String text, TextCommandEventType eventType = TextCommandEventType.Matched) {
            Language selectedLanguage = null;
            if (speakerAccount != null && speakerAccount.PreferredLanguageCode != String.Empty) {
                selectedLanguage = this.Languages.Languages.Find(x => x.LanguageCode == speakerAccount.PreferredLanguageCode);
            }
            else {
                selectedLanguage = this.Languages.Default;
            }

            if (selectedLanguage != null) {
                NLP parser = new NLP() {
                    Connection = this.Connection,
                    TextCommands = commands,
                    Document = selectedLanguage.Root,
                    LinqParameterExpressions = this.LinqParameterExpressions,
                    Speaker = speaker,
                    SpeakerAccount = speakerAccount
                };

                TextCommandEventArgs textEvent = parser.BuildEvent(prefix, text, eventType);

                if (textEvent != null) {
                    this.ThrowTextCommandEvent(textEvent);
                }
            }
        }

        /// <summary>
        /// Runs through the various parsers for all of the text commands.
        /// 
        /// This method may fire multiple events to execute multiple commands
        /// when more than one parser is included in the future. This is expected
        /// behaviour.
        /// </summary>
        /// <param name="speaker"></param>
        /// <param name="speakerAccount"></param>
        /// <param name="prefix"></param>
        /// <param name="text"></param>
        /// <param name="eventType"></param>
        protected void Parse(Player speaker, Account speakerAccount, String prefix, String text, TextCommandEventType eventType = TextCommandEventType.Matched) {
            
            // This could execute more in the future, in which case
            // this.TextCommands.Where(x => x.Parser == Parser.NLP).ToList()
            // would be passed to ExecuteNLP
            this.ParseNLP(speaker, speakerAccount, this.TextCommands, prefix, text, eventType);

        }

        /// <summary>
        /// Fetches a player in the current game connection that is
        /// asociated with the account executing this command.
        /// 
        /// This is used so an account over a layer can issue a command like
        /// "kick me", but we only know "me" from the context of the account
        /// issuing the command. We use this to fetch the player in the game
        /// so we know who to kick.
        /// </summary>
        /// <param name="speaker"></param>
        /// <returns></returns>
        private Player GetSpeakerAccountsPlayer(Account speaker) {
            Player player = null;

            if (speaker != null) {
                AccountAssignment assignment = speaker.Assignments.Where(p => p.GameType == this.Connection.GameType).FirstOrDefault();

                if (assignment != null) {
                    player = this.Connection.GameState.PlayerList.Where(x => x.UID == assignment.UID).FirstOrDefault();
                }
            }

            return player;
        }
        
        /// <summary>
        /// Parses then fires an event to execute a text command
        /// </summary>
        /// <param name="initiator"></param>
        /// <param name="text"></param>
        [Command(Command = CommandName.TextCommandsParse)]
        public void ParseTextCommand(CommandInitiator initiator, String text) {
            Account speakerAccount = this.Connection.Security.Account(initiator.Username);

            String prefix = this.Connection.Variables.Get<String>(CommandInitiator.Local, CommonVariableNames.TextCommandPublicPrefix);
            
            this.Parse(this.GetSpeakerAccountsPlayer(speakerAccount), speakerAccount, prefix, text, TextCommandEventType.Matched);
        }
        
        /// <summary>
        /// Parses then fires an event to preview the results of a text command.
        /// 
        /// Essentially does everything that parsing does, but fires a different event.
        /// </summary>
        /// <param name="initiator"></param>
        /// <param name="text"></param>
        [Command(Command = CommandName.TextCommandsPreview)]
        public void PreviewTextCommand(CommandInitiator initiator, String text) {
            Account speakerAccount = this.Connection.Security.Account(initiator.Username);

            String prefix = this.Connection.Variables.Get<String>(CommandInitiator.Local, CommonVariableNames.TextCommandPublicPrefix);
            
            this.Parse(this.GetSpeakerAccountsPlayer(speakerAccount), speakerAccount, prefix, text, TextCommandEventType.Previewed);
        }
    
        #endregion
    }
}
