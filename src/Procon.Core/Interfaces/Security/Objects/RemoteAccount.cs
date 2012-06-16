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
using System.Xml.Linq;

namespace Procon.Core.Interfaces.Security.Objects {
    using Procon.Core.Interfaces.Layer.Objects;
    using Procon.Net.Protocols;

    public class RemoteAccount : Account
    {
        #region Executable

        /// <summary>
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public override Account Execute()
        {
            return base.Execute();
        }

        /// <summary>
        /// Relies on children classes to implement 
        /// </summary>
        public override void Dispose() { }

        /// <summary>
        /// Relies on children classes to implement 
        /// </summary>
        protected override void WriteConfig(XElement config) { }

        #endregion



        /// <summary>
        /// 
        /// </summary>
        [Command(Event = EventName.SecurityAccountsUidAssigned)]
        protected void Assigned(CommandInitiator initiator, String username, GameType gameType, String uid) {
            if (Username == username) {
                Assignments.Add(
                    new AccountAssignment() {
                        GameType = gameType,
                        UID      = uid
                    }
                );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Command(Event = EventName.SecurityAccountsUidRevoked)]
        protected void Revoked(CommandInitiator initiator, String username, GameType gameType, String uid) {
            if (Username == username) {
                Assignments.RemoveAll(x => x.GameType == gameType && x.UID == uid);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Assign(CommandInitiator initiator, String username, GameType gameType, String uid) {
            Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.SecurityAccountsAssign,
                EventName.None,
                username,
                gameType,
                uid
            );
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Revoke(CommandInitiator initiator, String username, GameType gameType, String uid) {
            Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.SecurityAccountsRevoke,
                EventName.None,
                username,
                gameType,
                uid
            );
        }

        /// <summary>
        /// 
        /// </summary>
        public override void SetPassword(CommandInitiator initiator, String username, String password) {
            Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.SecurityAccountsSetPassword,
                EventName.None,
                username,
                password
            );
        }

        /// <summary>
        /// 
        /// </summary>
        public override void SetPreferredLanguageCode(CommandInitiator initiator, String username, String languageCode) {
            Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.SecurityAccountsSetPreferredLanguageCode,
                EventName.None,
                username,
                languageCode
            );
        }

    }
}
