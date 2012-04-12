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
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Procon.Core.Interfaces.Security.Objects {
    using Procon.Core.Interfaces.Layer.Objects;
    using Procon.Net.Protocols;

    public class LocalAccount : Account
    {
        // Public Objects
        [JsonIgnore]
        public  SecurityController Security {
            get { return mSecurity; }
            set {
                if (mSecurity != value) {
                    mSecurity = value;
                    OnPropertyChanged(this, "Security");
                }
            }
        }
        private SecurityController mSecurity;

        

        #region Executable

        /// <summary>
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public override Account Execute()
        {
            return base.Execute();
        }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        public override void Dispose() { }

        /// <summary>
        /// Saves the password, preferred language, and all the account's assignemnts.
        /// </summary>
        protected override void WriteConfig(XElement config, ref FileInfo xFile)
        {
            base.WriteConfig(config, ref xFile);

            config.Add(new XElement("command",
                new XAttribute("name",   CommandName.SecurityAccountsSetPassword),
                new XElement("username", Username),
                new XElement("password", Password)
            ));

            config.Add(new XElement("command",
                new XAttribute("name",       CommandName.SecurityAccountsSetPreferredLanguageCode),
                new XElement("username",     Username),
                new XElement("languageCode", PreferredLanguageCode)
            ));

            foreach (AccountAssignment assignment in Assignments) {
                config.Add(new XElement("command",
                    new XAttribute("name",   CommandName.SecurityAccountsAssign),
                    new XElement("username", Username),
                    new XElement("gameType", assignment.GameType),
                    new XElement("uid",      assignment.UID)
                ));
            }
        }

        #endregion



        /// <summary>
        /// Assigns events to be handled by this class.
        /// </summary>
        protected override void AssignEvents() {
            base.AssignEvents();
            AssignmentAdded   += Assignments_AssignmentAdded;
            AssignmentRemoved += Assignments_AssignmentRemoved;
        }

        /// <summary>
        /// Notify the layer that an assignment was added.
        /// </summary>
        private void Assignments_AssignmentAdded(Account parent, AccountAssignment item)
        {
            Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.None,
                EventName.SecurityAccountsUidAssigned,
                item
            );
        }

        /// <summary>
        /// Notify the layer that an assignment was removed.
        /// </summary>
        private void Assignments_AssignmentRemoved(Account parent, AccountAssignment item)
        {
            Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.None,
                EventName.SecurityAccountsUidRevoked,
                item
            );
        }


        
        // TODO: Now that the flat files are gone this no longer requires a username parameter.
        [Command(Command = CommandName.SecurityAccountsAssign)]
        public override void Assign(CommandInitiator initiator, String username, GameType gameType, string uid) {
            if (initiator.CommandOrigin == CommandOrigin.Remote && this.Security.Can(this.Security.Account(initiator.Username), initiator.Command, this.Security.Account(username)) == false) {
                return;
            }

            if (this.Username == username) {
                if (this.Assignments.Where(x => x.GameType == gameType && x.UID == uid).FirstOrDefault() == null) {
                    this.Assignments.Add(
                        new AccountAssignment() {
                            GameType = gameType,
                            UID = uid
                        }
                    );
                }
            }
        }

        // TODO: Now that the flat files are gone this no longer requires a username parameter.
        [Command(Command = CommandName.SecurityAccountsRevoke)]
        public override void Revoke(CommandInitiator initiator, string username, GameType gameType, string uid) {
            if (initiator.CommandOrigin == CommandOrigin.Remote && this.Security.Can(this.Security.Account(initiator.Username), initiator.Command, this.Security.Account(username)) == false) {
                return;
            }

            if (this.Username == username) {
                this.Assignments.RemoveAll(x => x.GameType == gameType && x.UID == uid);
            }
        }

        // TODO: Now that the flat files are gone this no longer requires a username parameter.
        [Command(Command = CommandName.SecurityAccountsSetPassword)]
        public override void SetPassword(CommandInitiator initiator, string username, string password) {
            if (initiator.CommandOrigin == CommandOrigin.Remote && this.Security.Can(this.Security.Account(initiator.Username), initiator.Command, this.Security.Account(username)) == false) {
                return;
            }

            if (this.Username == username && password.Length > 0) {
                this.Password = password;
            }
        }

        // TODO: Now that the flat files are gone this no longer requires a username parameter.
        [Command(Command = CommandName.SecurityAccountsSetPreferredLanguageCode)]
        public override void SetPreferredLanguageCode(CommandInitiator initiator, string username, string languageCode) {
            if (initiator.CommandOrigin == CommandOrigin.Remote && this.Security.Can(this.Security.Account(initiator.Username), initiator.Command, this.Security.Account(username)) == false) {
                return;
            }

            if (this.Username == username) {
                this.PreferredLanguageCode = languageCode;

                this.OnPropertyChanged(this, "PreferredLanguageCode");
            }
        }
    }
}
