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
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Procon.Core.Interfaces.Security.Objects {
    using Procon.Core.Interfaces.Layer;
    using Procon.Core.Interfaces.Layer.Objects;

    public abstract class Group : Executable<Group>
    {
        // Public Objects
        public  String Name {
            get { return mName; }
            set {
                if (mName != value) {
                    mName = value;
                    OnPropertyChanged(this, "Name");
                }
            }
        }
        private String mName;
        
        public  List<Permission> Permissions {
            get { return mPermissions; }
            set {
                if (mPermissions != value) {
                    mPermissions = value;
                    OnPropertyChanged(this, "Permissions");
                }
            }
        }
        private List<Permission> mPermissions;
        
        public  List<Account> AssignedAccounts {
            get { return mAssignedAccounts; }
            set {
                if (mAssignedAccounts != value) {
                    mAssignedAccounts = value;
                    OnPropertyChanged(this, "AssignedAccounts");
                }
            }
        }
        private List<Account> mAssignedAccounts;

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

        // Default Initialization
        public Group() : base() {
            Name             = String.Empty;
            AssignedAccounts = new List<Account>();
            Permissions      = new List<Permission>();
            foreach (CommandName name in Enum.GetValues(typeof(CommandName)))
                if (name != CommandName.None)
                    Permissions.Add(new Permission() { Name = name });
        }


        
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
        protected override void WriteConfig(XElement config) { }

        #endregion
        #region Events

        // -- Adding an Account --
        public delegate void AccountAddedHandler(Group parent, Account item);
        public event         AccountAddedHandler AccountAdded;
        protected void OnAccountAdded(Group parent, Account item)
        {
            if (AccountAdded != null)
                AccountAdded(parent, item);
        }

        // -- Removing an Account --
        public delegate void AccountRemovedHandler(Group parent, Account item);
        public event         AccountRemovedHandler AccountRemoved;
        protected void OnAccountRemoved(Group parent, Account item)
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
        private void Layer_ProcessLayerEvent(String username, Context context, CommandName command, EventName @event, Object[] parameters) {
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
        /// 
        /// </summary>
        /// <param name="groupName">The unique name of the group.  Group.Name</param>
        /// <param name="permissionName">The matching enum name from PermissionName</param>
        /// <param name="authority">The amount of power the user has on the permission</param>
        public abstract void SetPermission(CommandInitiator initiator, string groupName, string permissionName, int authority);
    }
}
