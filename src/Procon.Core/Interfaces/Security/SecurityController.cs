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
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Procon.Core.Interfaces.Security {
    using Procon.Core.Interfaces.Layer;
    using Procon.Core.Interfaces.Layer.Objects;
    using Procon.Core.Interfaces.Security.Objects;
    using Procon.Net.Protocols;
    using Procon.Net.Utils;

    public abstract class SecurityController : Executable<SecurityController>
    {
        // Public Objects
        public  List<Account> Accounts {
            get { return mAccounts; }
            private set {
                if (mAccounts != value) {
                    mAccounts = value;
                    OnPropertyChanged(this, "Accounts");
                }
            }
        }
        private List<Account> mAccounts;

        public  List<Group> Groups {
            get { return mGroups; }
            private set {
                if (mGroups != value) {
                    mGroups = value;
                    OnPropertyChanged(this, "Groups");
                }
            }
        }
        private List<Group> mGroups;
        
        [JsonIgnore]
        public  ILayer Layer {
            get { return mLayer; }
            set {
                if (mLayer != value) {
                    mLayer = value;
                    OnPropertyChanged(this, "Layer");
                }
            }
        }
        private ILayer mLayer;

        // Base Initialization
        public SecurityController() : base() {
            Accounts = new List<Account>();
            Groups   = new List<Group>();
        }


        
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
        protected override void WriteConfig(XElement config, ref FileInfo xFile) { }

        #endregion
        #region Events

        // -- Adding an Group --
        public delegate void GroupAddedHandler(SecurityController parent, Group item);
        public event         GroupAddedHandler GroupAdded;
        protected void OnGroupAdded(SecurityController parent, Group item)
        {
            if (GroupAdded != null)
                GroupAdded(parent, item);
        }

        // -- Removing an Group --
        public delegate void GroupRemovedHandler(SecurityController parent, Group item);
        public event         GroupRemovedHandler GroupRemoved;
        protected void OnGroupRemoved(SecurityController parent, Group item)
        {
            if (GroupRemoved != null)
                GroupRemoved(parent, item);
        }

        // -- Adding an Assignment --
        public delegate void AssignmentAddedHandler(SecurityController parent, Group group, Account account);
        public event         AssignmentAddedHandler AssignmentAdded;
        protected void OnAssignmentAdded(SecurityController parent, Group group, Account account)
        {
            if (AssignmentAdded != null)
                AssignmentAdded(parent, group, account);
        }

        // -- Removing an Assignment --
        public delegate void AssignmentRemovedHandler(SecurityController parent, Group group, Account account);
        public event         AssignmentRemovedHandler AssignmentRemoved;
        protected void OnAssignmentRemoved(SecurityController parent, Group group, Account account)
        {
            if (AssignmentRemoved != null)
                AssignmentRemoved(parent, group, account);
        }

        // -- Adding an Account --
        public delegate void AccountAddedHandler(SecurityController parent, Account item);
        public event         AccountAddedHandler AccountAdded;
        protected void OnAccountAdded(SecurityController parent, Account item)
        {
            if (AccountAdded != null)
                AccountAdded(parent, item);
        }

        // -- Removing an Account --
        public delegate void AccountRemovedHandler(SecurityController parent, Account item);
        public event         AccountRemovedHandler AccountRemoved;
        protected void OnAccountRemoved(SecurityController parent, Account item)
        {
            if (AccountRemoved != null)
                AccountRemoved(parent, item);
        }

        #endregion



        /// <summary>
        /// Assigns events to be handled by this class.
        /// </summary>
        protected virtual void AssignEvents()
        {
            Layer.ProcessLayerEvent += Layer_ProcessLayerEvent;
        }

        /// <summary>
        /// Executes a command based on events sent across the layer.
        /// </summary>
        private void Layer_ProcessLayerEvent(String username, Context context, CommandName command, EventName @event, Object[] parameters)
        {
            if (context.ContextType == ContextType.All) {
                Execute(
                    new CommandInitiator() {
                        CommandOrigin = CommandOrigin.Remote,
                        Username      = username
                    },
                    new CommandAttribute() {
                        Command = command,
                        Event   = @event
                    },
                    parameters
                );
            }
        }



        /// <summary>
        /// Creates a new group if the specified name is unique.
        /// </summary>
        public abstract void AddGroup(CommandInitiator initiator, String name);

        /// <summary>
        /// Removes the group whose name is specified.
        /// </summary>
        public abstract void RemoveGroup(CommandInitiator initiator, String name);

        /// <summary>
        /// Places an account into a group.
        /// </summary>
        public abstract void AssignAccountToGroup(CommandInitiator initiator, String groupName, String username);

        /// <summary>
        /// Removes an account from a group.
        /// </summary>
        public abstract void RevokeAccountFromGroup(CommandInitiator initiator, String groupName, String username);

        /// <summary>
        /// Creates a new account if the specified name is unique.
        /// </summary>
        public abstract void AddAccount(CommandInitiator initiator, String username);

        /// <summary>
        /// Removes the account whose name is specified.
        /// </summary>
        public abstract void RemoveAccount(CommandInitiator initiator, String username);



        /// <summary>
        /// Given a game, unique identifier, and permission, this method attempts to find an account
        /// who is setup for the game and matches the unique identifier. If found, then it iterates
        /// over all the groups the account is a part of and returns the highest level of authority 
        /// the account has for the specified permission. Returns null if the account is not allowed
        /// to perform the specified permission.
        /// </summary>
        /// <remarks>
        /// This method is used when all we know is the players in game information and we don't know
        /// their account name, or if they even have an account.
        /// </remarks>
        private int? HighestAuthority(AccountAssignment assignment, CommandName permission)
        {
            int? value = null;
            if (assignment != null)
                value = (from account in Accounts
                             from asn in account.Assignments
                                 where asn.GameType == assignment.GameType
                                    && asn.UID      == assignment.UID
                                 from grp in Groups
                                     where grp.AssignedAccounts.Contains(account) == true
                                     from prm in grp.Permissions
                                         where prm.Name == permission
                                         orderby prm.Authority descending
                         select prm.Authority
                        ).FirstOrDefault();

            return value;
        }

        /// <summary>
        /// Given a account username and permission, this method attempts to find an account whose
        /// username matches the one specified. If found, then it iterates over all the groups the
        /// account is a part of and returns the highest level of authority the account has for the
        /// specified permission. Returns null if the account is not allowed to perform the specified
        /// permission.
        /// </summary>
        /// <remarks>
        /// This is generally used for local or remote connections where we must know the initator
        /// has an account and what their username is.
        /// </remarks>
        private int? HighestAuthority(String username, CommandName permission)
        {
            int? value = null;
            value = (from account in this.Accounts
                         where account.Username == username
                         from grp in this.Groups
                             where grp.AssignedAccounts.Contains(account) == true
                             from prm in grp.Permissions
                                 where prm.Name == permission
                                 orderby prm.Authority descending
                     select prm.Authority
                    ).FirstOrDefault();

            return value;
        }

        /// <summary>
        /// Returns true if the intitiator's authority is higher than the target's authority
        /// OR the initiator's authority is above zero and the target's authority is null.
        /// </summary>
        private bool HasAuthority(int? initiator_authority, int? target_authority)
        {
            return (initiator_authority.HasValue) ? 
                       (target_authority.HasValue) ? 
                           (initiator_authority > target_authority) : initiator_authority > 0 : false;
        }

        /// <summary>
        /// Returns true if the initiator's authority is higher than the target's authority.
        /// </summary>
        public bool Can(SecurityCheck check, GameType gameType)
        {
            return HasAuthority(
                       HighestAuthority((check.Initiator != null) ? (new AccountAssignment() { GameType = gameType, UID = check.Initiator.UID }) : (null), check.Permission),
                       HighestAuthority((check.Target    != null) ? (new AccountAssignment() { GameType = gameType, UID = check.Target.UID    }) : (null), check.Permission)
                   );
        }

        /// <summary>
        /// Returns true if the initiator's authority is higher than the target's authority.
        /// </summary>
        public bool Can(Account initiator, CommandName permission, Account target = null)
        {
            return HasAuthority(
                       HighestAuthority((initiator != null) ? (initiator.Username) : (String.Empty), permission),
                       HighestAuthority((target    != null) ? (target.Username)    : (String.Empty), permission)
                   );
        }



        /// <summary>
        /// Returns true if the remote account's password hash matches the local account's password.
        /// </summary>
        public bool Authenticate(Account remoteAccount, String salt)
        {
            Account lAccount = Accounts.Where(x => x.Username == remoteAccount.Username).FirstOrDefault();
            if (lAccount != null && lAccount.Password != null && lAccount.Password.Length > 0)
                return String.Compare(remoteAccount.Password, SHA1.String(lAccount.Password + salt), true) == 0;
            return false;
        }



        /// <summary>
        /// Retrieves a password of an account.
        /// </summary>
        public String Password(Account remoteAccount)
        {
            Account lAccount = Accounts.Where(x => x.Username == remoteAccount.Username).FirstOrDefault();
            if (lAccount != null)
                return lAccount.Password;
            return null;
        }

        /// <summary>
        /// Retrieves an account that contains a specified uid.
        /// </summary>
        public Account Account(GameType gameType, String uid)
        {
            return Accounts
                       .Where(x =>
                              x.Assignments
                                  .Find(y =>
                                        y.GameType == gameType &&
                                        y.UID == uid
                                  ) != null
                       ).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves an account whose username matches the username specified.
        /// </summary>
        public Account Account(String username)
        {
            return Accounts.Where(x => x.Username == username).FirstOrDefault();
        }
    }
}
