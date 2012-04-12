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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Core.Interfaces.Security.Objects {

    /// <summary>
    /// Permissions are modeled on Teamspeak3's permissions.
    /// Keep it familier with what people know.
    /// </summary>
    public class Permission {

        public delegate void AuthorityChangedHandler(Permission sender, int? authority);
        public event AuthorityChangedHandler AuthorityChanged;

        /// <summary>
        /// The internal name of the group.
        /// 
        /// This group name can be used to lookup a key in a localization file
        /// to present a more friendly name to the user.
        /// </summary>
        public CommandName Name { get; set; }

        /// <summary>
        /// The power/value they have for this permission
        /// null means no value has been set for this group.
        /// </summary>
        private int? m_authority { get; set; }
        public int? Authority {
            get {
                return m_authority;
            }
            set {
                if (this.AuthorityChanged != null) {
                    this.AuthorityChanged(this, value);
                }

                this.m_authority = value;
            }
        }
        
        public Permission() {
            this.Name = CommandName.None;
            this.Authority = null;
        }
    }
}
