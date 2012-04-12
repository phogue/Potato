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
    using Procon.Net.Protocols;

    public abstract class Account : Executable<Account>
    {
        // Public Objects
        public  String Username {
            get { return mUsername; }
            set {
                if (mUsername != value) {
                    mUsername = value;
                    OnPropertyChanged(this, "Username");
                }
            }
        }
        private String mUsername;

        [JsonIgnore]
        public  String Password {
            get { return mPassword; }
            set {
                if (mPassword != value) {
                    mPassword = value;
                    OnPropertyChanged(this, "Password");
                }
            }
        }
        private String mPassword;

        public  String PreferredLanguageCode {
            // http://en.wikipedia.org/wiki/List_of_ISO_639-1_codes
            get { return mPreferredLanguageCode; }
            set {
                if (mPreferredLanguageCode != value) {
                    mPreferredLanguageCode = value;
                    OnPropertyChanged(this, "PreferredLanguageCode");
                }
            }
        }
        private String mPreferredLanguageCode;

        public  List<AccountAssignment> Assignments {
            get { return mAssignments; }
            set {
                if (mAssignments != value) {
                    mAssignments = value;
                    OnPropertyChanged(this, "Assignments");
                }
            }
        }
        private List<AccountAssignment> mAssignments;

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
        public Account() : base() {
            Username              = String.Empty;
            Password              = String.Empty;
            PreferredLanguageCode = String.Empty;
            Assignments           = new List<AccountAssignment>();
        }



        #region Executable

        /// <summary>
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public override Account Execute()
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
        protected override void WriteConfig(XElement config, ref FileInfo xFile) { }

        #endregion
        #region Events

        // -- Adding an Assignment --
        public delegate void AssignmentAddedHandler(Account parent, AccountAssignment item);
        public event         AssignmentAddedHandler AssignmentAdded;
        protected void OnAssignmentAdded(Account parent, AccountAssignment item)
        {
            if (AssignmentAdded != null)
                AssignmentAdded(parent, item);
        }

        // -- Removing an Assignment --
        public delegate void AssignmentRemovedHandler(Account parent, AccountAssignment item);
        public event         AssignmentRemovedHandler AssignmentRemoved;
        protected void OnAssignmentRemoved(Account parent, AccountAssignment item)
        {
            if (AssignmentRemoved != null)
                AssignmentRemoved(parent, item);
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
        /// procon.private.account.assign "Phogue" "CallOfDuty" "101478382" -- guid
        /// procon.private.account.assign "Phogue" "BFBC2" "ABCDABCDABCD" -- cdkey
        /// </summary>
        /// <param name="groupName">The unique name of the account.  Account.Name</param>
        /// <param name="gameName">The name of the game, found in Procon.Core.Connections.Support</param>
        /// <param name="uid">The UID of the player by cd key, name - etc.</param>
        public abstract void Assign(CommandInitiator initiator, string username, GameType gameType, string uid);

        /// <summary>
        /// procon.private.account.revoke "Phogue" "CallOfDuty" "101478382" -- guid
        /// procon.private.account.revoke "Phogue" "BFBC2" "ABCDABCDABCD" -- cdkey
        /// </summary>
        /// <param name="groupName">The unique name of the account.  Account.Name</param>
        /// <param name="gameName">The name of the game, found in Procon.Core.Connections.Support</param>
        /// <param name="uid">The UID of the player by cd key, name - etc.</param>
        public abstract void Revoke(CommandInitiator initiator, string username, GameType gameType, string uid);

        /// <summary>
        /// procon.private.account.setPassword "Phogue" "pass"
        /// procon.private.account.setPassword "Hassan" "password1"
        /// </summary>
        /// <param name="username">The unique name of the account.  Account.Name</param>
        /// <param name="password">The person password to login to the layer.  Account.Password</param>
        public abstract void SetPassword(CommandInitiator initiator, string username, string password);

        /// <summary>
        /// procon.private.account.setPreferredLanguageCode "Phogue" "en"
        /// </summary>
        /// <param name="initiator"></param>
        /// <param name="username">The unique name of the account.  Account.Name</param>
        /// <param name="languageCode">ISO 639-1 preferred language code</param>
        public abstract void SetPreferredLanguageCode(CommandInitiator initiator, string username, string languageCode);
    }
}
