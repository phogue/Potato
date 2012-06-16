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

    public class LocalGroup : Group
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
        public override Group Execute()
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
        protected override void WriteConfig(XElement config)
        {
            base.WriteConfig(config);

            foreach (Permission permission in Permissions) {
                if (permission.Authority != null) {
                    config.Add(new XElement("command",
                        new XAttribute("name",         CommandName.SecurityGroupsSetPermission),
                        new XElement("groupName",      Name),
                        new XElement("permissionName", permission.Name),
                        new XElement("authority",      permission.Authority)
                    ));
                }
            }
        }

        #endregion



        /// <summary>
        /// Assigns events to be handled by this class.
        /// </summary>
        protected override void AssignEvents()
        {
            base.AssignEvents();
            AccountAdded   += Accounts_AccountAdded;
            AccountRemoved += Accounts_AccountRemoved;
            foreach (Permission permission in Permissions)
                permission.AuthorityChanged += new Permission.AuthorityChangedHandler(Permission_AuthorityChanged);
        }

        /// <summary>
        /// Notify the layer that an account was added.
        /// </summary>
        private void Accounts_AccountAdded(Group parent, Account item)
        {
            Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.None,
                EventName.SecurityGroupsAccountAssigned,
                Name,
                item.Username
            );
        }

        /// <summary>
        /// Notify the layer that an account was removed.
        /// </summary>
        private void Accounts_AccountRemoved(Group parent, Account item)
        {
            Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.None,
                EventName.SecurityGroupsAccountRevoked,
                Name,
                item.Username
            );
        }

        /// <summary>
        /// Notify the layer that a permission was changed.
        /// </summary>
        private void Permission_AuthorityChanged(Permission sender, int? authority) {
            Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.None,
                EventName.SecurityGroupsPermissionAuthorityChanged,
                Name,
                sender.Name,
                authority
            );
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName">The unique name of the group.  Group.Name</param>
        /// <param name="permissionName">The matching enum name from PermissionName</param>
        /// <param name="authority">The amount of power the user has on the permission</param>
        [Command(Command = CommandName.SecurityGroupsSetPermission)]
        // TODO: Now that the flat files are gone this no longer requires a group name parameter.
        public override void SetPermission(CommandInitiator initiator, string groupName, string permissionName, int authority) {
            if (initiator.CommandOrigin == CommandOrigin.Remote && this.Security.Can(this.Security.Account(initiator.Username), initiator.Command) == false) {
                return;
            }
            
            if (this.Name == groupName) {
                CommandName permissionEnumName = CommandName.None;

                if (Enum.IsDefined(typeof(CommandName), permissionName) == true && (permissionEnumName = (CommandName)Enum.Parse(typeof(CommandName), permissionName)) != CommandName.None) {

                    Permission permission = this.Permissions.Where(x => x.Name == permissionEnumName).FirstOrDefault();

                    if (permission != null) {
                        permission.Authority = authority;
                    }
                }
            }
        }
    }
}
