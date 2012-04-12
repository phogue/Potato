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

namespace Procon.Core.Interfaces.Security {
    using Procon.Core.Interfaces.Layer.Objects;
    using Procon.Core.Interfaces.Security.Objects;

    public class LocalSecurityController : SecurityController
    {
        #region Executable

        /// <summary>
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public override SecurityController Execute()
        {
            AssignEvents();
            return base.Execute();
        }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        public override void Dispose() { }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        protected override void WriteConfig(XElement config, ref FileInfo xFile)
        {
            base.WriteConfig(config, ref xFile);

            foreach (Account account in this.Accounts)
                config.Add(new XElement("command",
                    new XAttribute("name",   CommandName.SecurityAccountsAddAccount),
                    new XElement("username", account.Username)
                ));

            foreach (Group group in this.Groups)
            {
                config.Add(new XElement("command",
                    new XAttribute("name", CommandName.SecurityGroupsAddGroup),
                    new XElement("name",   group.Name)
                ));
                foreach (Account account in group.AssignedAccounts)
                    config.Add(new XElement("command",
                        new XAttribute("name",    CommandName.SecurityGroupsAssignAccount),
                        new XElement("groupName", group.Name),
                        new XElement("username",  account.Username)
                    ));
            }
        }

        #endregion



        /// <summary>
        /// Assigns events to be handled by this class.
        /// </summary>
        protected override void AssignEvents()
        {
            base.AssignEvents();
            GroupAdded        += Groups_GroupAdded;
            GroupRemoved      += Groups_GroupRemoved;
            AssignmentAdded   += Assignments_AssignmentAdded;
            AssignmentRemoved += Assignments_AssignmentRemoved;
            AccountAdded      += Accounts_AccountAdded;
            AccountRemoved    += Accounts_AccountRemoved;
        }

        /// <summary>
        /// Lets the layer know a group has been added.
        /// </summary>
        private void Groups_GroupAdded(SecurityController parent, Group item)
        {
            Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.None,
                EventName.SecurityGroupsAdded,
                item
            );
        }

        /// <summary>
        /// Lets the layer know a group has been removed.
        /// </summary>
        private void Groups_GroupRemoved(SecurityController parent, Group item)
        {
            Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.None,
                EventName.SecurityGroupsRemoved,
                item
            );
        }

        /// <summary>
        /// Lets the layer know an account has been assigned to a group.
        /// </summary>
        private void Assignments_AssignmentAdded(SecurityController parent, Group group, Account account)
        {
            Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.None,
                EventName.SecurityGroupsAccountAssigned,
                group,
                account
            );
        }

        /// <summary>
        /// Lets the layer know an account has been removed from a group.
        /// </summary>
        private void Assignments_AssignmentRemoved(SecurityController parent, Group group, Account account)
        {
            Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.None,
                EventName.SecurityGroupsAccountRevoked,
                group,
                account
            );
        }

        /// <summary>
        /// Lets the layer know an account has been added.
        /// </summary>
        private void Accounts_AccountAdded(SecurityController parent, Account item)
        {
            Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.None,
                EventName.SecurityAccountsAdded,
                item
            );
        }

        /// <summary>
        /// Lets the layer know an account has been removed.
        /// Also removes the account from all the groups it is a part of.
        /// </summary>
        private void Accounts_AccountRemoved(SecurityController parent, Account item)
        {
            Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.None,
                EventName.SecurityAccountsRemoved,
                item
            );
            foreach (Group group in Groups)
                group.AssignedAccounts.RemoveAll(x => x.Username == item.Username);
        }



        /// <summary>
        /// Creates a new group if the specified name is unique.
        /// </summary>
        [Command(Command = CommandName.SecurityGroupsAddGroup)]
        public override void AddGroup(CommandInitiator initiator, String name)
        {
            if (initiator.CommandOrigin == CommandOrigin.Remote && !Can(Account(initiator.Username), initiator.Command))
                return;

            if (name.Length > 0 && Groups.Where(x => x.Name == name).FirstOrDefault() == null) {
                Group group = new LocalGroup() {
                    Name     = name,
                    Layer    = Layer,
                    Security = this
                }.Execute();
                Groups.Add(group);
                OnGroupAdded(this, group);
            }
        }

        /// <summary>
        /// Removes the group whose name is specified.
        /// </summary>
        [Command(Command = CommandName.SecurityGroupsRemoveGroup)]
        public override void RemoveGroup(CommandInitiator initiator, String name)
        {
            if (initiator.CommandOrigin == CommandOrigin.Remote && !Can(Account(initiator.Username), initiator.Command))
                return;

            Group group = Groups.Where(x => x.Name == name).FirstOrDefault();
            if (group != null) {
                Groups.Remove(group);
                OnGroupRemoved(this, group);
            }
        }

        /// <summary>
        /// Places an account into a group.
        /// </summary>
        [Command(Command = CommandName.SecurityGroupsAssignAccount)]
        public override void AssignAccountToGroup(CommandInitiator initiator, String groupName, String username)
        {
            if (initiator.CommandOrigin == CommandOrigin.Remote && !Can(Account(initiator.Username), initiator.Command))
                return;

            Group   group   = Groups.Where(x => x.Name == groupName).FirstOrDefault();
            Account account = Accounts.Where(x => x.Username == username).FirstOrDefault();
            if (group != null && account != null && group.AssignedAccounts.Where(x => x.Username == username).FirstOrDefault() == null) {
                group.AssignedAccounts.Add(account);
                OnAssignmentAdded(this, group, account);
            }
        }

        /// <summary>
        /// Removes an account from a group.
        /// </summary>
        [Command(Command = CommandName.SecurityGroupsRevokeAccount)]
        public override void RevokeAccountFromGroup(CommandInitiator initiator, String groupName, String username)
        {
            if (initiator.CommandOrigin == CommandOrigin.Remote && !Can(Account(initiator.Username), initiator.Command))
                return;

            Group group = Groups.Where(x => x.Name == groupName).FirstOrDefault();
            if (group != null) {
                Account account = group.AssignedAccounts.Where(x => x.Username == username).FirstOrDefault();
                if (account != null) {
                    group.AssignedAccounts.Remove(account);
                    OnAssignmentRemoved(this, group, account);
                }
            }
        }

        /// <summary>
        /// Creates a new account if the specified name is unique.
        /// </summary>
        [Command(Command = CommandName.SecurityAccountsAddAccount)]
        public override void AddAccount(CommandInitiator initiator, String username)
        {
            if (initiator.CommandOrigin == CommandOrigin.Remote && !Can(Account(initiator.Username), initiator.Command))
                return;

            if (username.Length > 0 && Accounts.Where(x => x.Username == username).FirstOrDefault() == null) {
                Account account = new LocalAccount() {
                    Username = username,
                    Layer    = Layer,
                    Security = this
                }.Execute();
                Accounts.Add(account);
                OnAccountAdded(this, account);
            }
        }

        /// <summary>
        /// Removes the account whose name is specified.
        /// </summary>
        [Command(Command = CommandName.SecurityAccountsRemoveAccount)]
        public override void RemoveAccount(CommandInitiator initiator, String username)
        {
            if (initiator.CommandOrigin == CommandOrigin.Remote && !Can(Account(initiator.Username), initiator.Command))
                return;

            Account account = Accounts.Where(x => x.Username == username).FirstOrDefault();
            if (account != null) {
                Accounts.Remove(account);
                OnAccountRemoved(this, account);
            }
        }
    }
}
