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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Procon.Core.Interfaces.Security {
    using Procon.Net.Utils;
    using Procon.Core.Interfaces.Layer;
    using Procon.Core.Interfaces.Layer.Objects;
    using Procon.Core.Interfaces.Security.Objects;

    public class RemoteSecurityController : SecurityController
    {
        #region Executable

        /// <summary>
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public override SecurityController Execute()
        {
            return base.Execute();
        }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        public override void Dispose() { }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        protected override void WriteConfig(XElement config) { }

        #endregion

        public RemoteSecurityController Synchronize(SecurityController securityController) {

            this.Accounts.Clear();
            foreach (Account account in securityController.Accounts) {
                this.Accounts.Add(
                    new RemoteAccount() {
                        Username = account.Username,
                        Password = account.Password, // Erm, of course it'll be blank but it's here to ensure that.
                        Assignments = account.Assignments,
                        Layer = this.Layer
                    }.Execute()
                );
            }

            this.Groups.Clear();
            foreach (Group group in securityController.Groups) {
                this.Groups.Add(
                    new RemoteGroup() {
                        Name = group.Name,
                        Permissions = group.Permissions,
                        AssignedAccounts = new List<Account>(this.Accounts.Where(x => group.AssignedAccounts.Select(z => z.Username).Contains(x.Username))),
                        Layer = this.Layer
                    }.Execute()
                );
            }

            return this;
        }

        [Command(Event = EventName.SecurityGroupsAdded)]
        protected void GroupAdded(CommandInitiator initiator, Group group) {
            this.Groups.Add(
                new RemoteGroup() {
                    Name = group.Name,
                    Permissions = group.Permissions,
                    AssignedAccounts = group.AssignedAccounts,
                    Layer = this.Layer
                }.Execute()
            );
        }

        [Command(Event = EventName.SecurityGroupsRemoved)]
        protected void GroupRemoved(CommandInitiator initiator, Group group) {
            this.Groups.RemoveAll(x => x.Name == group.Name);
        }

        [Command(Event = EventName.SecurityAccountsAdded)]
        protected void AccountAdded(CommandInitiator initiator, Account account) {
            this.Accounts.Add(
                new RemoteAccount() {
                    Username = account.Username,
                    Password = account.Password, // Erm, of course it'll be blank but it's here to ensure that.
                    Assignments = account.Assignments,
                    Layer = this.Layer
                }.Execute()
            );
        }

        [Command(Event = EventName.SecurityAccountsRemoved)]
        protected void AccountRemoved(CommandInitiator initiator, Account account) {
            this.Accounts.RemoveAll(x => x.Username == account.Username);

            foreach (Group group in this.Groups) {
                group.AssignedAccounts.RemoveAll(x => x.Username == account.Username);
            }
        }

        [Command(Event = EventName.SecurityGroupsAccountAssigned)]
        protected void AccountAssignedToGroup(CommandInitiator initiator, string groupName, string username) {
            Group group = this.Groups.Where(x => x.Name == groupName).FirstOrDefault();
            Account account = this.Accounts.Where(x => x.Username == username).FirstOrDefault();

            if (group != null && account != null && group.AssignedAccounts.Where(x => x.Username == username).FirstOrDefault() == null) {
                group.AssignedAccounts.Add(account);
            }
        }

        [Command(Event = EventName.SecurityGroupsAccountRevoked)]
        protected void AccountRevokedFromGroup(CommandInitiator initiator, string groupName, string username) {
            Group group = this.Groups.Where(x => x.Name == groupName).FirstOrDefault();

            if (group != null) {
                group.AssignedAccounts.RemoveAll(x => x.Username == username);
            }
        }

        public override void AddGroup(CommandInitiator initiator, string name) {
            this.Layer.Request(
                new Layer.Objects.Context() {
                    ContextType = ContextType.All
                },
                CommandName.SecurityGroupsAddGroup,
                EventName.None,
                name
            );
        }

        public override void RemoveGroup(CommandInitiator initiator, string name) {
            this.Layer.Request(
                new Layer.Objects.Context() {
                    ContextType = ContextType.All
                },
                CommandName.SecurityGroupsRemoveGroup,
                EventName.None,
                name
            );
        }

        public override void AddAccount(CommandInitiator initiator, string username) {
            this.Layer.Request(
                new Layer.Objects.Context() {
                    ContextType = ContextType.All
                },
                CommandName.SecurityAccountsAddAccount,
                EventName.None,
                username
            );
        }

        public override void RemoveAccount(CommandInitiator initiator, string username) {
            this.Layer.Request(
                new Layer.Objects.Context() {
                    ContextType = ContextType.All
                },
                CommandName.SecurityAccountsRemoveAccount,
                EventName.None,
                username
            );
        }

        public override void AssignAccountToGroup(CommandInitiator initiator, string groupName, string username) {
            this.Layer.Request(
                new Layer.Objects.Context() {
                    ContextType = ContextType.All
                },
                CommandName.SecurityGroupsAssignAccount,
                EventName.None,
                groupName,
                username
            );
        }

        public override void RevokeAccountFromGroup(CommandInitiator initiator, string groupName, string username) {
            this.Layer.Request(
                new Layer.Objects.Context() {
                    ContextType = ContextType.All
                },
                CommandName.SecurityGroupsRevokeAccount,
                EventName.None,
                groupName,
                username
            );
        }
    }
}
