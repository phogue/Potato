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

using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Procon.Core.Interfaces.Security.Objects {
    using Procon.Core.Interfaces.Layer.Objects;

    public class RemoteGroup : Group
    {
        #region Executable

        /// <summary>
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public override Group Execute()
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
        protected override void WriteConfig(XElement config, ref FileInfo xFile) { }

        #endregion


        
        /// <summary>
        /// 
        /// </summary>
        [Command(Event = EventName.SecurityGroupsPermissionAuthorityChanged)]
        protected void AuthorityChanged(CommandInitiator initiator, string groupName, CommandName permissionName, int authority) {
            if (Name == groupName) {
                Permission permission = Permissions.Where(x => x.Name == permissionName).FirstOrDefault();
                if (permission != null)
                    permission.Authority = authority;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void SetPermission(CommandInitiator initiator, string groupName, string permissionName, int authority) {
            Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.SecurityGroupsSetPermission,
                EventName.None,
                groupName,
                permissionName,
                authority
            );
        }

    }
}
