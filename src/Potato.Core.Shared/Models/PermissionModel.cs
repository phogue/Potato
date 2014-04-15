#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;

namespace Potato.Core.Shared.Models {
    /// <summary>
    /// Permissions are modeled on Teamspeak3's permissions.
    /// Keep it familier with what people know.
    /// </summary>
    [Serializable]
    public class PermissionModel : IDisposable {
        /// <summary>
        /// The command being executed. This is the only value used to match up a command.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The command to be executed, will be converted to a string in Name
        /// </summary>
        public CommandType CommandType {
            get { return this._mCommandType; }
            set {
                this._mCommandType = value;

                if (this._mCommandType != CommandType.None) {
                    this.Name = value.ToString();
                }
            }
        }
        private CommandType _mCommandType;

        /// <summary>
        /// A short, sweet description about what the permission does.
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// A list of traits describing how this permission should be handled.
        /// "Boolean" for example woud mean only applicable values is 0/null for nothing
        /// and anything above 0 for complete control. This permission
        /// is never used against another account so level of authority is
        /// never a permission to deny access.
        /// </summary>
        public List<String> Traits { get; set; }

        /// <summary>
        /// The power/value they have for this permission
        /// null means no value has been set for this group.
        /// </summary>
        public int? Authority { get; set; }

        /// <summary>
        /// Initializes the permission with default values.
        /// </summary>
        public PermissionModel() {
            this.Authority = null;
            this.Traits = new List<String>();
        }

        public void Dispose() {
            this.CommandType = CommandType.None;
            this.Name = null;
            this.Authority = null;
            this.Traits.Clear();
            this.Traits = null;
            this.Description = null;
        }
    }
}
